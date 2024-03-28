using EDCViewer.Messages;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using static EDCViewer.Messages.CompetitionControlCommand;

public class CommandEnumConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Command) || objectType == typeof(IMessage.MessageType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            string value = reader.Value.ToString();

            if (objectType == typeof(Command))
            {
                if (Enum.TryParse(typeof(Command), StringParser.ToCamelCase(value), out object command))
                {
                    return command;
                }
            }
            else if (objectType == typeof(IMessage.MessageType))
            {
                if (Enum.TryParse(typeof(IMessage.MessageType), StringParser.ToCamelCase(value), out object messageType))
                {
                    return messageType;
                }
            }
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is Command command)
        {
            writer.WriteValue(StringParser.ToSnakeCase(command.ToString()));
        }
        else if (value is IMessage.MessageType messageType)
        {
            writer.WriteValue(StringParser.ToSnakeCase(messageType.ToString()));
        } 
    }
}
