using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ITeamRepository
{
    Task<IReadOnlyList<Team>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<Team?> GetByIdAndUserIdAsync(int teamId, int userId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Team team, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Team team, int userId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int teamId, int userId, CancellationToken cancellationToken = default);
}
