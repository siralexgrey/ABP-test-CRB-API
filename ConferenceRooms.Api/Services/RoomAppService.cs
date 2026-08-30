using ConferenceRooms.Api.Dtos;
using ConferenceRooms.Api.Models;
using ConferenceRooms.Api.Repositories;

namespace ConferenceRooms.Api.Services;

public class RoomAppService : IRoomAppService
{
    private readonly IRoomRepository _repo;

    public RoomAppService(IRoomRepository repo)
    {
        _repo = repo;
    }
    public async Task<List<RoomDto>> GetAllAsync()
    {
        List<Room> rooms = await _repo.GetAllAsync();
        return rooms.Select(ToDto).ToList();
    }
    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        Room? room = await _repo.GetByIdAsync(id);
        return room is null ? null : ToDto(room);
    }
    public async Task<RoomDto> CreateAsync(CreateRoomRequest request)
    {
        Room room = new() {
            Name = request.Name,
            Capacity = request.Capacity,
            BasePricePerHour = request.BasePricePerHour
        };

        _repo.Add(room);
        await _repo.SaveChangesAsync();
        return ToDto(room);
    }
    public async Task<RoomDto?> UpdateAsync(int id, UpdateRoomRequest request)
    {
        Room? room = await _repo.GetByIdAsync(id);
        if (room is null) return null;
        room.Name = request.Name;
        room.BasePricePerHour = request.BasePricePerHour;
        room.Capacity = request.Capacity;
        await _repo.SaveChangesAsync();
        return ToDto(room);
    }
    public async Task<bool> DeleteAsync(int id)
    {
        Room? room = await _repo.GetByIdAsync(id);
        if (room is null) return false;
        _repo.Remove(room);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<List<RoomDto>> GetAvailableAsync(DateTime start, DateTime end, int minCapacity)
    {
        List<Room> rooms = await _repo.GetAvailableAsync(start, end, minCapacity);
        return rooms.Select(ToDto).ToList();
    }

    private static RoomDto ToDto(Room room)
    {
        return new RoomDto(room.Id, room.Name, room.Capacity, room.BasePricePerHour);
    }
}