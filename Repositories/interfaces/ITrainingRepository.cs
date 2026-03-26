using CoachManagement_Api.DTOs.Training;

namespace CoachManagement_Api.Repositories.interfaces;

public interface ITrainingRepository
{
    Task<IReadOnlyList<TrainingResponse>> GetAllByUserIdAndTeamIdAsync(int userId, int teamId);

    Task<TrainingResponse?> GetByIdAndUserIdAsync(int idEvents, int userId);

    Task<TrainingResponse?> CreateAsync(int userId, TrainingCreateRequest request);

    Task<TrainingResponse?> UpdateAsync(int idEvents, int userId, TrainingUpdateRequest request);

    Task<bool> DeleteAsync(int idEvents, int userId);
}
