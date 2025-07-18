using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PressureMeasurementAppAvaloniaView.Interfaces;
using PressureMeasurementAppAvaloniaView.Services;
using PressureMeasurementAppAvaloniaView.ViewModels;
using PressureMeasurementAppAvaloniaView.Views;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PressureMeasurementAppAvaloniaView;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;
    private string _apiBaseUrl;
    private string _kafkaServer;
    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        _apiBaseUrl = Resources["ApiBaseUrl"] as string
            ?? throw new InvalidOperationException("ApiBaseUrl resource not found");
        _kafkaServer = Resources["KafkaServer"] as string
            ?? throw new InvalidOperationException("KafkaServer resource not found");
    }
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();
        services.AddSingleton<IApiService>(provider =>
            new ApiService(
                provider.GetRequiredService<HttpClient>(),
                _apiBaseUrl
            ));

        services.AddSingleton<IKafkaConsumerService>(provider =>
            new KafkaConsumerService(
                _kafkaServer,
                "pressure-measurement-group"
            ));

        services.AddTransient(provider =>
            new MainViewModel(
                provider.GetRequiredService<IApiService>(),
                provider.GetRequiredService<IKafkaConsumerService>()
            ));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            var vm = _serviceProvider.GetRequiredService<MainViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };

            _ = InitializeViewModelAsync(vm);
        }

        base.OnFrameworkInitializationCompleted();
    }
    private async Task InitializeViewModelAsync(MainViewModel vm)
    {
        try
        {
             await vm.InitializeAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Initialization failed: {ex}");
        }
    }
    private void DisableAvaloniaDataAnnotationValidation()
    {
        var pluginsToRemove = BindingPlugins.DataValidators
            .OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        foreach (var plugin in pluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}