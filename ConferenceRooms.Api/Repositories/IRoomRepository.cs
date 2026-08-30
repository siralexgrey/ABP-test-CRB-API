using ConferenceRooms.Api.Models;

namespace ConferenceRooms.Api.Repositories;

public interface IRoomRepository
{
    Task<List<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(int id);
    void Add(Room room);
    void Update(Room room);
    void Remove(Room room);
    Task SaveChangesAsync();
    Task<List<Room>> GetAvailableAsync(DateTime start, DateTime end, int minCapacity);
}