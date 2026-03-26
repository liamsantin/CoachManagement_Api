using CoachManagement_Api.Entity;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatusMatchController : ControllerBase
{
    private readonly IStatusMatchService _service;
    public StatusMatchController(IStatusMatchService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var row = await _service.GetByIdAsync(id);
        return row == null ? NotFound() : Ok(row);
    }
}
