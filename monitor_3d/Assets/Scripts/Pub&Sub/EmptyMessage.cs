using System.Collections.Generic;
using EDCViewer.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDCViewer.Messages
{

    public record EmptyMessage :Message
    {
        [JsonConverter(typeof(CommandEnumConverter))]
        [JsonProperty("messageType")]
        public override IMessage.MessageType Type { get; }
    }
}
