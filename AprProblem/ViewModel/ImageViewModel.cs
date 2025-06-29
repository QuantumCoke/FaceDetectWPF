using System.IO;
using System.Windows.Media.Imaging;
using AprProblem.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp;

namespace AprProblem.ViewModel;

public partial class ImageViewModel : ObservableObject
{
    [ObservableProperty]
    private BitmapImage? _loadedImage;

    [RelayCommand]
    private void ImageClicked()
    {
        try
        {
            var dlg = new OpenFileDialog
            {
                Title = "이미지 파일 선택",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (dlg.ShowDialog() != true)
                return;

            var faces = FrontalRudolphHelper.DetectFaceFromImg(dlg.FileName);

            var frame = Cv2.ImRead(dlg.FileName);

            for (int i = 0; i < faces.Count; i++)
            {
                var rudolghNose = FrontalRudolphHelper.GetResizedNoseMat(faces[i].mouseLength);
                FrontalRudolphHelper.OverlayWithAlpha(frame, rudolghNose, faces[i].nosePosition);
            }

            LoadedImage = MatToBitmapImage(frame);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex}");
        }
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
