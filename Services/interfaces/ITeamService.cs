using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface ITeamService
{
    Task<IReadOnlyList<Team>> GetAllAsync(int userId);

    Task<Team?> GetByIdAsync(int teamId, int userId);

    Task<Team?> CreateAsync(int userId, int? fkClubsId, int fkLeaguesId, string name);

    Task<Team?> UpdateAsync(int teamId, int userId, int? fkClubsId, int fkLeaguesId, string name);

    Task<bool> DeleteAsync(int teamId, int userId);
}
