namespace ConferenceRooms.Api.Dtos;

public record RoomRevenueDto(
    int RoomId,
    string RoomName,
    decimal Amount
);