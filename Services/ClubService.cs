using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class ClubService : IClubService
{
    private readonly IClubRepository _clubRepository;

    public ClubService(IClubRepository clubRepository)
    {
        _clubRepository = clubRepository;
    }

    public Task<IReadOnlyList<Club>> GetAllAsync(int userId)
        => _clubRepository.GetAllByUserIdAsync(userId);

    public Task<Club?> GetByIdAsync(int clubId, int userId)
        => _clubRepository.GetByIdAndUserIdAsync(clubId, userId);

    public async Task<Club?> CreateAsync(int userId, string name)
    {
        var club = new Club
        {
            fk_users_id = userId,
            name = name
        };

        var id = await _clubRepository.CreateAsync(club);
        return await _clubRepository.GetByIdAndUserIdAsync(id, userId);
    }

    public async Task<Club?> UpdateAsync(int clubId, int userId, string name)
    {
        var existing = await _clubRepository.GetByIdAndUserIdAsync(clubId, userId);
        if (existing == null)
            return null;

        existing.name = name;

        var updated = await _clubRepository.UpdateAsync(existing, userId);
        if (!updated)
            return null;

        return await _clubRepository.GetByIdAndUserIdAsync(clubId, userId);
    }

    public Task<bool> DeleteAsync(int clubId, int userId)
        => _clubRepository.DeleteAsync(clubId, userId);
}
