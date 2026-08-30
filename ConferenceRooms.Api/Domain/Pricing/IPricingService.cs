namespace ConferenceRooms.Api.Domain.Pricing;

public interface IPricingService
{
    PriceBreakdown Calculate(
        decimal basePricePerHour,
        DateTime start,
        DateTime end,
        IEnumerable<decimal> servicePrices
    );
}