using System.Security.Claims;
using CoachManagement_Api.DTOs.Club;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClubController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubController(IClubService clubService)
    {
        _clubService = clubService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Club>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var clubs = await _clubService.GetAllAsync(userId);
        return Ok(clubs);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Club), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var club = await _clubService.GetByIdAsync(id, userId);
        if (club == null)
            return NotFound();

        return Ok(club);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Club), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] ClubCreateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var club = await _clubService.CreateAsync(userId, request.name);
        return CreatedAtAction(nameof(GetById), new { id = club!.id_clubs }, club);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Club), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] ClubUpdateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var club = await _clubService.UpdateAsync(id, userId, request.name);
        if (club == null)
            return NotFound();

        return Ok(club);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var deleted = await _clubService.DeleteAsync(id, userId);
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
