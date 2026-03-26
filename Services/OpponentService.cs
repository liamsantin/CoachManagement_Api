using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class OpponentService : IOpponentService
{
    private readonly IOpponentRepository _repo;
    public OpponentService(IOpponentRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<Opponent>> GetAllAsync()=>_repo.GetAllAsync();
    public Task<Opponent?> GetByIdAsync(int id)=>_repo.GetByIdAsync(id);
    public async Task<Opponent?> CreateAsync(Opponent entity){var id=await _repo.CreateAsync(entity);return await _repo.GetByIdAsync(id);}    
    public async Task<Opponent?> UpdateAsync(int id, Opponent entity){var existing=await _repo.GetByIdAsync(id); if(existing==null) return null; entity.id_opponents=id; var ok=await _repo.UpdateAsync(entity); return ok?await _repo.GetByIdAsync(id):null;}
    public Task<bool> DeleteAsync(int id)=>_repo.DeleteAsync(id);
}
