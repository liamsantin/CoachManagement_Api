using CoachManagement_Api.DTOs.User;

namespace CoachManagement_Api.Services.interfaces;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(int id);
    Task<UserResponse?> GetCurrentUserAsync();
}
