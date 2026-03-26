using CoachManagement_Api.DTOs.Attendance;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ITrainingRepository _trainingRepository;

    public AttendanceService(
        IAttendanceRepository attendanceRepository,
        ITrainingRepository trainingRepository)
    {
        _attendanceRepository = attendanceRepository;
        _trainingRepository = trainingRepository;
    }

    public async Task<IReadOnlyList<Attendance>?> GetByTrainingIdAsync(int trainingId, int userId)
    {
        var training = await _trainingRepository.GetByIdAndUserIdAsync(trainingId, userId);
        if (training == null)
            return null;

        return await _attendanceRepository.GetByTrainingIdAsync(trainingId);
    }

    public Task<Attendance?> GetByIdAsync(int playerId, int trainingId, int userId)
        => _attendanceRepository.GetByIdAsync(playerId, trainingId, userId);

    public Task<Attendance?> CreateAsync(int userId, AttendanceCreateRequest request)
    {
        var entity = new Attendance
        {
            id_players = request.id_players,
            id_trainings = request.id_trainings,
            notes = string.IsNullOrWhiteSpace(request.notes) ? null : request.notes,
            retard = request.retard,
            motif = string.IsNullOrWhiteSpace(request.motif) ? null : request.motif
        };
        return _attendanceRepository.CreateAsync(entity, userId);
    }

    public async Task<Attendance?> UpdateAsync(int playerId, int trainingId, int userId, AttendanceUpdateRequest request)
    {
        var existing = await _attendanceRepository.GetByIdAsync(playerId, trainingId, userId);
        if (existing == null)
            return null;

        existing.notes = string.IsNullOrWhiteSpace(request.notes) ? null : request.notes;
        existing.retard = request.retard;
        existing.motif = string.IsNullOrWhiteSpace(request.motif) ? null : request.motif;

        var ok = await _attendanceRepository.UpdateAsync(existing, userId);
        if (!ok)
            return null;

        return await _attendanceRepository.GetByIdAsync(playerId, trainingId, userId);
    }

    public Task<bool> DeleteAsync(int playerId, int trainingId, int userId)
        => _attendanceRepository.DeleteAsync(playerId, trainingId, userId);
}
