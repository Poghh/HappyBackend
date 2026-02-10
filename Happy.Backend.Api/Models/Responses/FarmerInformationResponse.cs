namespace Happy.Backend.Api.Models.Responses;

public class FarmerInformationResponse
{
    public int Id { get; set; }
    public int AppCredentialId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FarmName { get; set; } = string.Empty;
}
