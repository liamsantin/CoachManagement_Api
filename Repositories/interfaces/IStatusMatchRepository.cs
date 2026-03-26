using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IStatusMatchRepository
{
    Task<IReadOnlyList<StatusMatch>> GetAllAsync();
    Task<StatusMatch?> GetByIdAsync(int id);
}
