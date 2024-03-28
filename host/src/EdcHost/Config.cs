using System.Text.Json.Serialization;

namespace EdcHost;

public record Config
{
    public record GameType
    {
        [JsonPropertyName("diamondMines")]
        public List<Tuple<int, int>> DiamondMines { get; init; } = new();

        [JsonPropertyName("goldMines")]
        public List<Tuple<int, int>> GoldMines { get; init; } = new();

        [JsonPropertyName("ironMines")]
        public List<Tuple<int, int>> IronMines { get; init; } = new();
    }

    [JsonPropertyName("loggingLevel")]
    public string LoggingLevel { get; init; } = "Information";

    [JsonPropertyName("serverPort")]
    public int ServerPort { get; init; } = 8080;

    [JsonPropertyName("game")]
    public GameType Game { get; init; } = new();
}
