using OpenCvSharp;

namespace AprProblem.Model;

/// <summary>
/// 얼굴 정보
/// </summary>
public class FaceModel
{
    /// <summary>
    /// 입 좌우 길이
    /// </summary>
    public double mouseLength;

    /// <summary>
    /// 코 위치
    /// </summary>
    public Point2d nosePosition;
}
