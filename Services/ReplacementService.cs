using CoachManagement_Api.DTOs.Replacement;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class ReplacementService : IReplacementService
{
    private readonly IReplacementRepository _repo;
    public ReplacementService(IReplacementRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<Replacement>> GetByMatchIdAsync(int matchId, int userId)=>_repo.GetByMatchIdAsync(matchId,userId);
    public Task<Replacement?> GetByIdAsync(int id, int userId)=>_repo.GetByIdAsync(id,userId);
    public Task<Replacement?> CreateAsync(int userId, ReplacementCreateRequest r)=>_repo.CreateAsync(new Replacement{minute=r.minute,fk_matchs_id=r.fk_matchs_id,fk_play_entering=r.fk_play_entering,fk_play_outgoing=r.fk_play_outgoing},userId);
    public async Task<Replacement?> UpdateAsync(int id, int userId, ReplacementUpdateRequest r){var e=await _repo.GetByIdAsync(id,userId); if(e==null) return null; e.minute=r.minute; e.fk_play_entering=r.fk_play_entering; e.fk_play_outgoing=r.fk_play_outgoing; var ok=await _repo.UpdateAsync(e,userId); return ok?await _repo.GetByIdAsync(id,userId):null;}
    public Task<bool> DeleteAsync(int id, int userId)=>_repo.DeleteAsync(id,userId);
}
