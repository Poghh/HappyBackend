using System.ComponentModel.DataAnnotations;

namespace Happy.Backend.Api.Models.Requests;

public class StoreInformationRequest
{
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "AppName is required")]
    public string AppName { get; set; } = string.Empty;

    [Required(ErrorMessage = "StoreName is required")]
    public string StoreName { get; set; } = string.Empty;

    [Required(ErrorMessage = "UserName is required")]
    public string UserName { get; set; } = string.Empty;
}
