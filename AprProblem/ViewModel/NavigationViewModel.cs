using System.Collections.ObjectModel;
using AprProblem.Enum;
using AprProblem.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using MaterialDesignThemes.Wpf;

namespace AprProblem.ViewModel;

public partial class NavigationViewModel : ObservableObject

{
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _itemList;

    [ObservableProperty]
    private object? _selectedItem;

    /// <summary>
    /// 선택된 NavigationItem을 추적하는 프로퍼티
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

    partial void OnSelectedNavigationItemChanged(NavigationItem? value)
    {
        if (value != null)
        {
            UpdateSelectedView(value);
        }
    }

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
