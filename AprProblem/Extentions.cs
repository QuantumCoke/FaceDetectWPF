using AprProblem.ViewModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AprProblem;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddView<TView, TViewModel>(this IServiceCollection services)
        where TView : FrameworkElement, new()
        where TViewModel : class
    {
        services.AddTransient<TView>(sp =>
        {
            var vm = Ioc.Default.GetService<TViewModel>();
            var view = new TView { DataContext = vm };
            return view;
        });
        return services;
    }
}
