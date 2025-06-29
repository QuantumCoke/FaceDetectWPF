using System.IO;
using System.Windows.Media.Imaging;
using AprProblem.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace AprProblem.ViewModel;

public partial class CameraViewModel : ObservableObject
{
    private VideoCapture? _capture;
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private BitmapSource? _currentFrame;


    public CameraViewModel()
    {
        _capture = new VideoCapture(0);
        if (!_capture.IsOpened())
        {
            _capture = null;
            throw new InvalidOperationException("카메라를 열 수 없습니다.");
        }

        _cts = new CancellationTokenSource();
        CaptureLoop(_cts.Token);
    }

    private void CaptureLoop(CancellationToken token)
    {
        var mat = new Mat();
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (!_capture!.Read(mat) || mat.Empty())
                    break;

                mat.ThrowIfDisposed();
                mat = mat.CvtColor(ColorConversionCodes.BGR2BGRA);
                Cv2.ImEncode(".png", mat, out byte[] imgBytes);

                var faces = FrontalRudolphHelper.DetectFaceFromByte(imgBytes);

                for (int i = 0; i < faces.Count; i++)
                {
                    var rudolghNose = FrontalRudolphHelper.GetResizedNoseMat(faces[i].mouseLength);
                    FrontalRudolphHelper.OverlayWithAlpha(mat, rudolghNose, faces[i].nosePosition);
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    CurrentFrame = MatToBitmapImage(mat);
                });
            }
        }
        finally
        {
            mat.Dispose();
        }
    }

    [RelayCommand]
    private void Stop()
    {
        _cts?.Token.ThrowIfCancellationRequested();
    }

    private BitmapImage MatToBitmapImage(Mat mat)
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
