namespace ConferenceRooms.Api.Domain.Pricing;

public class PricingService : IPricingService
{
    public PriceBreakdown Calculate(
        decimal basePricePerHour,
        DateTime start,
        DateTime end,
        IEnumerable<decimal> servicePrices
    )
    {
        if (end <= start) throw new ArgumentException("End must be after start");

        decimal rentalTotal = 0;
        List<PriceSegment> segments = new();
        DateTime cursor = start;

        while (cursor < end)
        {
            DateTime nextBoundary = NextBoundaryAfter(cursor);
            DateTime segmentEnd = nextBoundary < end ? nextBoundary : end;
            decimal hours = (decimal)(segmentEnd - cursor).TotalHours;
            decimal multiplier = MultiplierAt(TimeOnly.FromDateTime(cursor));
            decimal amount = hours * basePricePerHour * multiplier;

            segments.Add(new PriceSegment(cursor, segmentEnd,hours, multiplier, amount));
            rentalTotal += amount;
            cursor = segmentEnd;
        }

        var servicesTotal = servicePrices.Sum();
        return new PriceBreakdown(rentalTotal, servicesTotal, rentalTotal + servicesTotal, segments);
    }

    private static decimal MultiplierAt(TimeOnly clock)
    {
        if (clock < new TimeOnly(6, 0)) return 0.70m;
        if (clock < new TimeOnly(9, 0)) return 0.90m;
        if (clock < new TimeOnly(12, 0)) return 1.00m;
        if (clock < new TimeOnly(14, 0)) return 1.15m;
        if (clock < new TimeOnly(18, 0)) return 1.00m;
        if (clock < new TimeOnly(23, 0)) return 0.80m;
        return 0.70m;
    }

    private static DateTime NextBoundaryAfter(DateTime cursor)
    {
        DateTime d = cursor.Date;
        TimeOnly clock = TimeOnly.FromDateTime(cursor);

        if (clock < new TimeOnly(6, 0)) return d.AddHours(6);
        if (clock < new TimeOnly(9, 0)) return d.AddHours(9);
        if (clock < new TimeOnly(12, 0)) return d.AddHours(12);
        if (clock < new TimeOnly(14, 0)) return d.AddHours(14);
        if (clock < new TimeOnly(18, 0)) return d.AddHours(18);
        if (clock < new TimeOnly(23, 0)) return d.AddHours(23);
        return d.AddDays(1).AddHours(6);
    }
}