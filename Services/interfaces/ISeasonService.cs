using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface ISeasonService
{
    Task<IReadOnlyList<Season>> GetAllAsync();
    Task<Season?> GetByIdAsync(int id);
    Task<Season?> CreateAsync(Season entity);
    Task<Season?> UpdateAsync(int id, Season entity);
    Task<bool> DeleteAsync(int id);
}
