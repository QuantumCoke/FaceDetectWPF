using System.Windows;
using AprProblem.View;
using AprProblem.ViewModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AprProblem;

public class AppBootstrapper
{
    public IServiceProvider? Services { get; private set; }

    public void Run()
    {
        var services = new ServiceCollection();

        RegisterViewModels(services);
        RegisterViews(services);

        Services = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(Services);
    }


    private void RegisterViewModels(IServiceCollection services)
    {
        services.AddSingleton<NavigationViewModel>();
        services.AddSingleton<ImageViewModel>();
        services.AddSingleton<CameraViewModel>();

    }


    private void RegisterViews(IServiceCollection services)
    {
        services.AddView<NavigationView, NavigationViewModel>();

        services.AddView<ImageView, ImageViewModel>();
        services.AddView<CameraView, CameraViewModel>();
    }
}
