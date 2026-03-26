using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class TypesMatchService : ITypesMatchService
{
    private readonly ITypesMatchRepository _repository;
    public TypesMatchService(ITypesMatchRepository repository) => _repository = repository;
    public Task<IReadOnlyList<TypesMatch>> GetAllAsync() => _repository.GetAllAsync();
    public Task<TypesMatch?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
