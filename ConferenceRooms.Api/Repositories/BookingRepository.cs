using ConferenceRooms.Api.Data;
using ConferenceRooms.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRooms.Api.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _db;

    public BookingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _db.Bookings
            .Include(booking => booking.Services)
            .ThenInclude(bookingService => bookingService.RoomService)
            .Include(booking => booking.Room)
            .FirstOrDefaultAsync(booking => booking.Id == id);
    }

    public void Add(Booking booking)
    {
        _db.Bookings.Add(booking);
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }

    public async Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end)
    {
        return await _db.Bookings
            .AnyAsync(booking =>
                booking.RoomId == roomId
                && booking.StartTime < end && start < booking.EndTime
            );
    }

    public async Task<List<Booking>> GetInPeriodAsync(DateTime from, DateTime to)
    {
        return await _db.Bookings
            .Where(booking => booking.StartTime >= from && booking.StartTime < to)
            .Include(booking => booking.Services)
            .ThenInclude(bookingService => bookingService.RoomService)
            .Include(booking => booking.Room)
            .ToListAsync();
    }
}