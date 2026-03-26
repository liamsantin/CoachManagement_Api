using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IClubsLocalitesRepository
{
    Task<IReadOnlyList<ClubsLocalitesA>> GetByUserIdAsync(int userId);
    Task<ClubsLocalitesA?> GetByIdAsync(int idLocalite, int idClub, int userId);
    Task<ClubsLocalitesA?> CreateAsync(ClubsLocalitesA entity, int userId);
    Task<bool> DeleteAsync(int idLocalite, int idClub, int userId);
}
