using System.Security.Claims;
using CoachManagement_Api.DTOs.Match;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchController : ControllerBase
{
    private readonly IMatchService _service;
    public MatchController(IMatchService service)=>_service=service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int teamId)
    {
        if(!TryGetUserId(out var uid)) return Unauthorized();
        if(teamId<=0) return BadRequest("teamId est requis et doit être > 0.");
        return Ok(await _service.GetAllAsync(uid, teamId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if(!TryGetUserId(out var uid)) return Unauthorized();
        var row = await _service.GetByIdAsync(id, uid);
        return row==null?NotFound():Ok(row);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MatchCreateRequest request)
    {
        if(!TryGetUserId(out var uid)) return Unauthorized();
        if(!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(uid, request);
        if(created==null) return Forbid();
        return CreatedAtAction(nameof(GetById), new{id = created.id_events}, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MatchUpdateRequest request)
    {
        if(!TryGetUserId(out var uid)) return Unauthorized();
        if(!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _service.UpdateAsync(id, uid, request);
        return updated==null?NotFound():Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if(!TryGetUserId(out var uid)) return Unauthorized();
        return await _service.DeleteAsync(id, uid)?NoContent():NotFound();
    }

    private bool TryGetUserId(out int userId)
    {
        userId=0; var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return !string.IsNullOrEmpty(claim) && int.TryParse(claim, out userId);
    }
}
