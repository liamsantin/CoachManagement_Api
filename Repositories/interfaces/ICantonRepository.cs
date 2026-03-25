using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ICantonRepository
{
    Task<IReadOnlyList<Canton>> GetAllAsync();

    Task<Canton?> GetByIdAsync(int id);
}
