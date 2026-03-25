using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ITeamRepository
{
    Task<IReadOnlyList<Team>> GetAllByUserIdAsync(int userId);

    Task<Team?> GetByIdAndUserIdAsync(int teamId, int userId);

    Task<int> CreateAsync(Team team);

    Task<bool> UpdateAsync(Team team, int userId);

    Task<bool> DeleteAsync(int teamId, int userId);
}
