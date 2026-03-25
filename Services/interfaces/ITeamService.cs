using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface ITeamService
{
    Task<IReadOnlyList<Team>> GetAllAsync(int userId, CancellationToken cancellationToken = default);

    Task<Team?> GetByIdAsync(int teamId, int userId, CancellationToken cancellationToken = default);

    Task<Team?> CreateAsync(int userId, int? fkClubsId, int fkLeaguesId, string name, CancellationToken cancellationToken = default);

    Task<Team?> UpdateAsync(int teamId, int userId, int? fkClubsId, int fkLeaguesId, string name, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int teamId, int userId, CancellationToken cancellationToken = default);
}
