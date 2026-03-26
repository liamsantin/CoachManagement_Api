using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IClubsLocalitesService
{
    Task<IReadOnlyList<ClubsLocalitesA>> GetAllAsync(int userId);
    Task<ClubsLocalitesA?> GetByIdAsync(int idLocalite, int idClub, int userId);
    Task<ClubsLocalitesA?> CreateAsync(ClubsLocalitesA entity, int userId);
    Task<bool> DeleteAsync(int idLocalite, int idClub, int userId);
}
