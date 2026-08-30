namespace ConferenceRooms.Api.Models;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public decimal BasePricePerHour { get; set; }
    public ICollection<RoomService> Services { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}