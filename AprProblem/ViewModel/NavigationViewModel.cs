using System.Collections.ObjectModel;
using AprProblem.Enum;
using AprProblem.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using MaterialDesignThemes.Wpf;

namespace AprProblem.ViewModel;

public partial class NavigationViewModel : ObservableObject

{
    /// <summary>
    /// 메뉴
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _itemList;

    /// <summary>
    /// 선택된 뷰
    /// </summary>
    [ObservableProperty]
    private object? _selectedItem;

    /// <summary>
    /// 선택된 NavigationItem
    /// </summary>
    [ObservableProperty]
    private NavigationItem? _selectedNavigationItem;

    public NavigationViewModel()
    {
        ItemList = new ObservableCollection<NavigationItem>
        {
            new NavigationItem
            {
                Title = "Image",
                Icon = PackIconKind.Image,
                ViewType = ViewType.Image
            },
            new NavigationItem
            {
                Title = "Camera",
                Icon = PackIconKind.Camera,
                ViewType = ViewType.Camera
            },
        };

        SelectedNavigationItem = ItemList.FirstOrDefault();
    }

    /// <summary>
    /// SelectedNavigationItem이 바뀔때 마다 호출되는 메서드
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedNavigationItemChanged(NavigationItem? value)
    {
        if (value != null)
        {
            UpdateSelectedView(value);
        }
    }

    /// <summary>
    /// type에 맞는 view 리턴 메서드
    /// </summary>
    /// <param name="item"></param>
    private void UpdateSelectedView(NavigationItem item)
    {
        SelectedItem = item.ViewType switch
        {
            ViewType.Image => Ioc.Default.GetRequiredService<ImageView>(),
            ViewType.Camera => Ioc.Default.GetRequiredService<CameraView>(),
            _ => SelectedItem
        };
    }
}

/// <summary>
/// 네이게이션 정보
/// </summary>
public class NavigationItem : ObservableObject
{
    public string? Title { get; set; }

    public ViewType ViewType { get; set; }

    public PackIconKind Icon { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
