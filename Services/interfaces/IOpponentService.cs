using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IOpponentService
{
    Task<IReadOnlyList<Opponent>> GetAllAsync();
    Task<Opponent?> GetByIdAsync(int id);
    Task<Opponent?> CreateAsync(Opponent entity);
    Task<Opponent?> UpdateAsync(int id, Opponent entity);
    Task<bool> DeleteAsync(int id);
}
