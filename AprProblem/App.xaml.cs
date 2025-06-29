using System.Windows;
using AprProblem.Helper;
using AprProblem.View;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace AprProblem;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{  
    AppBootstrapper bootstrapper = new();

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
