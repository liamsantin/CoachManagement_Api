using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface ILeagueService
{
    Task<IReadOnlyList<League>> GetAllAsync();
    Task<League?> GetByIdAsync(int id);
}
