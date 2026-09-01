namespace ConferenceRooms.Api.Dtos;

public record RevenueReportDto(
    decimal Total,
    IReadOnlyList<RoomRevenueDto> ByRoom,
    IReadOnlyList<ServiceRevenueDto> ByService
);