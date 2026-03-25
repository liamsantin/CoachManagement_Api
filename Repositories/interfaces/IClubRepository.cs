using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IClubRepository
{
    Task<IReadOnlyList<Club>> GetAllByUserIdAsync(int userId);

    Task<Club?> GetByIdAndUserIdAsync(int clubId, int userId);

    Task<int> CreateAsync(Club club);

    Task<bool> UpdateAsync(Club club, int userId);

    Task<bool> DeleteAsync(int clubId, int userId);
}
