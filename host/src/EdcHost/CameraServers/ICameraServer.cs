namespace EdcHost.CameraServers;

public interface ICameraServer : IDisposable
{
    static ICameraServer Create()
    {
        return new CameraServer(new CameraFactory());
    }

    List<int> AvailableCameraIndexes { get; }

    List<int> OpenCameraIndexes { get; }

    void CloseCamera(int cameraIndex);

    ICamera? GetCamera(int cameraIndex);

    ICamera OpenCamera(int cameraIndex, ILocator locator);

    void Start();

    void Stop();
}
