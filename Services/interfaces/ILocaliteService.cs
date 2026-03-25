using CoachManagement_Api.DTOs.Geo;

namespace CoachManagement_Api.Services.interfaces;

public interface ILocaliteService
{
    Task<IReadOnlyList<LocaliteResponse>> GetAllAsync();

    Task<LocaliteResponse?> GetByIdAsync(int id);
}
