using CoachManagement_Api.DTOs.Training;

namespace CoachManagement_Api.Services.interfaces;

public interface ITrainingService
{
    Task<IReadOnlyList<TrainingResponse>> GetAllAsync(int userId, int teamId);

    Task<TrainingResponse?> GetByIdAsync(int idEvents, int userId);

    Task<TrainingResponse?> CreateAsync(int userId, TrainingCreateRequest request);

    Task<TrainingResponse?> UpdateAsync(int idEvents, int userId, TrainingUpdateRequest request);

    Task<bool> DeleteAsync(int idEvents, int userId);
}
