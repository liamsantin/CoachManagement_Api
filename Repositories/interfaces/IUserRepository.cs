using CoachManagement_Api.Models;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string? email, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(User user, CancellationToken cancellationToken = default);
}
