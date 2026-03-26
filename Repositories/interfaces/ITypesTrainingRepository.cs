using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ITypesTrainingRepository
{
    Task<IReadOnlyList<TypesTraining>> GetAllAsync();
    Task<TypesTraining?> GetByIdAsync(int id);
}
