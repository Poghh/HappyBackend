using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Happy.Backend.Api.Models.Requests;

public class SyncRequest : IValidatableObject
{
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;

    public DateTime SyncTime { get; set; }

    public JsonElement SyncData { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SyncTime == default)
            yield return new ValidationResult("SyncTime is required", [nameof(SyncTime)]);

        if (SyncData.ValueKind == JsonValueKind.Undefined || SyncData.ValueKind == JsonValueKind.Null)
            yield return new ValidationResult("SyncData is required", [nameof(SyncData)]);
    }
}
