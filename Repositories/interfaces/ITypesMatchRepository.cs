using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ITypesMatchRepository
{
    Task<IReadOnlyList<TypesMatch>> GetAllAsync();
    Task<TypesMatch?> GetByIdAsync(int id);
}
