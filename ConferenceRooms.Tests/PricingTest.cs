using ConferenceRooms.Api.Domain.Pricing;

namespace ConferenceRooms.Tests;

public class PricingTest
{
    private readonly DateTime Day = new(2026, 1, 1);
    private DateTime At(double hour) => Day.AddHours(hour);

    [Theory]
    [InlineData(10, 12, 200)]
    [InlineData(11, 13, 215)]
    [InlineData(8, 15, 720)]
    [InlineData(22, 26, 290)]
    [InlineData(23, 30, 490)]
    public void Pricing_ReturnSegmentsAndEqualToExpected_ForIntervals(double start, double end, double expected)
    {
        decimal basePrice = 100;

        PriceBreakdown breakdown = new PricingService().Calculate(basePrice, At(start), At(end), []);
        Assert.Equal((decimal)expected, breakdown.Total);
        Assert.NotEmpty(breakdown.Segments);
    }

    [Fact]
    public void Pricing_ReturnTotalPrice_ForIntervalAndAdditionalServices()
    {
        decimal basePrice = 100;

        PriceBreakdown breakdown = new PricingService().Calculate(basePrice, At(10), At(12), [50, 30]);
        Assert.Equal(80, breakdown.ServicesTotal);
        Assert.Equal(280, breakdown.Total);
        Assert.Throws<ArgumentException>(() => new PricingService().Calculate(basePrice, At(12), At(10), []));
    }

    [Fact]
    public void Pricing_Throws_WhenEndNotAfterStart() =>
        Assert.Throws<ArgumentException>(() => new PricingService().Calculate(100, At(12), At(10), []));
}