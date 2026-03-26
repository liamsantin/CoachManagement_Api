using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IFormationService
{
    Task<IReadOnlyList<Formation>> GetAllAsync();
    Task<Formation?> GetByIdAsync(int id);
    Task<Formation?> CreateAsync(Formation entity);
    Task<Formation?> UpdateAsync(int id, Formation entity);
    Task<bool> DeleteAsync(int id);
}
