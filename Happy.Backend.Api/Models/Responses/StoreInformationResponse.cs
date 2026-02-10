namespace Happy.Backend.Api.Models.Responses;

public class StoreInformationResponse
{
    public int Id { get; set; }
    public int AppCredentialId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
