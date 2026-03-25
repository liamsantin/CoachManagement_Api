using CoachManagement_Api.DTOs.User;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Retourne les infos de l'utilisateur connecté (JWT).</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userService.GetCurrentUserAsync();
        if (user == null)
            return NotFound("Utilisateur non trouvé.");
        return Ok(user);
    }

    /// <summary>Retourne les infos d'un utilisateur par son id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("Utilisateur non trouvé.");
        return Ok(user);
    }
}
