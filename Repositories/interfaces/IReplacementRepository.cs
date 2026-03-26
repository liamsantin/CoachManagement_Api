using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IReplacementRepository
{
    Task<IReadOnlyList<Replacement>> GetByMatchIdAsync(int matchId, int userId);
    Task<Replacement?> GetByIdAsync(int id, int userId);
    Task<Replacement?> CreateAsync(Replacement entity, int userId);
    Task<bool> UpdateAsync(Replacement entity, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
