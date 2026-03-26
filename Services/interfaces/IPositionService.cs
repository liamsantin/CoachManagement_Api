using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IPositionService
{
    Task<IReadOnlyList<Position>> GetAllAsync();
    Task<Position?> GetByIdAsync(int id);
}
