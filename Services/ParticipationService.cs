using CoachManagement_Api.DTOs.Participation;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class ParticipationService : IParticipationService
{
    private readonly IParticipationRepository _repo;
    public ParticipationService(IParticipationRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<Participation>> GetByMatchIdAsync(int matchId, int userId)=>_repo.GetByMatchIdAsync(matchId,userId);
    public Task<Participation?> GetByIdAsync(int playerId, int matchId, int userId)=>_repo.GetByIdAsync(playerId,matchId,userId);
    public Task<Participation?> CreateAsync(int userId, ParticipationCreateRequest r)=>_repo.CreateAsync(new Participation{id_players=r.id_players,id_matchs=r.id_matchs,noteOn10=r.noteOn10,notes=r.notes,tempsJeu=r.tempsJeu,but=r.but,passeD=r.passeD,cartonJaune=r.cartonJaune,cartonRouge=r.cartonRouge},userId);
    public async Task<Participation?> UpdateAsync(int playerId, int matchId, int userId, ParticipationUpdateRequest r){var e=await _repo.GetByIdAsync(playerId,matchId,userId); if(e==null) return null; e.noteOn10=r.noteOn10; e.notes=r.notes; e.tempsJeu=r.tempsJeu; e.but=r.but; e.passeD=r.passeD; e.cartonJaune=r.cartonJaune; e.cartonRouge=r.cartonRouge; var ok=await _repo.UpdateAsync(e,userId); return ok?await _repo.GetByIdAsync(playerId,matchId,userId):null;}
    public Task<bool> DeleteAsync(int playerId, int matchId, int userId)=>_repo.DeleteAsync(playerId,matchId,userId);
}
