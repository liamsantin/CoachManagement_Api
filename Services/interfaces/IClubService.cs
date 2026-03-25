using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IClubService
{
    Task<IReadOnlyList<Club>> GetAllAsync(int userId);

    Task<Club?> GetByIdAsync(int clubId, int userId);

    Task<Club?> CreateAsync(int userId, string name);

    Task<Club?> UpdateAsync(int clubId, int userId, string name);

    Task<bool> DeleteAsync(int clubId, int userId);
}
