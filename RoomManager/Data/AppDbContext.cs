using Microsoft.EntityFrameworkCore;
using RoomManager.Models;

namespace RoomManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Bookings> Bookings => Set<Bookings>();
    public DbSet<Rooms> Rooms => Set<Rooms>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
}