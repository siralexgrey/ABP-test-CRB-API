using Microsoft.Data.Sqlite;
using ConferenceRooms.Api.Data;
using ConferenceRooms.Api.Models;
using ConferenceRooms.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRooms.Tests;

public class AvailabilityTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private static readonly DateTime Day = new(2026, 1, 1);
    private static DateTime At(double hour) => Day.AddHours(hour);

    public AvailabilityTest()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetAvailable_ReturnRoom_WhenCapacityEnough_NoBookings()
    {
        AddRoom("A", 20);
        _context.SaveChanges();

        List<Room> result = await GetAvailable(10, 12, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAvailable_ExcludeRoom_WhenCapacityBelowMin()
    {
        AddRoom("A", 5);
        AddRoom("B", 20);
        _context.SaveChanges();

        List<Room> result = await GetAvailable(10, 12, 10);

        Room room = Assert.Single(result);
        Assert.Equal("B", room.Name);
    }

    [Fact]
    public async Task GetAvailable_ExcludeRoom_WhenBookingOverlap()
    {
        Room room = AddRoom("A", 20);
        AddBooking(room, 10, 12);
        _context.SaveChanges();

        List<Room> result = await GetAvailable(11, 13, 10);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAvailable_ReturnRoom_WhenBookingStartAtPreviousEnd()
    {
        Room room = AddRoom("A", 20);
        AddBooking(room, 12, 14);
        _context.SaveChanges();

        List<Room> result = await GetAvailable(10, 12, 10);

        Room singleRoom = Assert.Single(result);
        Assert.Equal("A", singleRoom.Name);
    }

    private Room AddRoom(string name, int capacity)
    {
        Room room = new() { Name = name, Capacity = capacity, BasePricePerHour = 10 };
        _context.Rooms.Add(room);
        return room;
    }

    private void AddBooking(Room room, double fromHour, double toHour)
    {
        Booking booking = new()
        {
            Room = room,
            StartTime = At(fromHour),
            EndTime = At(toHour),
            TotalPrice = 0,
            CreatedAt = At(0)
        };
        _context.Bookings.Add(booking);
    }

    private Task<List<Room>> GetAvailable(double fromHour, double toHour, int minCapacity)
    {
        return new RoomRepository(_context).GetAvailableAsync(At(fromHour), At(toHour), minCapacity);
    }
}