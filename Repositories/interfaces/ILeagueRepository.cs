using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ILeagueRepository
{
    Task<IReadOnlyList<League>> GetAllAsync();
    Task<League?> GetByIdAsync(int id);
}
