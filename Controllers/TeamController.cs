using System.Security.Claims;
using CoachManagement_Api.DTOs.Team;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Team>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var teams = await _teamService.GetAllAsync(userId);
        return Ok(teams);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var team = await _teamService.GetByIdAsync(id, userId);
        if (team == null)
            return NotFound();

        return Ok(team);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Team), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] TeamCreateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var team = await _teamService.CreateAsync(userId, request.fk_clubs_id, request.fk_leagues_id, request.name);
        return CreatedAtAction(nameof(GetById), new { id = team!.id_teams }, team);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] TeamUpdateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var team = await _teamService.UpdateAsync(id, userId, request.fk_clubs_id, request.fk_leagues_id, request.name);
        if (team == null)
            return NotFound();

        return Ok(team);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var deleted = await _teamService.DeleteAsync(id, userId);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out userId))
            return false;
        return true;
    }
}
