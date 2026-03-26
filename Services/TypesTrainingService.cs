using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class TypesTrainingService : ITypesTrainingService
{
    private readonly ITypesTrainingRepository _repository;
    public TypesTrainingService(ITypesTrainingRepository repository) => _repository = repository;
    public Task<IReadOnlyList<TypesTraining>> GetAllAsync() => _repository.GetAllAsync();
    public Task<TypesTraining?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
