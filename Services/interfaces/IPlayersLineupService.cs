using CoachManagement_Api.DTOs.PlayersLineup;
using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IPlayersLineupService
{
    Task<IReadOnlyList<PlayersLineup>> GetByLineupIdAsync(int lineupId, int userId);
    Task<PlayersLineup?> GetByIdAsync(int idPlayersLineup, int userId);
    Task<PlayersLineup?> CreateAsync(int userId, PlayersLineupCreateRequest request);
    Task<PlayersLineup?> UpdateAsync(int idPlayersLineup, int userId, PlayersLineupUpdateRequest request);
    Task<bool> DeleteAsync(int idPlayersLineup, int userId);
}
