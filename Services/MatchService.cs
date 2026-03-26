using CoachManagement_Api.DTOs.Match;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _repo;
    public MatchService(IMatchRepository repo)=>_repo=repo;
    public Task<IReadOnlyList<MatchResponse>> GetAllAsync(int userId, int teamId)=>_repo.GetAllByUserIdAndTeamIdAsync(userId, teamId);
    public Task<MatchResponse?> GetByIdAsync(int idEvents, int userId)=>_repo.GetByIdAndUserIdAsync(idEvents, userId);
    public Task<MatchResponse?> CreateAsync(int userId, MatchCreateRequest request)=>_repo.CreateAsync(userId, request);
    public Task<MatchResponse?> UpdateAsync(int idEvents, int userId, MatchUpdateRequest request)=>_repo.UpdateAsync(idEvents, userId, request);
    public Task<bool> DeleteAsync(int idEvents, int userId)=>_repo.DeleteAsync(idEvents, userId);
}
