namespace ConferenceRooms.Api.Dtos;

public record ServiceRevenueDto(
    int RoomServiceId,
    string Name,
    decimal Amount
);