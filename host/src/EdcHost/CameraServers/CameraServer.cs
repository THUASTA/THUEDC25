using System.Diagnostics;
using Serilog;

namespace EdcHost.CameraServers;

public class CameraServer : ICameraServer
{
    readonly ICameraFactory _cameraFactory;
    readonly List<ICamera> _cameras = new();
    readonly ILogger _logger = Log.Logger.ForContext("Component", "CameraServers");

    public List<int> AvailableCameraIndexes => _cameraFactory.CameraIndexes;
    public List<int> OpenCameraIndexes => _cameras.Select(x => x.CameraIndex).ToList();
    bool _isRunning = false;

    public CameraServer(ICameraFactory cameraFactory)
    {
        _cameraFactory = cameraFactory;
    }

    public void CloseCamera(int cameraIndex)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        ICamera? camera = _cameras.Find(x => x.CameraIndex == cameraIndex) ??
            throw new ArgumentException($"camera index does not exist: {cameraIndex}");

        camera.Close();
        camera.Dispose();
        _cameras.Remove(camera);

        _logger.Information("Camera {cameraIndex} opened.", cameraIndex);
    }

    public ICamera? GetCamera(int cameraIndex)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        ICamera? camera = _cameras.Find(x => x.CameraIndex == cameraIndex);

        return camera;
    }

    public ICamera OpenCamera(int cameraIndex, ILocator locator)
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        if (_cameras.Any(x => x.CameraIndex == cameraIndex))
        {
            ICamera? camera = _cameras.Find(x => x.CameraIndex == cameraIndex);
            Debug.Assert(camera is not null);
            return camera;
        }
        else
        {
            ICamera camera = _cameraFactory.Create(cameraIndex, locator);
            _cameras.Add(camera);

            _logger.Information("Camera {cameraIndex} opened.", cameraIndex);

            return camera;
        }
    }

    public void Start()
    {
        if (_isRunning is true)
        {
            throw new InvalidOperationException("already running");
        }

        _logger.Information("Starting...");

        _isRunning = true;

        _logger.Information("Started.");
    }

    public void Stop()
    {
        if (_isRunning is false)
        {
            throw new InvalidOperationException("not running");
        }

        _logger.Information("Stopping...");

        _isRunning = false;

        _logger.Information("Stopped.");
    }

    public void Dispose()
    {
        foreach (ICamera camera in _cameras)
        {
            camera.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
