using Microsoft.EntityFrameworkCore;

namespace RoomManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<DbContext> options) : base(options)
    {
        
    }
}