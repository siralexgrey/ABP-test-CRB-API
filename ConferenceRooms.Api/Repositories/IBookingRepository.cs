using ConferenceRooms.Api.Models;

namespace ConferenceRooms.Api.Repositories;
public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    void Add(Booking booking);
    Task SaveChangesAsync();
    Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end);
    Task<List<Booking>> GetInPeriodAsync(DateTime from, DateTime to);
}