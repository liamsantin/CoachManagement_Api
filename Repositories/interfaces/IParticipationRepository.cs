using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IParticipationRepository
{
    Task<IReadOnlyList<Participation>> GetByMatchIdAsync(int matchId, int userId);
    Task<Participation?> GetByIdAsync(int playerId, int matchId, int userId);
    Task<Participation?> CreateAsync(Participation entity, int userId);
    Task<bool> UpdateAsync(Participation entity, int userId);
    Task<bool> DeleteAsync(int playerId, int matchId, int userId);
}
