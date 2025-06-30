using System.Windows;
using System.Windows.Media.Imaging;
using AprProblem.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;

namespace AprProblem.ViewModel;

public partial class CameraViewModel : baseViewModel
{
    private VideoCapture? _capture;
    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private BitmapSource? _currentFrame;

    /// <summary>
    /// 카메라 정보 정제 메서드
    /// </summary>
    /// <param name="token"></param>
    private void CaptureLoop(CancellationToken token)
    {
        var mat = new Mat();

        while (!token.IsCancellationRequested)
        {
            if (!_capture!.Read(mat) || mat.Empty())
                break;

            mat.ThrowIfDisposed();
            mat = mat.CvtColor(ColorConversionCodes.BGR2BGRA);
            Cv2.ImEncode(".png", mat, out byte[] imgBytes);

            var faces = RudolphNoseHelper.DetectFaceFromByte(imgBytes);

            for (int i = 0; i < faces.Count; i++)
            {
                var rudolghNose = RudolphNoseHelper.GetResizedNoseMat(faces[i].mouseLength);
                RudolphNoseHelper.OverlayWithAlpha(mat, rudolghNose, faces[i].nosePosition);
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                CurrentFrame = MatToBitmapImage(mat);
            });
        }

        mat.Dispose();
    }

    /// <summary>
    /// 재생 커맨드
    /// </summary>
    [RelayCommand]
    private void Start()
    {
        _capture = new VideoCapture(0);
        if (!_capture.IsOpened())
        {
            _capture = null;
            MessageBox.Show("Camera 연결이 없습니다");
            return;
        }
        _cts = new CancellationTokenSource();
        Task.Run(() => CaptureLoop(_cts.Token));
    }

    /// <summary>
    /// 스탑 커맨드
    /// </summary>
    [RelayCommand]
    private void Stop()
    {
        _cts?.Cancel();
    }
}
