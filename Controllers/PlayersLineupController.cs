using System.Security.Claims;
using CoachManagement_Api.DTOs.PlayersLineup;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayersLineupController : ControllerBase
{
    private readonly IPlayersLineupService _service;
    public PlayersLineupController(IPlayersLineupService service)=>_service=service;

    [HttpGet("by-lineup/{lineupId:int}")]
    public async Task<IActionResult> GetByLineup(int lineupId){if(!TryGetUserId(out var uid)) return Unauthorized(); return Ok(await _service.GetByLineupIdAsync(lineupId,uid));}
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){if(!TryGetUserId(out var uid)) return Unauthorized(); var row=await _service.GetByIdAsync(id,uid); return row==null?NotFound():Ok(row);}    
    [HttpPost] public async Task<IActionResult> Create([FromBody] PlayersLineupCreateRequest r){if(!TryGetUserId(out var uid)) return Unauthorized(); if(!ModelState.IsValid) return BadRequest(ModelState); var created=await _service.CreateAsync(uid,r); if(created==null) return Forbid(); return CreatedAtAction(nameof(GetById),new{id=created.id_playersLineup},created);}    
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,[FromBody] PlayersLineupUpdateRequest r){if(!TryGetUserId(out var uid)) return Unauthorized(); if(!ModelState.IsValid) return BadRequest(ModelState); var updated=await _service.UpdateAsync(id,uid,r); return updated==null?NotFound():Ok(updated);}    
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){if(!TryGetUserId(out var uid)) return Unauthorized(); return await _service.DeleteAsync(id,uid)?NoContent():NotFound();}

    private bool TryGetUserId(out int userId){userId=0;var claim=User.FindFirstValue(ClaimTypes.NameIdentifier);return !string.IsNullOrEmpty(claim)&&int.TryParse(claim,out userId);}    
}
