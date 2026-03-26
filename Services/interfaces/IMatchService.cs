using CoachManagement_Api.DTOs.Match;

namespace CoachManagement_Api.Services.interfaces;

public interface IMatchService
{
    Task<IReadOnlyList<MatchResponse>> GetAllAsync(int userId, int teamId);
    Task<MatchResponse?> GetByIdAsync(int idEvents, int userId);
    Task<MatchResponse?> CreateAsync(int userId, MatchCreateRequest request);
    Task<MatchResponse?> UpdateAsync(int idEvents, int userId, MatchUpdateRequest request);
    Task<bool> DeleteAsync(int idEvents, int userId);
}
