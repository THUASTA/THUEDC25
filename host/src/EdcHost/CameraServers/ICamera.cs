using Emgu.CV.CvEnum;

namespace EdcHost.CameraServers;

public interface ICamera : IDisposable
{
    int CameraIndex { get; }
    int Height { get; }
    bool IsOpened { get; }
    byte[]? JpegData { get; }
    ILocator Locator { get; set; }
    int Width { get; }
    Tuple<float, float>? TargetLocation { get; }
    Tuple<float, float>? TargetLocationNotCalibrated { get; }

    void Close();
    void Open();
    double GetProperty(CapProp property);
    void SetProperty(CapProp property, double value);
}
