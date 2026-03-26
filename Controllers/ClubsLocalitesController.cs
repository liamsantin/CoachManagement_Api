using System.Security.Claims;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClubsLocalitesController : ControllerBase
{
    private readonly IClubsLocalitesService _service;
    public ClubsLocalitesController(IClubsLocalitesService service)=>_service=service;

    [HttpGet]
    public async Task<IActionResult> GetAll(){ if(!TryGetUserId(out var uid)) return Unauthorized(); return Ok(await _service.GetAllAsync(uid)); }

    [HttpGet("{idClub:int}/{idLocalite:int}")]
    public async Task<IActionResult> GetById(int idClub,int idLocalite){ if(!TryGetUserId(out var uid)) return Unauthorized(); var row=await _service.GetByIdAsync(idLocalite,idClub,uid); return row==null?NotFound():Ok(row);}    

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClubsLocalitesA body){ if(!TryGetUserId(out var uid)) return Unauthorized(); if(!ModelState.IsValid) return BadRequest(ModelState); var created=await _service.CreateAsync(body,uid); if(created==null) return Forbid(); return CreatedAtAction(nameof(GetById), new { idClub = created.id_clubs, idLocalite = created.id_localites }, created);}    

    [HttpDelete("{idClub:int}/{idLocalite:int}")]
    public async Task<IActionResult> Delete(int idClub,int idLocalite){ if(!TryGetUserId(out var uid)) return Unauthorized(); return await _service.DeleteAsync(idLocalite,idClub,uid)?NoContent():NotFound(); }

    private bool TryGetUserId(out int userId){userId=0;var claim=User.FindFirstValue(ClaimTypes.NameIdentifier);return !string.IsNullOrEmpty(claim)&&int.TryParse(claim,out userId);}    
}
