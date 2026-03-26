using CoachManagement_Api.DTOs.PlayersLineup;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class PlayersLineupService : IPlayersLineupService
{
    private readonly IPlayersLineupRepository _repo;
    public PlayersLineupService(IPlayersLineupRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<PlayersLineup>> GetByLineupIdAsync(int lineupId, int userId)=>_repo.GetByLineupIdAsync(lineupId,userId);
    public Task<PlayersLineup?> GetByIdAsync(int idPlayersLineup, int userId)=>_repo.GetByIdAsync(idPlayersLineup,userId);
    public Task<PlayersLineup?> CreateAsync(int userId, PlayersLineupCreateRequest r)=>_repo.CreateAsync(new PlayersLineup{fk_players_id=r.fk_players_id,fk_lineup_id=r.fk_lineup_id,fk_positions_id=r.fk_positions_id,titulaire=r.titulaire,numMaillot=r.numMaillot,capitaine=r.capitaine},userId);
    public async Task<PlayersLineup?> UpdateAsync(int idPlayersLineup, int userId, PlayersLineupUpdateRequest r){var e=await _repo.GetByIdAsync(idPlayersLineup,userId); if(e==null) return null; e.fk_positions_id=r.fk_positions_id; e.titulaire=r.titulaire; e.numMaillot=r.numMaillot; e.capitaine=r.capitaine; var ok=await _repo.UpdateAsync(e,userId); return ok?await _repo.GetByIdAsync(idPlayersLineup,userId):null;}
    public Task<bool> DeleteAsync(int idPlayersLineup, int userId)=>_repo.DeleteAsync(idPlayersLineup,userId);
}
