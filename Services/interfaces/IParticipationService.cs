using CoachManagement_Api.DTOs.Participation;
using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IParticipationService
{
    Task<IReadOnlyList<Participation>> GetByMatchIdAsync(int matchId, int userId);
    Task<Participation?> GetByIdAsync(int playerId, int matchId, int userId);
    Task<Participation?> CreateAsync(int userId, ParticipationCreateRequest request);
    Task<Participation?> UpdateAsync(int playerId, int matchId, int userId, ParticipationUpdateRequest request);
    Task<bool> DeleteAsync(int playerId, int matchId, int userId);
}
