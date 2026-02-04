namespace Happy.Backend.Domain.Entities;

public class FarmerInformation
{
    public int Id { get; set; }
    public int AppCredentialId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public AppCredential? AppCredential { get; set; }
}
