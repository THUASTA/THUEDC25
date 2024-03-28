using Newtonsoft.Json;
using System.Collections.Generic;

namespace EDCViewer.Messages
{

    public record Error : Message
    {

        [JsonProperty("messageType")]
        public override IMessage.MessageType Type => IMessage.MessageType.Error;

        [JsonProperty("errorCode")]
        public int ErrorCode { get; init; }

        [JsonProperty("message")]
        public string Message { get; init; } = string.Empty;
    }
}
