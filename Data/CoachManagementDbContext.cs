using Microsoft.EntityFrameworkCore;

namespace CoachManagement_Api.Data;

public class CoachManagementDbContext : DbContext
{
    public CoachManagementDbContext(DbContextOptions<CoachManagementDbContext> options)
        : base(options)
    {
    }
}
