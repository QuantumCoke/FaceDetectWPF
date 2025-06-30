using AprProblem.View;
using AprProblem.ViewModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AprProblem;

public class AppBootstrapper
{
    public IServiceProvider? Services { get; private set; }

    /// <summary>
    /// 서비스 콜렉션을 생성하여 ViewModel과 View를 등록
    /// Ioc 컨테이너를 초기화합니다.
    /// </summary>
    public void Run()
    {
        var services = new ServiceCollection();

        RegisterViewModels(services);
        RegisterViews(services);

        Services = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(Services);
    }

    /// <summary>
    /// ViewModel들을 싱글턴으로 등록합니다.
    /// </summary>
    /// <param name="services"></param>
    private void RegisterViewModels(IServiceCollection services)
    {
        services.AddSingleton<NavigationViewModel>();
        services.AddSingleton<ImageViewModel>();
        services.AddSingleton<CameraViewModel>();
    }

    /// <summary>
    /// 바인딩
    /// </summary>
    /// <param name="services"></param>
    private void RegisterViews(IServiceCollection services)
    {
        services.AddView<NavigationView, NavigationViewModel>();

        services.AddView<ImageView, ImageViewModel>();
        services.AddView<CameraView, CameraViewModel>();
    }
}
