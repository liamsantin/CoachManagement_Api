using CoachManagement_Api.DTOs.Lineup;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class LineupService : ILineupService
{
    private readonly ILineupRepository _lineupRepository;

    public LineupService(ILineupRepository lineupRepository)
    {
        _lineupRepository = lineupRepository;
    }

    public Task<IReadOnlyList<Lineup>> GetByMatchIdAsync(int matchId, int userId)
        => _lineupRepository.GetByMatchIdAsync(matchId, userId);

    public Task<Lineup?> GetByIdAsync(int lineupId, int userId)
        => _lineupRepository.GetByIdAsync(lineupId, userId);

    public Task<Lineup?> CreateAsync(int userId, LineupCreateRequest request)
    {
        var lineup = new Lineup
        {
            fk_matchs_id = request.fk_matchs_id,
            fk_formations_id = request.fk_formations_id,
            name = request.name,
            notes = string.IsNullOrWhiteSpace(request.notes) ? null : request.notes
        };
        return _lineupRepository.CreateAsync(lineup, userId);
    }

    public async Task<Lineup?> UpdateAsync(int lineupId, int userId, LineupUpdateRequest request)
    {
        var existing = await _lineupRepository.GetByIdAsync(lineupId, userId);
        if (existing == null)
            return null;

        existing.fk_formations_id = request.fk_formations_id;
        existing.name = request.name;
        existing.notes = string.IsNullOrWhiteSpace(request.notes) ? null : request.notes;

        var ok = await _lineupRepository.UpdateAsync(existing, userId);
        if (!ok)
            return null;

        return await _lineupRepository.GetByIdAsync(lineupId, userId);
    }

    public Task<bool> DeleteAsync(int lineupId, int userId)
        => _lineupRepository.DeleteAsync(lineupId, userId);
}
