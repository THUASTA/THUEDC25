using Newtonsoft.Json;
using System.Collections.Generic;

namespace EDCViewer.Messages
{
    public record CompetitionControlCommand : Message
    {
        public enum Command
        {
            Start,

            End,

            Reset,

            GetHostConfiguration
        }
        [JsonConverter(typeof(CommandEnumConverter))] 
        [JsonProperty("messageType")]
        public override IMessage.MessageType Type => IMessage.MessageType.CompetitionControlCommand;

        [JsonProperty("token")]
        public string Token { get; init; } = string.Empty;

        [JsonConverter(typeof(CommandEnumConverter))] 
        [JsonProperty("command")]
        public Command command { get; init; } = new();
    }
}
