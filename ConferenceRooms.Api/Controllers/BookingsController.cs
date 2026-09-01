using ConferenceRooms.Api.Dtos;
using ConferenceRooms.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRooms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingAppService _service;

    public BookingsController(IBookingAppService service)
    {
        _service = service;
    }

    //GET: api/bookings/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        BookingDto? bookingDto =  await _service.GetByIdAsync(id);
        return bookingDto is null ? NotFound() : bookingDto;
    }

    //POST: api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingRequest request)
    {
        BookingDto bookingDto = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = bookingDto.Id }, bookingDto);
    }
}