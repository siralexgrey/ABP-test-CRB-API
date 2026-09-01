using System.ComponentModel.DataAnnotations;

namespace ConferenceRooms.Api.Dtos;

public record AvailabilityQuery : IValidatableObject
{
    [Required] public DateTime? Start { get; set; }
    [Required] public DateTime? End { get; set; }
    [Required, Range(1, int.MaxValue)] public int? MinCapacity { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Start >= End)
        {
            yield return new ValidationResult(
                "Start of period can not be more or equal end",
                [nameof(Start), nameof(End)]
            );
        }
    }
}