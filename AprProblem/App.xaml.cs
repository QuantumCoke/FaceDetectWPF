using System.Windows;
using AprProblem.View;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace AprProblem;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{  
    AppBootstrapper bootstrapper = new();

    /// <summary>
    /// 앱 시작 셋업
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        bootstrapper.Run();
        var navigation = Ioc.Default.GetRequiredService<NavigationView>();

        var mainWindow = new MainWindow
        {
            Content = navigation
        };
        mainWindow.Show();
    }
}
