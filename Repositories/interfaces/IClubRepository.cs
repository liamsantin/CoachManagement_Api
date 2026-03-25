using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IClubRepository
{
    Task<IReadOnlyList<Club>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Club?> GetByIdAndUserIdAsync(int clubId, int userId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Club club, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Club club, int userId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int clubId, int userId, CancellationToken cancellationToken = default);
}
