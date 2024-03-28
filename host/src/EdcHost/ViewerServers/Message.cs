using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public record Message
{
    [JsonPropertyName("messageType")]
    public virtual string MessageType { get; init; } = "";

    [JsonIgnore]
    public string Json => JsonSerializer.Serialize((object)this);
}
