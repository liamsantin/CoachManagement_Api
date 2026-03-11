using CoachManagement_Api.DTOs.Auth;

namespace CoachManagement_Api.Services;

public interface IAuthService
{
    Task<RegisterResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
