using Newtonsoft.Json;
using System.Collections.Generic;

namespace EDCViewer.Messages
{

    public record HostConfigurationFromServer : Message
    {
        public record PlayerInfo
        {

            [JsonProperty("playerId")]
            public int PlayerId { get; init; }

            [JsonProperty("camera")]
            public Camera camera { get; init; } = new();

            [JsonProperty("serialPort")]
            public SerialPort serialPort { get; init; } = new();

            public record Camera
            {
                [JsonProperty("cameraId")]
                public int CameraId { get; init; }

                [JsonProperty("calibration")]
                public Calibration calibration { get; init; } = new();

                [JsonProperty("recognition")]
                public Recognition recognition { get; init; } = new();       

                public record Calibration
                {
                    [JsonProperty("topLeft")]
                    public TopLeft topLeft { get; init; } = new();

                    [JsonProperty("topRight")]
                    public TopRight topRight { get; init; } = new();    

                    [JsonProperty("bottomLeft")]
                    public BottomLeft bottomLeft { get; init; } = new();

                    [JsonProperty("bottomRight")]
                    public BottomRight bottomRight { get; init; } = new();    

                    public record TopLeft      
                    {
                        [JsonProperty("x")]
                        public int x { get; init; }

                        [JsonProperty("y")]
                        public int y { get; init; }
                    }        

                    public record TopRight      
                    {
                        [JsonProperty("x")]
                        public int x { get; init; }

                        [JsonProperty("y")]
                        public int y { get; init; }
                    }          

                    public record BottomLeft      
                    {
                        [JsonProperty("x")]
                        public int x { get; init; }

                        [JsonProperty("y")]
                        public int y { get; init; }
                    }   

                    public record BottomRight      
                    {
                        [JsonProperty("x")]
                        public int x { get; init; }

                        [JsonProperty("y")]
                        public int y { get; init; }
                    }                                                                                  
                }    

                public record Recognition
                {
                    [JsonProperty("hueCenter")]
                    public int HueCenter { get; init; }   

                    [JsonProperty("hueRange")]
                    public int HueRange { get; init; }  

                    [JsonProperty("saturationCenter")]
                    public int SaturationCenter { get; init; }       

                    [JsonProperty("saturationRange")]
                    public int SaturationRange { get; init; }    

                    [JsonProperty("valueCenter")]
                    public int ValueCenter { get; init; }    

                    [JsonProperty("valueRange")]
                    public int ValueRange { get; init; }    

                    [JsonProperty("minArea")]
                    public int MinArea { get; init; }    

                    [JsonProperty("showMask")]
                    public bool ShowMask { get; init; }                                                                                                                                                           
                }             
            }

            public record SerialPort
            {
                [JsonProperty("portName")]
                public string PortName { get; init; }

                [JsonProperty("baudRate")]
                public int BaudRate { get; init; }
            }
        }


        [JsonConverter(typeof(CommandEnumConverter))]
        [JsonProperty("messageType")]
        public override IMessage.MessageType Type => IMessage.MessageType.HostConfigurationFromServer;

        [JsonProperty("availableCameras")]
        public List<int> AvailableCameras { get; init; } = new();

        [JsonProperty("availableSerialPorts")]
        public List<string> AvailableSerialPorts { get; init; } = new(); 

        [JsonProperty("players")]
        public List<PlayerInfo> playerInfo { get; init; } = new();
    }
}
