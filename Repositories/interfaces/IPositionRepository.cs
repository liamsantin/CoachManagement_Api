using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IPositionRepository
{
    Task<IReadOnlyList<Position>> GetAllAsync();
    Task<Position?> GetByIdAsync(int id);
}
