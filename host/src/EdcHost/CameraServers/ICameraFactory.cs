namespace EdcHost.CameraServers;

public interface ICameraFactory
{
    List<int> CameraIndexes { get; }

    ICamera Create(int cameraIndex, ILocator locator);

    void Scan();
}
