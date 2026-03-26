using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IFormationRepository
{
    Task<IReadOnlyList<Formation>> GetAllAsync();
    Task<Formation?> GetByIdAsync(int id);
    Task<int> CreateAsync(Formation entity);
    Task<bool> UpdateAsync(Formation entity);
    Task<bool> DeleteAsync(int id);
}
