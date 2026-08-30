namespace ConferenceRooms.Api.Dtos;

public record UpdateRoomRequest(
    string Name,
    int Capacity,
    decimal BasePricePerHour
);