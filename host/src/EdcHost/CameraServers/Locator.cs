using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace EdcHost.CameraServers;

public class Locator : ILocator
{
    public Mat? Mask { get; private set; } = null;

    public RecognitionOptions Options { get; set; }

    public Locator(RecognitionOptions? options = null)
    {
        Options = options ?? new RecognitionOptions();
    }

    public ILocator.RecognitionResult? Locate(Mat originalFrame)
    {
        using Mat mask = GetMask(originalFrame);

        // Show mask if requested.
        if (Options.ShowMask)
        {
            Mask?.Dispose();
            Mask = mask.Clone();
        }
        else
        {
            Mask?.Dispose();
            Mask = null;
        }

        Tuple<float, float>? location = GetLocation(mask);

        if (location is null)
        {
            return null;
        }

        Tuple<float, float> calibratedLocation = GetCalibratedLocation(location);

        return new ILocator.RecognitionResult
        {
            CalibratedLocation = calibratedLocation,
            Location = location,
        };
    }

    Tuple<float, float> GetCalibratedLocation(Tuple<float, float> pixelLocation)
    {
        using Mat transform = CvInvoke.GetPerspectiveTransform(
            src: new PointF[] {
                new(Options.TopLeftX, Options.TopLeftY),
                new(Options.TopRightX, Options.TopRightY),
                new(Options.BottomRightX, Options.BottomRightY),
                new(Options.BottomLeftX, Options.BottomLeftY),
            },
            dest: new PointF[] {
                new(0, 0),
                new(1, 0),
                new(1, 1),
                new(0, 1),
            }
        );

        PointF[] transformed = CvInvoke.PerspectiveTransform(
            src: new PointF[] { new(pixelLocation.Item1, pixelLocation.Item2) },
            mat: transform
        );

        Debug.Assert(transformed.Length == 1);

        return new Tuple<float, float>(
            transformed[0].X,
            transformed[0].Y
        );
    }

    Mat GetMask(Mat frame)
    {
        Mat mask = frame.Clone();

        // Convert to HSV color space.
        CvInvoke.CvtColor(
            src: mask,
            dst: mask,
            code: ColorConversion.Bgr2Hsv
        );

        // Binarize the image.
        CvInvoke.InRange(
            src: mask,
            lower: new ScalarArray(new MCvScalar(
                Options.HueCenter - Options.HueRange / 2,
                Options.SaturationCenter - Options.SaturationRange / 2,
                Options.ValueCenter - Options.ValueRange / 2
            )),
            upper: new ScalarArray(new MCvScalar(
                Options.HueCenter + Options.HueRange / 2,
                Options.SaturationCenter + Options.SaturationRange / 2,
                Options.ValueCenter + Options.ValueRange / 2
            )),
            dst: mask
        );

        return mask;
    }

    Tuple<float, float>? GetLocation(Mat mask)
    {
        // Find contours.
        using VectorOfVectorOfPoint contours = new();
        CvInvoke.FindContours(
            image: mask,
            contours: contours,
            hierarchy: null,
            mode: RetrType.List,
            method: ChainApproxMethod.ChainApproxSimple
        );

        // Find the largest contour.
        int? largestContourIndex = null;
        double largestContourArea = 0;
        for (int i = 0; i < contours.Size; i++)
        {
            double area = CvInvoke.ContourArea(contours[i]);
            if (area > largestContourArea)
            {
                largestContourIndex = i;
                largestContourArea = area;
            }
        }

        // Return null if no contours were found.
        if (largestContourIndex is null)
        {
            return null;
        }

        // Return null if no area is large enough.
        if (largestContourArea / (mask.Height * mask.Width) < Options.MinArea)
        {
            return null;
        }

        // Find the center of the largest contour.
        using VectorOfPoint largestContour = contours[largestContourIndex.Value];
        using Moments moments = CvInvoke.Moments(largestContour);
        float centerX = (float)(moments.M10 / moments.M00);
        float centerY = (float)(moments.M01 / moments.M00);

        Tuple<float, float> location = new(
            centerX,
            centerY
        );

        return location;
    }
}
