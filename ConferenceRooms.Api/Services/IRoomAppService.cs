using ConferenceRooms.Api.Dtos;

namespace ConferenceRooms.Api.Services;

public interface IRoomAppService
{
    Task<List<RoomDto>> GetAllAsync();
    Task<RoomDto?> GetByIdAsync(int id);
    Task<RoomDto> CreateAsync(CreateRoomRequest request);
    Task<RoomDto?> UpdateAsync(int id, UpdateRoomRequest request);
    Task<bool> DeleteAsync(int id);
    Task<List<RoomDto>> GetAvailableAsync(DateTime start, DateTime end, int minCapacity);
}