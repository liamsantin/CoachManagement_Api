using CoachManagement_Api.DTOs.Geo;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class LocaliteService : ILocaliteService
{
    private readonly ILocaliteRepository _localiteRepository;

    public LocaliteService(ILocaliteRepository localiteRepository)
    {
        _localiteRepository = localiteRepository;
    }

    public async Task<IReadOnlyList<LocaliteResponse>> GetAllAsync()
    {
        var list = await _localiteRepository.GetAllAsync();
        return list.Select(ToResponse).ToList();
    }

    public async Task<LocaliteResponse?> GetByIdAsync(int id)
    {
        var entity = await _localiteRepository.GetByIdAsync(id);
        return entity == null ? null : ToResponse(entity);
    }

    private static LocaliteResponse ToResponse(Localite e)
    {
        return new LocaliteResponse
        {
            id_localites = e.id_localites,
            fk_cantons_id = e.fk_cantons_id,
            npa = e.npa,
            localite = e.localite
        };
    }
}
