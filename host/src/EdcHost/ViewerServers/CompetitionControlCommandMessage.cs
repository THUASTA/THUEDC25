using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public record CompetitionControlCommandMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "COMPETITION_CONTROL_COMMAND";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("command")]
    public string Command { get; init; } = "";
}
