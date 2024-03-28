using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public record HostConfigurationFromServerMessage : Message
{
    [JsonPropertyName("messageType")]
    [JsonRequired]
    public override string MessageType { get; init; } = "HOST_CONFIGURATION_FROM_SERVER";

    [JsonPropertyName("availableCameras")]
    [JsonRequired]
    public List<int> AvailableCameras { get; init; } = new();

    [JsonPropertyName("availableSerialPorts")]
    [JsonRequired]
    public List<string> AvailableSerialPorts { get; init; } = new();


    [JsonPropertyName("configuration")]
    [JsonRequired]
    public HostConfiguration Configuration { get; init; } = new();
}
