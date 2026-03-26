using System.Security.Claims;
using CoachManagement_Api.DTOs.Attendance;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpGet("by-training/{trainingId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<Attendance>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTraining(int trainingId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var list = await _attendanceService.GetByTrainingIdAsync(trainingId, userId);
        if (list == null)
            return NotFound("Training introuvable ou non autorisé pour cet utilisateur.");

        return Ok(list);
    }

    [HttpGet("{playerId:int}/{trainingId:int}")]
    [ProducesResponseType(typeof(Attendance), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int playerId, int trainingId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var row = await _attendanceService.GetByIdAsync(playerId, trainingId, userId);
        if (row == null)
            return NotFound();

        return Ok(row);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Attendance), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] AttendanceCreateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _attendanceService.CreateAsync(userId, request);
        if (created == null)
            return StatusCode(StatusCodes.Status403Forbidden);

        return CreatedAtAction(
            nameof(GetById),
            new { playerId = created.id_players, trainingId = created.id_trainings },
            created);
    }

    [HttpPut("{playerId:int}/{trainingId:int}")]
    [ProducesResponseType(typeof(Attendance), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int playerId, int trainingId, [FromBody] AttendanceUpdateRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _attendanceService.UpdateAsync(playerId, trainingId, userId, request);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{playerId:int}/{trainingId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int playerId, int trainingId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var deleted = await _attendanceService.DeleteAsync(playerId, trainingId, userId);
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
