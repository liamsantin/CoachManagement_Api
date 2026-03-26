using CoachManagement_Api.DTOs.Attendance;
using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Repositories.interfaces;

public interface IAttendanceRepository
{
    Task<IReadOnlyList<Attendance>> GetByTrainingIdAsync(int trainingId);

    Task<Attendance?> GetByIdAsync(int playerId, int trainingId, int userId);

    Task<Attendance?> CreateAsync(Attendance attendance, int userId);

    Task<bool> UpdateAsync(Attendance attendance, int userId);

    Task<bool> DeleteAsync(int playerId, int trainingId, int userId);
}
