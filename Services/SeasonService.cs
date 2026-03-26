using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class SeasonService : ISeasonService
{
    private readonly ISeasonRepository _repo;
    public SeasonService(ISeasonRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<Season>> GetAllAsync()=>_repo.GetAllAsync();
    public Task<Season?> GetByIdAsync(int id)=>_repo.GetByIdAsync(id);
    public async Task<Season?> CreateAsync(Season entity){var id=await _repo.CreateAsync(entity);return await _repo.GetByIdAsync(id);}    
    public async Task<Season?> UpdateAsync(int id, Season entity){var existing=await _repo.GetByIdAsync(id); if(existing==null) return null; entity.id_seasons=id; var ok=await _repo.UpdateAsync(entity); return ok?await _repo.GetByIdAsync(id):null;}
    public Task<bool> DeleteAsync(int id)=>_repo.DeleteAsync(id);
}
