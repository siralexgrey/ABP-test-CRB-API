using ConferenceRooms.Api.Dtos;

namespace ConferenceRooms.Api.Services;

public interface IReportAppService
{
    Task<RevenueReportDto> GetRevenueAsync(DateTime from, DateTime to);
}