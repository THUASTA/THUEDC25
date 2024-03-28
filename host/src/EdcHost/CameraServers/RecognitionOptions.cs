namespace EdcHost.CameraServers;

public record RecognitionOptions
{
    public bool Calibrate = false;
    public float TopLeftX;
    public float TopLeftY;
    public float TopRightX;
    public float TopRightY;
    public float BottomLeftX;
    public float BottomLeftY;
    public float BottomRightX;
    public float BottomRightY;
    public float HueCenter;
    public float HueRange;
    public float SaturationCenter;
    public float SaturationRange;
    public float ValueCenter;
    public float ValueRange;
    public float MinArea;
    public bool ShowMask;
}
