using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface ITypesMatchService
{
    Task<IReadOnlyList<TypesMatch>> GetAllAsync();
    Task<TypesMatch?> GetByIdAsync(int id);
}
