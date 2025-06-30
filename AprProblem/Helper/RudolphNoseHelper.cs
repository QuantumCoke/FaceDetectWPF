using System.IO;
using System.Windows;
using AprProblem.Model;
using DlibDotNet;
using OpenCvSharp;

namespace AprProblem.Helper;

/// <summary>
/// 루돌프 사슴 코
/// </summary>
public static class RudolphNoseHelper
{
    /// <summary>
    /// dlib 모델 경로
    /// </summary>
    private static string _modelFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dat", "shape_predictor_68_face_landmarks.dat");

    /// <summary>
    /// 루돌프 코 이미지
    /// </summary>
    private static string _noseFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Img", "RudolphNose.png");

    /// <summary>
    /// 이미지에 있는 얼굴과 코, 입크기를 구하는 메서드
    /// </summary>
    /// <param name="inputFilePath">이미지 경로</param>
    /// <returns>얼굴 정보(코 좌표, 입 크기)</returns>
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

    /// <summary>
    /// 이미지 바이트 정보에 있는 얼굴과 코, 입크기를 구하는 메서드
    /// </summary>
    /// <param name="data">이미지 바이트 정보</param>
    /// <returns>얼굴 정보(코 좌표, 입 크기)</returns>
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

    /// <summary>
    /// 루돌프 코 이미지를 얼굴에 맞게 크기를 변환하는 메서드
    /// </summary>
    /// <param name="mouthWidth">입 크기</param>
    /// <returns></returns>
    public static Mat? GetResizedNoseMat(double mouthWidth)
    {
        var nosePng = Cv2.ImRead(_noseFile, ImreadModes.Unchanged);
        if (nosePng.Empty())
        {
            MessageBox.Show("루돌프 코 이미지가 없습니다");
            return null;
        }

        int targetWidth = (int)(mouthWidth);
        double aspect = (double)nosePng.Height / nosePng.Width;
        int targetHeight = (int)(targetWidth * aspect);

        var resized = new Mat();
        Cv2.Resize(
            src: nosePng,
            dst: resized,
            dsize: new OpenCvSharp.Size(targetWidth, targetHeight),
            fx: 0, fy: 0,
            interpolation: InterpolationFlags.Linear);

        return resized;
    }

    /// <summary>
    /// 이미지에 루돌프 코 이미지를 올리는 메서드
    /// </summary>
    /// <param name="background">이미지 Mat</param>
    /// <param name="overlay">루돌프 코 Mat</param>
    /// <param name="location">코 위치</param>
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