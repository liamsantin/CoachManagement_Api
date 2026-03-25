using CoachManagement_Api.Models;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string? email);
    Task<int> CreateAsync(User user);
}
