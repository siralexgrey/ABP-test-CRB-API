using Microsoft.EntityFrameworkCore;
using ConferenceRooms.Api.Models;

namespace ConferenceRooms.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<RoomService> RoomServices { get; set; } = null!;
    public DbSet<BookingService> BookingServices { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookingService>()
            .HasOne(bs => bs.RoomService)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }
}