using CoachManagement_Api.DTOs.Geo;

namespace CoachManagement_Api.Services.interfaces;

public interface ICantonService
{
    Task<IReadOnlyList<CantonResponse>> GetAllAsync();

    Task<CantonResponse?> GetByIdAsync(int id);
}
