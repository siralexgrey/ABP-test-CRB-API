namespace ConferenceRooms.Api.Dtos;

public record RoomDto(
    int Id,
    string Name,
    int Capacity,
    decimal BasePricePerHour,
    IReadOnlyList<RoomServiceDto> Services
);
