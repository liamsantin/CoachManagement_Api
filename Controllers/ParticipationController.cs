using System.Security.Claims;
using CoachManagement_Api.DTOs.Participation;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ParticipationController : ControllerBase
{
    private readonly IParticipationService _service;
    public ParticipationController(IParticipationService service)=>_service=service;

    [HttpGet("by-match/{matchId:int}")]
    public async Task<IActionResult> GetByMatch(int matchId){if(!TryGetUserId(out var uid)) return Unauthorized(); return Ok(await _service.GetByMatchIdAsync(matchId,uid));}
    [HttpGet("{playerId:int}/{matchId:int}")] public async Task<IActionResult> GetById(int playerId,int matchId){if(!TryGetUserId(out var uid)) return Unauthorized(); var row=await _service.GetByIdAsync(playerId,matchId,uid); return row==null?NotFound():Ok(row);}    
    [HttpPost] public async Task<IActionResult> Create([FromBody] ParticipationCreateRequest r){if(!TryGetUserId(out var uid)) return Unauthorized(); if(!ModelState.IsValid) return BadRequest(ModelState); var created=await _service.CreateAsync(uid,r); if(created==null) return Forbid(); return CreatedAtAction(nameof(GetById),new{playerId=created.id_players,matchId=created.id_matchs},created);}    
    [HttpPut("{playerId:int}/{matchId:int}")] public async Task<IActionResult> Update(int playerId,int matchId,[FromBody] ParticipationUpdateRequest r){if(!TryGetUserId(out var uid)) return Unauthorized(); if(!ModelState.IsValid) return BadRequest(ModelState); var updated=await _service.UpdateAsync(playerId,matchId,uid,r); return updated==null?NotFound():Ok(updated);}    
    [HttpDelete("{playerId:int}/{matchId:int}")] public async Task<IActionResult> Delete(int playerId,int matchId){if(!TryGetUserId(out var uid)) return Unauthorized(); return await _service.DeleteAsync(playerId,matchId,uid)?NoContent():NotFound();}

    private bool TryGetUserId(out int userId){userId=0;var claim=User.FindFirstValue(ClaimTypes.NameIdentifier);return !string.IsNullOrEmpty(claim)&&int.TryParse(claim,out userId);}    
}
