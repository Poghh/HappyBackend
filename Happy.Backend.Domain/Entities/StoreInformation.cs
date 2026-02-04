namespace Happy.Backend.Domain.Entities;

public class StoreInformation
{
    public int Id { get; set; }
    public int AppCredentialId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public AppCredential? AppCredential { get; set; }
}
