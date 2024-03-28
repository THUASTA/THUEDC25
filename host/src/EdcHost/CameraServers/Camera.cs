using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EdcHost.CameraServers;

public class Camera : ICamera
{
    public int CameraIndex { get; private set; }
    public int Height => _capture.Height;
    public bool IsOpened => _capture.IsOpened;
    public byte[]? JpegData { get; private set; }
    public ILocator Locator { get; set; }
    public Tuple<float, float>? TargetLocation { get; private set; }
    public Tuple<float, float>? TargetLocationNotCalibrated { get; private set; }
    public int Width => _capture.Width;

    VideoCapture _capture;
    Task? _task = null;
    CancellationTokenSource? _taskCancellationTokenSource = null;

    public Camera(int cameraIndex, ILocator locator)
    {
        CameraIndex = cameraIndex;

        _capture = new(cameraIndex);
        Locator = locator;

        _taskCancellationTokenSource = new();
        _task = Task.Run(TaskFunc);
    }

    public void Close()
    {
        if (!_capture.IsOpened)
        {
            throw new InvalidOperationException($"camera {CameraIndex} is not open");
        }

        Debug.Assert(_task is not null);
        Debug.Assert(_taskCancellationTokenSource is not null);

        _capture.Dispose();

        _taskCancellationTokenSource.Cancel();
        _task.Wait();
        _taskCancellationTokenSource.Dispose();
        _task.Dispose();

        _task = null;
        _taskCancellationTokenSource = null;
    }

    public void Dispose()
    {
        _capture.Dispose();
        _task?.Dispose();

        GC.SuppressFinalize(this);
    }

    public void Open()
    {
        if (_capture.IsOpened)
        {
            throw new InvalidOperationException($"camera {CameraIndex} is already open");
        }

        Debug.Assert(_task is null);
        Debug.Assert(_taskCancellationTokenSource is null);

        _capture = new(CameraIndex);

        _taskCancellationTokenSource = new();
        _task = Task.Run(TaskFunc);
    }

    public double GetProperty(CapProp property)
    {
        return _capture.Get(property);
    }

    public void SetProperty(CapProp property, double value)
    {
        _capture.Set(property, value);
    }

    void TaskFunc()
    {
        Debug.Assert(JpegData is null);
        Debug.Assert(TargetLocation is null);
        Debug.Assert(TargetLocationNotCalibrated is null);

        while (!_taskCancellationTokenSource?.Token.IsCancellationRequested ?? false)
        {
            Mat frame = _capture.QueryFrame();

            if (frame is null)
            {
                continue;
            }

            ILocator.RecognitionResult? recognitionResult = Locator.Locate(frame);

            if (Locator.Mask is null)
            {
                var image = frame.ToImage<Bgr, byte>();
                JpegData = image.ToJpegData();
                image.Dispose();
            }
            else
            {
                var image = Locator.Mask.ToImage<Bgr, byte>();
                JpegData = image.ToJpegData();
                image.Dispose();
            }

            if (recognitionResult is null)
            {
                TargetLocation = null;
                TargetLocationNotCalibrated = null;
            }
            else
            {
                TargetLocation = recognitionResult.CalibratedLocation;
                TargetLocationNotCalibrated = recognitionResult.Location;
            }

            frame.Dispose();
        }

        JpegData = null;
        TargetLocation = null;
        TargetLocationNotCalibrated = null;
    }
}
