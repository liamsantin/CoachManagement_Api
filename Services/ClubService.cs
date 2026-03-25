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

    public Task<IReadOnlyList<Club>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
        => _clubRepository.GetAllByUserIdAsync(userId, cancellationToken);

    public Task<Club?> GetByIdAsync(int clubId, int userId, CancellationToken cancellationToken = default)
        => _clubRepository.GetByIdAndUserIdAsync(clubId, userId, cancellationToken);

    public async Task<Club?> CreateAsync(int userId, string name, CancellationToken cancellationToken = default)
    {
        var club = new Club
        {
            fk_users_id = userId,
            name = name
        };

        var id = await _clubRepository.CreateAsync(club, cancellationToken);
        return await _clubRepository.GetByIdAndUserIdAsync(id, userId, cancellationToken);
    }

    public async Task<Club?> UpdateAsync(int clubId, int userId, string name, CancellationToken cancellationToken = default)
    {
        var existing = await _clubRepository.GetByIdAndUserIdAsync(clubId, userId, cancellationToken);
        if (existing == null)
            return null;

        existing.name = name;

        var updated = await _clubRepository.UpdateAsync(existing, userId, cancellationToken);
        if (!updated)
            return null;

        return await _clubRepository.GetByIdAndUserIdAsync(clubId, userId, cancellationToken);
    }

    public Task<bool> DeleteAsync(int clubId, int userId, CancellationToken cancellationToken = default)
        => _clubRepository.DeleteAsync(clubId, userId, cancellationToken);
}
