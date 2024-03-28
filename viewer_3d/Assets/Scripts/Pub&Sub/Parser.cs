using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System;
using EDCViewer.Messages;
using System.Diagnostics;

namespace EDCViewer.Messages
{
    /// <summary>
    /// Parses messages from JSON.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses a message from JSON.
        /// </summary>
        public static IMessage Parse(JToken json)
        {
            return Parse(json.ToString()!);
        }

        /// <summary>
        /// Parses a message from JSON.
        /// </summary>
        public static IMessage Parse(string jsonString)
        {
            JObject json = JObject.Parse(jsonString); // 解析 JSON 字符串
            string messageTypeString = (string)json["messageType"]; // 获取 "messageType" 字段的值

            IMessage.MessageType type = IMessage.ConvertToMessageType(messageTypeString);

            return type switch
            {
                IMessage.MessageType.CompetitionControlCommand =>
                  JsonConvert.DeserializeObject<CompetitionControlCommand>(jsonString)!,
                IMessage.MessageType.CompetitionUpdate =>
                  JsonConvert.DeserializeObject<CompetitionUpdate>(jsonString)!,
                IMessage.MessageType.Error =>
                  JsonConvert.DeserializeObject<Error>(jsonString)!,
                IMessage.MessageType.HostConfigurationFromClient =>
                  JsonConvert.DeserializeObject<HostConfigurationFromClient>(jsonString)!,
                IMessage.MessageType.HostConfigurationFromServer =>
                  JsonConvert.DeserializeObject<HostConfigurationFromServer>(jsonString)!,

                _ => throw new Exception("The message is not supported"),
            };
        }
    }
}
