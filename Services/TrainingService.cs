using CoachManagement_Api.DTOs.Training;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class TrainingService : ITrainingService
{
    private readonly ITrainingRepository _trainingRepository;

    public TrainingService(ITrainingRepository trainingRepository)
    {
        _trainingRepository = trainingRepository;
    }

    public Task<IReadOnlyList<TrainingResponse>> GetAllAsync(int userId, int teamId)
        => _trainingRepository.GetAllByUserIdAndTeamIdAsync(userId, teamId);

    public Task<TrainingResponse?> GetByIdAsync(int idEvents, int userId)
        => _trainingRepository.GetByIdAndUserIdAsync(idEvents, userId);

    public Task<TrainingResponse?> CreateAsync(int userId, TrainingCreateRequest request)
        => _trainingRepository.CreateAsync(userId, request);

    public Task<TrainingResponse?> UpdateAsync(int idEvents, int userId, TrainingUpdateRequest request)
        => _trainingRepository.UpdateAsync(idEvents, userId, request);

    public Task<bool> DeleteAsync(int idEvents, int userId)
        => _trainingRepository.DeleteAsync(idEvents, userId);
}
