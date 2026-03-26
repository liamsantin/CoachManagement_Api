using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IPlayerRepository
{
    Task<IReadOnlyList<Player>> GetAllByUserIdAndTeamIdAsync(int userId, int teamId);

    Task<Player?> GetByIdAndUserIdAsync(int playerId, int userId);

    Task<int?> CreateAsync(Player player, int userId);

    Task<bool> UpdateAsync(Player player, int userId);

    Task<bool> DeleteAsync(int playerId, int userId);
}
