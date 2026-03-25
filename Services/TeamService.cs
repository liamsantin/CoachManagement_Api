using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;

    public TeamService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public Task<IReadOnlyList<Team>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
        => _teamRepository.GetAllByUserIdAsync(userId, cancellationToken);

    public Task<Team?> GetByIdAsync(int teamId, int userId, CancellationToken cancellationToken = default)
        => _teamRepository.GetByIdAndUserIdAsync(teamId, userId, cancellationToken);

    public async Task<Team?> CreateAsync(int userId, int? fkClubsId, int fkLeaguesId, string name, CancellationToken cancellationToken = default)
    {
        var team = new Team
        {
            fk_users_id = userId,
            fk_clubs_id = fkClubsId,
            fk_leagues_id = fkLeaguesId,
            name = name
        };

        var id = await _teamRepository.CreateAsync(team, cancellationToken);
        return await _teamRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken);
    }

    public async Task<Team?> UpdateAsync(int teamId, int userId, int? fkClubsId, int fkLeaguesId, string name, CancellationToken cancellationToken = default)
    {
        var existing = await _teamRepository.GetByIdAndUserIdAsync(teamId, userId, cancellationToken);
        if (existing == null)
            return null;

        existing.fk_clubs_id = fkClubsId;
        existing.fk_leagues_id = fkLeaguesId;
        existing.name = name;

        var updated = await _teamRepository.UpdateAsync(existing, userId, cancellationToken);
        if (!updated)
            return null;

        return await _teamRepository.GetByIdAndUserIdAsync(teamId, userId, cancellationToken);
    }

    public Task<bool> DeleteAsync(int teamId, int userId, CancellationToken cancellationToken = default)
        => _teamRepository.DeleteAsync(teamId, userId, cancellationToken);
}
