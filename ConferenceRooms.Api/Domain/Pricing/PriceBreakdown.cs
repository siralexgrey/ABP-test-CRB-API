namespace ConferenceRooms.Api.Domain.Pricing;

public record PriceBreakdown(
    decimal RentalTotal,
    decimal ServicesTotal,
    decimal Total,
    IReadOnlyList<PriceSegment> Segments
);

public record PriceSegment(
    DateTime Start,
    DateTime End,
    decimal Hours,
    decimal Multiplier,
    decimal Amount
);