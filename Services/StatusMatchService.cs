using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class StatusMatchService : IStatusMatchService
{
    private readonly IStatusMatchRepository _repository;
    public StatusMatchService(IStatusMatchRepository repository) => _repository = repository;
    public Task<IReadOnlyList<StatusMatch>> GetAllAsync() => _repository.GetAllAsync();
    public Task<StatusMatch?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
