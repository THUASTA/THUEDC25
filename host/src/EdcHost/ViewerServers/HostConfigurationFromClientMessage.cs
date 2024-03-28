using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public record HostConfigurationFromClientMessage : Message
{
    [JsonPropertyName("messageType")]
    [JsonRequired]
    public override string MessageType { get; init; } = "HOST_CONFIGURATION_FROM_CLIENT";

    [JsonPropertyName("token")]
    [JsonRequired]
    public string Token { get; init; } = "";

    [JsonPropertyName("configuration")]
    [JsonRequired]
    public HostConfiguration Configuration { get; init; } = new();
}
