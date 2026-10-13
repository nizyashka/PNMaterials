using System;
using Microsoft.Extensions.Logging;
using Uno.Resizetizer;
using PNMaterialsClient.Services;
using System.Text.Json;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    private static string ResolveApiBaseAddress()
    {
        const string fallback = "https://localhost:7073/";

        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            if (!File.Exists(path))
                return fallback;

            using var stream = File.OpenRead(path);
            using var document = JsonDocument.Parse(stream);

            if (document.RootElement.TryGetProperty("ApiBaseAddress", out var value))
            {
                var address = value.GetString();

                if (!string.IsNullOrWhiteSpace(address))
                    return address;
            }
        }
        catch
        {
            // повреждённый или недоступный файл не должен мешать запуску
        }

        return fallback;
    }

    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window();
#if DEBUG
        MainWindow.UseStudio();
#endif


        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;

            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        MainWindow.SetWindowIcon();
        // Ensure the current window is active
        MainWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    public static void InitializeServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConsole();
#if DEBUG
            builder.SetMinimumLevel(LogLevel.Debug);
#else
        builder.SetMinimumLevel(LogLevel.Information);
#endif
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);
        });

        var apiBaseAddress = new Uri(ResolveApiBaseAddress());

        services.AddHttpClient<ApiClient>(client => client.BaseAddress = apiBaseAddress);

        services.AddTransient<MaterialsViewModel>();
        services.AddTransient<MaterialGroupsViewModel>();
        services.AddTransient<PurchaseRequestsViewModel>();
        services.AddTransient<MaterialCardViewModel>();
        services.AddTransient<MaterialGroupCardViewModel>();
        services.AddTransient<PurchaseRequestCardViewModel>();
        services.AddTransient<ReportViewModel>();

        Services = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory =
            Services.GetRequiredService<ILoggerFactory>();

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
    }
}
