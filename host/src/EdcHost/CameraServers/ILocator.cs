using Emgu.CV;

namespace EdcHost.CameraServers;

public interface ILocator
{
    record RecognitionResult
    {
        public Tuple<float, float> CalibratedLocation { get; init; } = null!;
        public Tuple<float, float> Location { get; init; } = null!;
    }

    Mat? Mask { get; }

    RecognitionOptions Options { get; set; }

    /// <summary>
    /// Locates the target in the given frame.
    /// </summary>
    /// <param name="frame">The frame to locate the target in.</param>
    /// <returns>
    /// The location of the target in the frame, or null if the target could
    /// not be located.
    /// </returns>
    RecognitionResult? Locate(Mat frame);
}
