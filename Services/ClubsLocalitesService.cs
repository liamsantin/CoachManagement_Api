using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class ClubsLocalitesService : IClubsLocalitesService
{
    private readonly IClubsLocalitesRepository _repo;
    public ClubsLocalitesService(IClubsLocalitesRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<ClubsLocalitesA>> GetAllAsync(int userId)=>_repo.GetByUserIdAsync(userId);
    public Task<ClubsLocalitesA?> GetByIdAsync(int idLocalite, int idClub, int userId)=>_repo.GetByIdAsync(idLocalite,idClub,userId);
    public Task<ClubsLocalitesA?> CreateAsync(ClubsLocalitesA entity, int userId)=>_repo.CreateAsync(entity,userId);
    public Task<bool> DeleteAsync(int idLocalite, int idClub, int userId)=>_repo.DeleteAsync(idLocalite,idClub,userId);
}
