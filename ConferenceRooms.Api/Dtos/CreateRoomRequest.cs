namespace ConferenceRooms.Api.Dtos;

public record CreateRoomRequest(
    string Name,
    int Capacity,
    decimal BasePricePerHour,
    IReadOnlyList<CreateRoomServiceRequest>? Services
);