using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ILocaliteRepository
{
    Task<IReadOnlyList<Localite>> GetAllAsync();

    Task<Localite?> GetByIdAsync(int id);
}
