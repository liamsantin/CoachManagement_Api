using CoachManagement_Api.DTOs.Geo;
using CoachManagement_Api.Entity;
using CoachManagement_Api.Repositories.interfaces;
using CoachManagement_Api.Services.interfaces;

namespace CoachManagement_Api.Services;

public class CantonService : ICantonService
{
    private readonly ICantonRepository _cantonRepository;

    public CantonService(ICantonRepository cantonRepository)
    {
        _cantonRepository = cantonRepository;
    }

    public async Task<IReadOnlyList<CantonResponse>> GetAllAsync()
    {
        var list = await _cantonRepository.GetAllAsync();
        return list.Select(ToResponse).ToList();
    }

    public async Task<CantonResponse?> GetByIdAsync(int id)
    {
        var entity = await _cantonRepository.GetByIdAsync(id);
        return entity == null ? null : ToResponse(entity);
    }

    private static CantonResponse ToResponse(Canton e)
    {
        return new CantonResponse
        {
            id_cantons = e.id_cantons,
            code = e.code,
            name = e.name
        };
    }
}
