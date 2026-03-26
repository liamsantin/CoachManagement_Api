using CoachManagement_Api.DTOs.Match;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IMatchRepository
{
    Task<IReadOnlyList<MatchResponse>> GetAllByUserIdAndTeamIdAsync(int userId, int teamId);
    Task<MatchResponse?> GetByIdAndUserIdAsync(int idEvents, int userId);
    Task<MatchResponse?> CreateAsync(int userId, MatchCreateRequest request);
    Task<MatchResponse?> UpdateAsync(int idEvents, int userId, MatchUpdateRequest request);
    Task<bool> DeleteAsync(int idEvents, int userId);
}
