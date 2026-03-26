using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class PositionService : IPositionService
{
    private readonly IPositionRepository _repository;
    public PositionService(IPositionRepository repository) => _repository = repository;
    public Task<IReadOnlyList<Position>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Position?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
}
