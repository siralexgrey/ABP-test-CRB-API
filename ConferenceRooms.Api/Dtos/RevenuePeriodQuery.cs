using System.ComponentModel.DataAnnotations;

namespace ConferenceRooms.Api.Dtos;

public record RevenuePeriodQuery : IValidatableObject
{
    [Required] public DateTime? From { get; set; }
    [Required] public DateTime? To { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (From >= To)
        {
            yield return new ValidationResult(
                "Start of period can not be more or equal end",
                [nameof(From), nameof(To)]
            );
        }
    }
}