using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace AprProblem.ViewModel;

public class baseViewModel : ObservableObject
{
    /// <summary>
    /// Mat 정보를 bitmap으로 바꾸는 메서드
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    protected BitmapImage MatToBitmapImage(Mat mat)
    {
        mat.ThrowIfDisposed();
        mat = mat.CvtColor(ColorConversionCodes.BGR2BGRA);

        Cv2.ImEncode(".png", mat, out byte[] imgBytes);

        var bmp = new BitmapImage();
        using (var ms = new MemoryStream(imgBytes))
        {
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();
        }
        return bmp;
    }
}
