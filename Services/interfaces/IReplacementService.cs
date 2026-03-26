using CoachManagement_Api.DTOs.Replacement;
using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IReplacementService
{
    Task<IReadOnlyList<Replacement>> GetByMatchIdAsync(int matchId, int userId);
    Task<Replacement?> GetByIdAsync(int id, int userId);
    Task<Replacement?> CreateAsync(int userId, ReplacementCreateRequest request);
    Task<Replacement?> UpdateAsync(int id, int userId, ReplacementUpdateRequest request);
    Task<bool> DeleteAsync(int id, int userId);
}
