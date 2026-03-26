using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IPlayersLineupRepository
{
    Task<IReadOnlyList<PlayersLineup>> GetByLineupIdAsync(int lineupId, int userId);
    Task<PlayersLineup?> GetByIdAsync(int idPlayersLineup, int userId);
    Task<PlayersLineup?> CreateAsync(PlayersLineup entity, int userId);
    Task<bool> UpdateAsync(PlayersLineup entity, int userId);
    Task<bool> DeleteAsync(int idPlayersLineup, int userId);
}
