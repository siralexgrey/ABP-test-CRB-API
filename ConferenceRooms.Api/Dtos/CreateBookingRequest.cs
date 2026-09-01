using System.ComponentModel.DataAnnotations;

namespace ConferenceRooms.Api.Dtos;

public record CreateBookingRequest(
    [property: Range(1, int.MaxValue)] int RoomId,
    DateTime StartTime,
    TimeSpan Duration,
    IReadOnlyList<int>? ServiceIds
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Duration <= TimeSpan.Zero)
        {
            yield return new ValidationResult(
                "Duration should be more than 0",
                [nameof(Duration)]
            );
        }
        if (Duration > new TimeSpan(24, 0, 0))
        {
            yield return new ValidationResult(
                "Duration cannot be longer than 24 hours",
                [nameof(Duration)]
            );
        }
        if (StartTime == new DateTime())
        {
            yield return new ValidationResult(
                "StartTimes not provided",
                [nameof(StartTime)]
            );
        }
    }
}