using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoachManagement_Api.DTOs.Auth;
using CoachManagement_Api.Models;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;
using Microsoft.IdentityModel.Tokens;

namespace CoachManagement_Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
    {
        var existingByUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingByUsername != null)
            return null; // Username déjà pris

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingByEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingByEmail != null)
                return null; // Email déjà utilisé
        }

        var user = new User
        {
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            Phone = request.Phone
        };

        var id = await _userRepository.CreateAsync(user);
        return new RegisterResponse
        {
            Id = id,
            Username = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            Message = "Inscription réussie."
        };
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return null;

        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes());

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            UserId = user.IdUsers,
            ExpiresAt = expiresAt
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "CoachManagement_Api";
        var audience = _configuration["Jwt:Audience"] ?? "CoachManagement_Api";
        var expirationMinutes = GetJwtExpirationMinutes();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsers.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetJwtExpirationMinutes()
    {
        var value = _configuration["Jwt:ExpirationMinutes"];
        return int.TryParse(value, out var minutes) && minutes > 0 ? minutes : 60;
    }
}
