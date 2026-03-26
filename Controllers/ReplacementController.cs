using System.Security.Claims;
using CoachManagement_Api.DTOs.Replacement;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReplacementController : ControllerBase
{
    private readonly IReplacementService _service;
    public ReplacementController(IReplacementService service)=>_service=service;

    [HttpGet("by-match/{matchId:int}")]
    public async Task<IActionResult> GetByMatch(int matchId){if(!TryGetUserId(out var uid)) return Unauthorized(); return Ok(await _service.GetByMatchIdAsync(matchId,uid));}
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id){if(!TryGetUserId(out var uid)) return Unauthorized(); var row=await _service.GetByIdAsync(id,uid); return row==null?NotFound():Ok(row);}    
    [HttpPost] public async Task<IActionResult> Create([FromBody] ReplacementCreateRequest r){if(!TryGetUserId(out var uid)) return Unauthorized(); if(!ModelState.IsValid) return BadRequest(ModelState); var created=await _service.CreateAsync(uid,r); if(created==null) return Forbid(); return CreatedAtAction(nameof(GetById),new{id=created.id_replacements},created);}    
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,[FromBody] ReplacementUpdateRequest r){if(!TryGetUserId(out var uid)) return Unauthorized(); if(!ModelState.IsValid) return BadRequest(ModelState); var updated=await _service.UpdateAsync(id,uid,r); return updated==null?NotFound():Ok(updated);}    
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){if(!TryGetUserId(out var uid)) return Unauthorized(); return await _service.DeleteAsync(id,uid)?NoContent():NotFound();}

    private bool TryGetUserId(out int userId){userId=0;var claim=User.FindFirstValue(ClaimTypes.NameIdentifier);return !string.IsNullOrEmpty(claim)&&int.TryParse(claim,out userId);}    
}
