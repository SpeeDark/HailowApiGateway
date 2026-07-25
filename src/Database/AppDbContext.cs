using Microsoft.EntityFrameworkCore;

namespace HailowApiGateway.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}