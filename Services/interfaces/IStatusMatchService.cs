using CoachManagement_Api.Entity;

namespace CoachManagement_Api.Services.interfaces;

public interface IStatusMatchService
{
    Task<IReadOnlyList<StatusMatch>> GetAllAsync();
    Task<StatusMatch?> GetByIdAsync(int id);
}
