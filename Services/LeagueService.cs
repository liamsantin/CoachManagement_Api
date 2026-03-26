using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class LeagueService : ILeagueService
{
    private readonly ILeagueRepository _leagueRepository;

    public LeagueService(ILeagueRepository leagueRepository)
    {
        _leagueRepository = leagueRepository;
    }

    public Task<IReadOnlyList<League>> GetAllAsync() => _leagueRepository.GetAllAsync();

    public Task<League?> GetByIdAsync(int id) => _leagueRepository.GetByIdAsync(id);
}
