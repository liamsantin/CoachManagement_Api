using CoachManagement_Api.DTOs.Auth;

namespace CoachManagement_Api.Services.interfaces;

public interface IAuthService
{
    Task<RegisterResponse?> RegisterAsync(RegisterRequest request);

    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
