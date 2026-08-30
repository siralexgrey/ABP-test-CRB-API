using ConferenceRooms.Api.Domain;

namespace ConferenceRooms.Tests;

public class BookingOverlapTests
{
    [Theory]
    [InlineData(10, 12, 12, 14, false)]
    [InlineData(10, 12, 11, 13, true)]
    [InlineData(10, 12, 09, 11, true)]
    [InlineData(10, 12, 09, 14, true)]
    [InlineData(10, 12, 10.5, 11.5, true)]
    [InlineData(10, 12, 08, 10, false)]
    [InlineData(10, 12, 14, 16, false)]
    public void Overlaps_ReturnsExpected_ForIntervalPair(double startHour1, double endHour1, double startHour2, double endHour2, bool expected)
    {
        DateTime day = new DateTime(2026, 1, 1);
        DateTime At(double hour) => day.AddHours(hour);
        DateTime existingStart = At(startHour1);
        DateTime existingEnd = At(endHour1);
        DateTime requestStart = At(startHour2);
        DateTime requestEnd = At(endHour2);

        bool result = BookingOverlap.Overlaps(existingStart, existingEnd, requestStart, requestEnd);
        Assert.Equal(expected, result);
    }
}