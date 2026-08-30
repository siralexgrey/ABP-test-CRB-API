namespace ConferenceRooms.Api.Domain;

public static class BookingOverlap
{
    public static bool Overlaps(DateTime existingStart, DateTime existingEnd, DateTime requestStart, DateTime requestEnd)
    {
        return existingStart < requestEnd && requestStart < existingEnd;
    }
}