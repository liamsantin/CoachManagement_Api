using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IOpponentRepository
{
    Task<IReadOnlyList<Opponent>> GetAllAsync();
    Task<Opponent?> GetByIdAsync(int id);
    Task<int> CreateAsync(Opponent entity);
    Task<bool> UpdateAsync(Opponent entity);
    Task<bool> DeleteAsync(int id);
}
