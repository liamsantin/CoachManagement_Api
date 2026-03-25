using CoachManagement_Api.DTOs.Geo;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocaliteController : ControllerBase
{
    private readonly ILocaliteService _localiteService;

    public LocaliteController(ILocaliteService localiteService)
    {
        _localiteService = localiteService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LocaliteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var list = await _localiteService.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LocaliteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var localite = await _localiteService.GetByIdAsync(id);
        if (localite == null)
            return NotFound();
        return Ok(localite);
    }
}
