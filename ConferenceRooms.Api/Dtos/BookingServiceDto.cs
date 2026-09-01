namespace ConferenceRooms.Api.Dtos;

public record BookingServiceDto(
    int RoomServiceId,
    string Name,
    decimal PriceAtBooking
);