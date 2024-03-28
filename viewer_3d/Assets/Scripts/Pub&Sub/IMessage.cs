using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDCViewer.Messages
{
    public interface IMessage
    {

        public enum MessageType
        {
            CompetitionControlCommand,

            CompetitionUpdate,

            Error,

            HostConfigurationFromClient,

            HostConfigurationFromServer
        }

        public abstract MessageType Type { get; }

        /// <summary>
        /// Gets the JSON representation of the message.
        /// </summary>
        public JToken Json { get; }

        /// <summary>
        /// Gets the JSON string representation of the message.
        /// </summary>
        public string JsonString { get; }   

        public static MessageType ConvertToMessageType(string messageTypeString)
        {
            messageTypeString= StringParser.ToCamelCase(messageTypeString);
            // 将全大写+下划线的字符串转换为枚举类型
            if (Enum.TryParse<MessageType>(messageTypeString, ignoreCase: true, out MessageType messageType))
            {
                return messageType;
            }
            else
            {
                // 处理无效的字符串，例如抛出异常或返回默认值
                throw new ArgumentException("Invalid MessageType string: " + messageTypeString);
            }
        }
    }
}
