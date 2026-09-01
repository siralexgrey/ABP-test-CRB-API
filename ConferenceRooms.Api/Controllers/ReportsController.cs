using ConferenceRooms.Api.Dtos;
using ConferenceRooms.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRooms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportAppService _service;

    public ReportsController(IReportAppService service)
    {
        _service = service;
    }

    // GET: api/reports/revenue
    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueReportDto>> GetRevenue([FromQuery] RevenuePeriodQuery query)
    {
        return await _service.GetRevenueAsync(query.From.Value, query.To.Value); // [Required] in DTO guarantees non-null
    }
}