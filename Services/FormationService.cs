using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class FormationService : IFormationService
{
    private readonly IFormationRepository _repo;
    public FormationService(IFormationRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<Formation>> GetAllAsync()=>_repo.GetAllAsync();
    public Task<Formation?> GetByIdAsync(int id)=>_repo.GetByIdAsync(id);
    public async Task<Formation?> CreateAsync(Formation entity){var id=await _repo.CreateAsync(entity);return await _repo.GetByIdAsync(id);}    
    public async Task<Formation?> UpdateAsync(int id, Formation entity){var existing=await _repo.GetByIdAsync(id); if(existing==null) return null; entity.id_formations=id; var ok=await _repo.UpdateAsync(entity); return ok?await _repo.GetByIdAsync(id):null;}
    public Task<bool> DeleteAsync(int id)=>_repo.DeleteAsync(id);
}
