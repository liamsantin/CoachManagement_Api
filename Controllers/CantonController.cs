using CoachManagement_Api.DTOs.Geo;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CantonController : ControllerBase
{
    private readonly ICantonService _cantonService;

    public CantonController(ICantonService cantonService)
    {
        _cantonService = cantonService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CantonResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var list = await _cantonService.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CantonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var canton = await _cantonService.GetByIdAsync(id);
        if (canton == null)
            return NotFound();
        return Ok(canton);
    }
}
