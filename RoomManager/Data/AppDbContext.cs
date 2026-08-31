using Microsoft.EntityFrameworkCore;
using RoomManager.Models;


namespace RoomManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Room> Rooms => Set<Room>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RoomService>()
            .HasKey(roomService => new { roomService.RoomId, roomService.ServiceId });

        modelBuilder.Entity<BookingService>()
            .HasKey(bookingService => new
            {
                bookingService.BookingId,
                bookingService.ServiceId
            });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(booking => booking.StartAt)
                .HasColumnType("timestamp without time zone");

            entity.Property(booking => booking.EndAt)
                .HasColumnType("timestamp without time zone");
        });


        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.HasData(
                new Service
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Проєктор",
                    Price = 500,
                },
                new Service
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Wi-Fi",
                    Price = 300
                },
                new Service
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Звук",
                    Price = 700
                }
            );
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasData(
                new Room()
                {
                    Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                    Name = "Зал А",
                    Capacity = 50,
                    PricePerHour = 2000m
                },
                new Room()
                {
                    Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                    Name = "Зал B",
                    Capacity = 100,
                    PricePerHour = 3500m
                },
                new Room()
                {
                    Id = Guid.Parse("c3333333-3333-3333-3333-333333333333"),
                    Name = "Зал C",
                    Capacity = 30,
                    PricePerHour = 1500m
                }
            );
        });
    }
}