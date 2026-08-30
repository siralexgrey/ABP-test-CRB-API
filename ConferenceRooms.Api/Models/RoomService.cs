namespace ConferenceRooms.Api.Models;

public class RoomService
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public required string Name { get; set; }
    public decimal Price { get; set; }

}