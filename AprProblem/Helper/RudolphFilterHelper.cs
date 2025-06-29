using System.IO;
using AprProblem.Model;
using DlibDotNet;
using OpenCvSharp;

namespace AprProblem.Helper;

public static class FrontalRudolphHelper
{
    private static string _modelFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dat", "shape_predictor_68_face_landmarks.dat");
    private static string _noseFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Img", "RudolphNose.png");

    public static List<FaceModel> DetectFaceFromImg(string inputFilePath)
    {
        List<FaceModel> nosesPoint = new(); 
        using (var fd = Dlib.GetFrontalFaceDetector())
        using (var sp = ShapePredictor.Deserialize(_modelFile))
        {
            var img = Dlib.LoadImage<RgbPixel>(inputFilePath);

            var faces = fd.Operator(img);
            foreach (var face in faces)
            {
                var shape = sp.Detect(img, face);
                var nosePt = shape.GetPart(30);

                var rightMouthCorner = shape.GetPart(54);
                var leftMouthCorner = shape.GetPart(48);

                double x1 = rightMouthCorner.X;
                double y1 = rightMouthCorner.Y;
                double x2 = leftMouthCorner.X;
                double y2 = leftMouthCorner.Y;

                double dx = x1 - x2;
                double dy = y1 - y2;
                double mouthWidth = Math.Sqrt(dx * dx + dy * dy);
                int x = nosePt.X;
                int y = nosePt.Y;
                nosesPoint.Add(new FaceModel() { 
                    nosePosition = new OpenCvSharp.Point2d(x, y),
                    mouseLength = mouthWidth,
                });
            }      
        }
        return nosesPoint;
    }

    public static List<FaceModel> DetectFaceFromByte(byte[] data)
    {
        List<FaceModel> nosesPoint = new();
        using (var fd = Dlib.GetFrontalFaceDetector())
        using (var sp = ShapePredictor.Deserialize(_modelFile))
        {
            var img = Dlib.LoadPng<RgbPixel>(data);

            var faces = fd.Operator(img);
            foreach (var face in faces)
            {
                var shape = sp.Detect(img, face);
                var nosePt = shape.GetPart(30);

                var rightMouthCorner = shape.GetPart(54);
                var leftMouthCorner = shape.GetPart(48);

                double x1 = rightMouthCorner.X;
                double y1 = rightMouthCorner.Y;
                double x2 = leftMouthCorner.X;
                double y2 = leftMouthCorner.Y;

                double dx = x1 - x2;
                double dy = y1 - y2;
                double mouthWidth = Math.Sqrt(dx * dx + dy * dy);
                int x = nosePt.X;
                int y = nosePt.Y;
                nosesPoint.Add(new FaceModel()
                {
                    nosePosition = new OpenCvSharp.Point2d(x, y),
                    mouseLength = mouthWidth,
                });
            }
        }
        return nosesPoint;
    }

    public static Mat GetResizedNoseMat(double mouthWidth)
    {
        var nosePng = Cv2.ImRead(_noseFile, ImreadModes.Unchanged);
        if (nosePng.Empty())
            throw new FileNotFoundException($"Nose image not found or invalid: {_noseFile}");

        int targetWidth = (int)(mouthWidth);
        double aspect = (double)nosePng.Height / nosePng.Width;
        int targetHeight = (int)(targetWidth * aspect);

        var resized = new Mat();
        Cv2.Resize(
            src: nosePng,
            dst: resized,
            dsize: new Size(targetWidth, targetHeight),
            fx: 0, fy: 0,
            interpolation: InterpolationFlags.Linear);

        return resized;
    }

    public static void OverlayWithAlpha(Mat background, Mat overlay, Point2d location)
    {
        int startX = (int)(location.X - overlay.Width / 2.0);
        int startY = (int)(location.Y - overlay.Height / 2.0);

        for (int oy = 0; oy < overlay.Rows; oy++)
        {
            int by = startY + oy;
            if (by < 0 || by >= background.Rows) continue;

            for (int ox = 0; ox < overlay.Cols; ox++)
            {
                int bx = startX + ox;
                if (bx < 0 || bx >= background.Cols) continue;

                Vec4b fgPx = overlay.At<Vec4b>(oy, ox);
                float alpha = fgPx[3] / 255f;
                if (alpha <= 0) continue;

                Vec3b bgPx = background.At<Vec3b>(by, bx);
                for (int c = 0; c < 3; c++)
                    bgPx[c] = (byte)(fgPx[c] * alpha + bgPx[c] * (1 - alpha));

                background.Set<Vec3b>(by, bx, bgPx);
            }
        }
    }
}