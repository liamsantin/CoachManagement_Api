using CoachManagement_Api.DTOs.Auth;
using CoachManagement_Api.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoachManagement_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Inscription d'un nouvel utilisateur.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null)
            return BadRequest("Données d'inscription invalides.");

        var result = await _authService.RegisterAsync(request);
        if (result == null)
            return Conflict("Un compte existe déjà avec ce nom d'utilisateur ou cette adresse email.");

        return CreatedAtAction(nameof(Register), result);
    }

    /// <summary>Connexion et récupération d'un token JWT.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Identifiant et mot de passe requis.");

        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized("Identifiant ou mot de passe incorrect.");

        return Ok(result);
    }
}
