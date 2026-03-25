using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IClubService
{
    Task<IReadOnlyList<Club>> GetAllAsync(int userId, CancellationToken cancellationToken = default);

    Task<Club?> GetByIdAsync(int clubId, int userId, CancellationToken cancellationToken = default);

    Task<Club?> CreateAsync(int userId, string name, CancellationToken cancellationToken = default);

    Task<Club?> UpdateAsync(int clubId, int userId, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int clubId, int userId, CancellationToken cancellationToken = default);
}
