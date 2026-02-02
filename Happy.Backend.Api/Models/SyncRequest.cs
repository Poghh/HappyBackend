using System.Text.Json;

namespace Happy.Backend.Api.Models;

public class SyncRequest
{
    public string Phone { get; set; } = string.Empty;
    public DateTime SyncTime { get; set; }
    public JsonElement SyncData { get; set; }
}
