using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ISeasonRepository
{
    Task<IReadOnlyList<Season>> GetAllAsync();
    Task<Season?> GetByIdAsync(int id);
    Task<int> CreateAsync(Season entity);
    Task<bool> UpdateAsync(Season entity);
    Task<bool> DeleteAsync(int id);
}
