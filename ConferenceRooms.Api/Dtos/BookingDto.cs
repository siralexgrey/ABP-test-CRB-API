using ConferenceRooms.Api.Domain.Pricing;

namespace ConferenceRooms.Api.Dtos;

public record BookingDto(
    int Id,
    int RoomId,
    DateTime StartTime,
    DateTime EndTime,
    IReadOnlyList<BookingServiceDto> Services,
    PriceBreakdown PriceBreakdown
);