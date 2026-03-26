using CoachManagement_Api.DTOs.Attendance;
using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IAttendanceService
{
    /// <summary>
    /// <see langword="null"/> si le training (id_events) n'existe pas ou n'appartient pas à l'utilisateur ;
    /// sinon une ligne par joueur avec présence (équivalent Players JOIN Attendances_a ; liste vide si aucune présence).
    /// </summary>
    Task<IReadOnlyList<Attendance>?> GetByTrainingIdAsync(int trainingId, int userId);

    Task<Attendance?> GetByIdAsync(int playerId, int trainingId, int userId);

    Task<Attendance?> CreateAsync(int userId, AttendanceCreateRequest request);

    Task<Attendance?> UpdateAsync(int playerId, int trainingId, int userId, AttendanceUpdateRequest request);

    Task<bool> DeleteAsync(int playerId, int trainingId, int userId);
}
