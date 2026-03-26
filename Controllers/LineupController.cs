using System.Security.Claims;
using CoachManagement_Api.DTOs.Lineup;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LineupController : ControllerBase
{
    private readonly ILineupService _lineupService;

    public LineupController(ILineupService lineupService)
    {
        _lineupService = lineupService;
    }

    [HttpGet("by-match/{matchId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<Lineup>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMatch(int matchId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return Ok(await _lineupService.GetByMatchIdAsync(matchId, userId));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Lineup), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var lineup = await _lineupService.GetByIdAsync(id, userId);
        return lineup == null ? NotFound() : Ok(lineup);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Lineup), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] LineupCreateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _lineupService.CreateAsync(userId, request);
        if (created == null)
            return Forbid();

        return CreatedAtAction(nameof(GetById), new { id = created.id_lineup }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Lineup), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] LineupUpdateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _lineupService.UpdateAsync(id, userId, request);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        return await _lineupService.DeleteAsync(id, userId) ? NoContent() : NotFound();
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return !string.IsNullOrWhiteSpace(claim) && int.TryParse(claim, out userId);
    }
}
