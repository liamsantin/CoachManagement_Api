using CoachManagement_Api.DTOs.Player;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public Task<IReadOnlyList<Player>> GetAllAsync(int userId, int teamId)
        => _playerRepository.GetAllByUserIdAndTeamIdAsync(userId, teamId);

    public Task<Player?> GetByIdAsync(int playerId, int userId)
        => _playerRepository.GetByIdAndUserIdAsync(playerId, userId);

    public async Task<Player?> CreateAsync(int userId, PlayerCreateRequest request)
    {
        var player = MapFromCreate(request);

        var id = await _playerRepository.CreateAsync(player, userId);
        if (id == null)
            return null;

        return await _playerRepository.GetByIdAndUserIdAsync(id.Value, userId);
    }

    public async Task<Player?> UpdateAsync(int playerId, int userId, PlayerUpdateRequest request)
    {
        var existing = await _playerRepository.GetByIdAndUserIdAsync(playerId, userId);
        if (existing == null)
            return null;

        existing.fk_teams_id = request.fk_teams_id;
        existing.fk_positions_id = request.fk_positions_id;
        existing.nom = request.nom;
        existing.prenom = request.prenom;
        existing.numeroMaillot = request.numeroMaillot;
        existing.email = string.IsNullOrWhiteSpace(request.email) ? null : request.email.Trim();
        existing.phone = string.IsNullOrWhiteSpace(request.phone) ? null : request.phone.Trim();
        existing.anneeExp = request.anneeExp;
        existing.poids = request.poids;
        existing.taille = request.taille;
        existing.dateNaiss = request.dateNaiss;
        existing.dateArrivee = request.dateArrivee;
        existing.photoUrl = string.IsNullOrWhiteSpace(request.photoUrl) ? null : request.photoUrl.Trim();

        var updated = await _playerRepository.UpdateAsync(existing, userId);
        if (!updated)
            return null;

        return await _playerRepository.GetByIdAndUserIdAsync(playerId, userId);
    }

    public Task<bool> DeleteAsync(int playerId, int userId)
        => _playerRepository.DeleteAsync(playerId, userId);

    private static Player MapFromCreate(PlayerCreateRequest request)
    {
        return new Player
        {
            fk_teams_id = request.fk_teams_id,
            fk_positions_id = request.fk_positions_id,
            nom = request.nom,
            prenom = request.prenom,
            numeroMaillot = request.numeroMaillot,
            email = string.IsNullOrWhiteSpace(request.email) ? null : request.email.Trim(),
            phone = string.IsNullOrWhiteSpace(request.phone) ? null : request.phone.Trim(),
            anneeExp = request.anneeExp,
            poids = request.poids,
            taille = request.taille,
            dateNaiss = request.dateNaiss,
            dateArrivee = request.dateArrivee,
            photoUrl = string.IsNullOrWhiteSpace(request.photoUrl) ? null : request.photoUrl.Trim()
        };
    }
}
