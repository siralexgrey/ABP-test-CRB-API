using ConferenceRooms.Api.Dtos;

namespace ConferenceRooms.Api.Services;

public interface IBookingAppService
{
    Task<BookingDto?> GetByIdAsync(int id);

    Task<BookingDto> CreateAsync(CreateBookingRequest request);
}