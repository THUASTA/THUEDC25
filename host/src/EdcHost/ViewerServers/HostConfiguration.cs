using System.Text.Json.Serialization;

namespace EdcHost.ViewerServers;

public record HostConfiguration
{
    public record CameraType
    {
        public record CalibrationType
        {
            public record Point
            {
                [JsonPropertyName("x")]
                [JsonRequired]
                public float X { get; init; } = 0.0f;

                [JsonPropertyName("y")]
                [JsonRequired]
                public float Y { get; init; } = 0.0f;
            }

            [JsonPropertyName("topLeft")]
            [JsonRequired]
            public Point TopLeft { get; init; } = new();

            [JsonPropertyName("topRight")]
            [JsonRequired]
            public Point TopRight { get; init; } = new();

            [JsonPropertyName("bottomLeft")]
            [JsonRequired]
            public Point BottomLeft { get; init; } = new();

            [JsonPropertyName("bottomRight")]
            [JsonRequired]
            public Point BottomRight { get; init; } = new();
        }

        public record PropertiesType
        {
            [JsonPropertyName("frameWidth")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? FrameWidth { get; init; } = null;

            [JsonPropertyName("frameHeight")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? FrameHeight { get; init; } = null;

            [JsonPropertyName("fps")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Fps { get; init; } = null;

            [JsonPropertyName("brightness")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Brightness { get; init; } = null;

            [JsonPropertyName("contrast")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Contrast { get; init; } = null;

            [JsonPropertyName("saturation")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Saturation { get; init; } = null;

            [JsonPropertyName("hue")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Hue { get; init; } = null;

            [JsonPropertyName("gain")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Gain { get; init; } = null;

            [JsonPropertyName("exposure")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Exposure { get; init; } = null;

            [JsonPropertyName("monochrome")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Monochrome { get; init; } = null;

            [JsonPropertyName("sharpness")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Sharpness { get; init; } = null;

            [JsonPropertyName("autoExposure")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? AutoExposure { get; init; } = null;

            [JsonPropertyName("gamma")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Gamma { get; init; } = null;

            [JsonPropertyName("temperature")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Temperature { get; init; } = null;

            [JsonPropertyName("whiteBalanceRedV")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? WhiteBalanceRedV { get; init; } = null;

            [JsonPropertyName("zoom")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Zoom { get; init; } = null;

            [JsonPropertyName("focus")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Focus { get; init; } = null;

            [JsonPropertyName("isoSpeed")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? IsoSpeed { get; init; } = null;

            [JsonPropertyName("iris")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Iris { get; init; } = null;

            [JsonPropertyName("autofocus")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? Autofocus { get; init; } = null;

            [JsonPropertyName("autoWb")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? AutoWb { get; init; } = null;

            [JsonPropertyName("wbTemperature")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public double? WbTemperature { get; init; } = null;
        }

        public record RecognitionType
        {
            [JsonPropertyName("hueCenter")]
            [JsonRequired]
            public float HueCenter { get; init; } = 0;

            [JsonPropertyName("hueRange")]
            [JsonRequired]
            public float HueRange { get; init; } = 0;

            [JsonPropertyName("saturationCenter")]
            [JsonRequired]
            public float SaturationCenter { get; init; } = 0;

            [JsonPropertyName("saturationRange")]
            [JsonRequired]
            public float SaturationRange { get; init; } = 0;

            [JsonPropertyName("valueCenter")]
            [JsonRequired]
            public float ValueCenter { get; init; } = 0;

            [JsonPropertyName("valueRange")]
            [JsonRequired]
            public float ValueRange { get; init; } = 0;

            [JsonPropertyName("minArea")]
            [JsonRequired]
            public float MinArea { get; init; } = 0;

            [JsonPropertyName("showMask")]
            [JsonRequired]
            public bool ShowMask { get; init; } = false;
        }

        [JsonPropertyName("cameraId")]
        [JsonRequired]
        public int CameraId { get; init; } = 0;

        [JsonPropertyName("calibration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CalibrationType? Calibration { get; init; } = null;

        [JsonPropertyName("properties")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PropertiesType? Properties { get; init; } = null;

        [JsonPropertyName("recognition")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RecognitionType? Recognition { get; init; } = null;
    }

    public record PlayerType
    {
        [JsonPropertyName("playerId")]
        [JsonRequired]
        public int PlayerId { get; init; } = 0;

        [JsonPropertyName("camera")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Camera { get; init; } = null;

        [JsonPropertyName("serialPort")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SerialPort { get; init; } = null;
    }

    public record SerialPortType
    {
        [JsonPropertyName("portName")]
        [JsonRequired]
        public string PortName { get; init; } = "";

        [JsonPropertyName("baudRate")]
        [JsonRequired]
        public int BaudRate { get; init; }
    }

    [JsonPropertyName("cameras")]
    [JsonRequired]
    public List<CameraType> Cameras { get; init; } = new();

    [JsonPropertyName("players")]
    [JsonRequired]
    public List<PlayerType> Players { get; init; } = new();

    [JsonPropertyName("serialPorts")]
    [JsonRequired]
    public List<SerialPortType> SerialPorts { get; init; } = new();
}

