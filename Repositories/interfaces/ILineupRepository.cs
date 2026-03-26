using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ILineupRepository
{
    Task<IReadOnlyList<Lineup>> GetByMatchIdAsync(int matchId, int userId);
    Task<Lineup?> GetByIdAsync(int lineupId, int userId);
    Task<Lineup?> CreateAsync(Lineup lineup, int userId);
    Task<bool> UpdateAsync(Lineup lineup, int userId);
    Task<bool> DeleteAsync(int lineupId, int userId);
}
