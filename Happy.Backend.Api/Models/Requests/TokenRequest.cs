using System.ComponentModel.DataAnnotations;

namespace Happy.Backend.Api.Models.Requests;

public class TokenRequest
{
    [Required(ErrorMessage = "AppSecret is required")]
    public string AppSecret { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;
}
