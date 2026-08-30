using Microsoft.EntityFrameworkCore;
using ConferenceRooms.Api.Models;
using ConferenceRooms.Api.Data;

namespace ConferenceRooms.Api.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _db;

    public RoomRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Room>> GetAllAsync()
    {
        return _db.Rooms
            .Include(room => room.Services)
            .ToListAsync();
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _db.Rooms.Include(room => room.Services).FirstOrDefaultAsync(room => room.Id == id);
    }

    public void Add(Room room)
    {
        _db.Rooms.Add(room);
    }

    public void Update(Room room)
    {
        _db.Rooms.Update(room);
    }

    public void Remove(Room room)
    {
        _db.Rooms.Remove(room);
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }

    public Task<List<Room>> GetAvailableAsync(DateTime start, DateTime end, int minCapacity)
    {
        return _db.Rooms
            .Include(room => room.Services)
            .Where(room =>
                room.Capacity >= minCapacity
                && !room.Bookings.Any(booking => booking.StartTime < end && start < booking.EndTime)
            )
            .ToListAsync();
    }
}