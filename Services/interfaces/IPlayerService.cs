using CoachManagement_Api.DTOs.Player;
using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IPlayerService
{
    Task<IReadOnlyList<Player>> GetAllAsync(int userId, int teamId);

    Task<Player?> GetByIdAsync(int playerId, int userId);

    Task<Player?> CreateAsync(int userId, PlayerCreateRequest request);

    Task<Player?> UpdateAsync(int playerId, int userId, PlayerUpdateRequest request);

    Task<bool> DeleteAsync(int playerId, int userId);
}
