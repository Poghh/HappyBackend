using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Happy.Backend.Api.Models.Requests;

public class FarmerInformationRequest
{
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "AppName is required")]
    public string AppName { get; set; } = string.Empty;

    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "FarmName is required")]
    [JsonPropertyName("farmerName")]
    public string FarmName { get; set; } = string.Empty;
}
