using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface ITypesTrainingService
{
    Task<IReadOnlyList<TypesTraining>> GetAllAsync();
    Task<TypesTraining?> GetByIdAsync(int id);
}
