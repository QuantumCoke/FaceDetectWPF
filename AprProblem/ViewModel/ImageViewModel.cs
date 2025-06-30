using System.IO;
using System.Windows.Media.Imaging;
using AprProblem.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenCvSharp;

namespace AprProblem.ViewModel;

public partial class ImageViewModel : baseViewModel
{
    /// <summary>
    /// 로드된 이미지 정보
    /// </summary>
    [ObservableProperty]
    private BitmapImage? _loadedImage;

    /// <summary>
    /// 이미지 버튼 클릭
    /// </summary>
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

            var faces = RudolphNoseHelper.DetectFaceFromImg(dlg.FileName);

            var frame = Cv2.ImRead(dlg.FileName);

            for (int i = 0; i < faces.Count; i++)
            {
                var rudolghNose = RudolphNoseHelper.GetResizedNoseMat(faces[i].mouseLength);
                RudolphNoseHelper.OverlayWithAlpha(frame, rudolghNose, faces[i].nosePosition);
            }

            LoadedImage = MatToBitmapImage(frame);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex}");
        }
    }
}
