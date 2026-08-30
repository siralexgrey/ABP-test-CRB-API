using ConferenceRooms.Api.Dtos;
using ConferenceRooms.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceRooms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomAppService _service;
    public RoomsController(IRoomAppService service)
    {
        _service = service;
    }

    // GET: api/rooms
    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetAll()
    {
        return await _service.GetAllAsync();
    }

    // GET: api/rooms/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        RoomDto? roomDto = await _service.GetByIdAsync(id);
        return roomDto is null ? NotFound() : roomDto;
    }

    // GET: api/rooms/available?start={start}&end={end}&minCapacity={minCapacity}
    [HttpGet("available")]
    public async Task<ActionResult<List<RoomDto>>> GetAvailable([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int minCapacity)
    {
        return await _service.GetAvailableAsync(start, end, minCapacity);
    }

    // POST: api/rooms
    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create(CreateRoomRequest request)
    {
        RoomDto roomDto = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = roomDto.Id }, roomDto);
    }

    // POST: api/rooms/{id}/services
    [HttpPost("{id}/services")]
    public async Task<ActionResult<RoomServiceDto>> AddService(int id, CreateRoomServiceRequest request)
    {
        RoomServiceDto? serviceDto = await _service.AddServiceAsync(id, request);
        return serviceDto is null ? NotFound() : CreatedAtAction(nameof(GetById), new { id }, serviceDto);
    }

    // PUT: api/rooms/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<RoomDto>> Update(int id, UpdateRoomRequest request)
    {
        RoomDto? roomDto = await _service.UpdateAsync(id, request);
        return roomDto is null ? NotFound() : roomDto;
    }
    // DELETE: api/rooms/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool ok = await _service.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }
}