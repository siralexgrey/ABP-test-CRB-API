using ConferenceRooms.Api.Models;

namespace ConferenceRooms.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Rooms.Any()) return;

        db.Rooms.AddRange(
            new Room
            {
                Name = "A", Capacity = 50, BasePricePerHour = 2000,
                Services =
                {
                    new RoomService { Name = "Projector", Price = 500 },
                    new RoomService { Name = "Wi-Fi", Price = 300 },
                    new RoomService { Name = "Sound", Price = 700 }
                }
            },
            new Room
            {
                Name = "B", Capacity = 100, BasePricePerHour = 3500,
                Services =
                {
                    new RoomService { Name = "Projector", Price = 500 },
                    new RoomService { Name = "Wi-Fi", Price = 300 },
                    new RoomService { Name = "Sound", Price = 700 }
                }
            },
            new Room
            {
                Name = "C", Capacity = 30, BasePricePerHour = 1500,
                Services =
                {
                    new RoomService { Name = "Projector", Price = 500 },
                    new RoomService { Name = "Wi-Fi", Price = 300 },
                    new RoomService { Name = "Sound", Price = 700 }
                }
            }
        );
        db.SaveChanges();
    }
}