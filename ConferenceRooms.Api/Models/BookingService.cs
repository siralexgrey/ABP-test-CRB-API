namespace ConferenceRooms.Api.Models;

public class BookingService
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int RoomServiceId { get; set; }
    public RoomService RoomService { get; set; } = null!;
    public decimal PriceAtBooking { get; set; }
}