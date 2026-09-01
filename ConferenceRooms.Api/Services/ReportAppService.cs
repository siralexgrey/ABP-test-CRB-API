using ConferenceRooms.Api.Dtos;
using ConferenceRooms.Api.Models;
using ConferenceRooms.Api.Repositories;

namespace ConferenceRooms.Api.Services;

public class ReportAppService : IReportAppService
{
    private readonly IBookingRepository _repo;

    public ReportAppService(IBookingRepository repo)
    {
        _repo = repo;
    }

    public async Task<RevenueReportDto> GetRevenueAsync(DateTime start, DateTime end)
    {
        List<Booking> period = await _repo.GetInPeriodAsync(start, end);
        return new RevenueReportDto(
            period.Sum(booking => booking.TotalPrice),
            [.. period
                .GroupBy(booking => booking.RoomId)
                .Select(group => new RoomRevenueDto(group.Key, group.First().Room.Name, group.Sum(g => g.TotalPrice)))
            ],
            [.. period
                .SelectMany(booking => booking.Services)
                .GroupBy(service => service.RoomServiceId)
                .Select(group => new ServiceRevenueDto(group.Key, group.First().RoomService.Name, group.Sum(g => g.PriceAtBooking)))
            ]
        );
    }
}