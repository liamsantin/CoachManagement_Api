using CoachManagement_Api.DTOs.Lineup;
using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface ILineupService
{
    Task<IReadOnlyList<Lineup>> GetByMatchIdAsync(int matchId, int userId);
    Task<Lineup?> GetByIdAsync(int lineupId, int userId);
    Task<Lineup?> CreateAsync(int userId, LineupCreateRequest request);
    Task<Lineup?> UpdateAsync(int lineupId, int userId, LineupUpdateRequest request);
    Task<bool> DeleteAsync(int lineupId, int userId);
}
