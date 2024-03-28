using Emgu.CV;

namespace EdcHost.CameraServers;

public class CameraFactory : ICameraFactory
{
    public List<int> CameraIndexes { get; } = new();

    public CameraFactory()
    {
        Scan();
    }

    public ICamera Create(int cameraIndex, ILocator locator)
    {
        if (!CameraIndexes.Contains(cameraIndex))
        {
            throw new ArgumentException($"Camera index {cameraIndex} is not available");
        }

        return new Camera(cameraIndex, locator);
    }

    static bool IsCameraAvailable(int cameraIndex)
    {
        using VideoCapture capture = new(cameraIndex);

        if (!capture.IsOpened)
        {
            return false;
        }

        using Mat frame = capture.QueryFrame();

        if (frame?.IsEmpty ?? true)
        {
            return false;
        }

        return true;
    }

    public void Scan()
    {
        List<int> cameraIndexes = new();
        int index = 0;
        while (true)
        {
            if (IsCameraAvailable(index))
            {
                cameraIndexes.Add(index);
                ++index;
                continue;
            }

            break;
        }

        CameraIndexes.Clear();
        CameraIndexes.AddRange(cameraIndexes);
    }
}
