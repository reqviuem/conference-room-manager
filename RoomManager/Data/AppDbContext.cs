using Microsoft.EntityFrameworkCore;
using RoomManager.Models;

namespace RoomManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Bookings> Bookings => Set<Bookings>();
    public DbSet<Services> Services => Set<Services>();
    public DbSet<Rooms> Rooms => Set<Rooms>();
    public AppDbContext(DbContextOptions<DbContext> options) : base(options)
    {
        
    }
}