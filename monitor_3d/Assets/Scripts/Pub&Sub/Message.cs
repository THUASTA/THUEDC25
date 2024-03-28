using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}

namespace EDCViewer.Messages
{

    public abstract record Message : IMessage
    {
        [JsonConverter(typeof(CommandEnumConverter))]
        [JsonProperty("messageType")]
        public abstract IMessage.MessageType Type { get; }

        [JsonIgnore]
        public JToken Json
        {
            get => JToken.Parse(JsonConvert.SerializeObject((object)this))!;
        }

        [JsonIgnore]
        public string JsonString
        {
            get => JsonConvert.SerializeObject((object)this);
        }
    }
}
