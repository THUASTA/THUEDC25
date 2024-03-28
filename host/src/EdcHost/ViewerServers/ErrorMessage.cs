using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public record ErrorMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "ERROR";

    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; } = "";
}
