using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.UI.Windowing;
using PNMaterialsClient.Services;
using PNMaterialsClient.ViewModels;
using Serilog;
using Serilog.Extensions.Logging;
using Uno.Resizetizer;

namespace PNMaterialsClient;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    private static ILogger<App> _logger = NullLogger<App>.Instance;

    private const string DefaultApiAddress = "https://localhost:7073/";

    private static (Uri Address, string? Problem, Exception? Error) ResolveApiBaseAddress()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(path))
            return (new Uri(DefaultApiAddress), "Файл appsettings.json не найден", null);

        try
        {
            using var stream = File.OpenRead(path);
            using var document = JsonDocument.Parse(stream);

            if (!document.RootElement.TryGetProperty("ApiBaseAddress", out var value)
                || value.GetString() is not { Length: > 0 } text)
                return (new Uri(DefaultApiAddress), "В appsettings.json не задан ApiBaseAddress", null);

            if (!Uri.TryCreate(text, UriKind.Absolute, out var address))
                return (new Uri(DefaultApiAddress), $"В appsettings.json некорректный ApiBaseAddress «{text}»", null);

            return (address, null, null);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException)
        {
            return (new Uri(DefaultApiAddress), "Не удалось прочитать appsettings.json", ex);
        }
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

        if (MainWindow.AppWindow.Presenter is OverlappedPresenter presenter)
            presenter.Maximize();

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

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        _logger.LogError(e.Exception, "Не удалось открыть страницу {PageType}", e.SourcePageType.FullName);
        e.Handled = true;
    }

    private static Serilog.ILogger CreateFileLogger()
    {
        var logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PNMaterials",
            "logs");

        return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                Path.Combine(logDirectory, "client-.log"),
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 30,
                shared: true)
            .CreateLogger();
    }

    public static void InitializeServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConsole();
            builder.AddProvider(new SerilogLoggerProvider(CreateFileLogger(), dispose: true));
#if DEBUG
            builder.SetMinimumLevel(LogLevel.Debug);
#else
            builder.SetMinimumLevel(LogLevel.Information);
#endif
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);
            builder.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
        });

        var (apiBaseAddress, addressProblem, addressError) = ResolveApiBaseAddress();

        services.AddHttpClient<ApiClient>(client => client.BaseAddress = apiBaseAddress);

        services.AddTransient<MaterialsViewModel>();
        services.AddTransient<MaterialGroupsViewModel>();
        services.AddTransient<MaterialCardViewModel>();
        services.AddTransient<MaterialGroupCardViewModel>();
        services.AddTransient<PurchaseRequestCardViewModel>();
        services.AddTransient<ReportViewModel>();

        services.AddSingleton<MessagesService>();

        Services = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory =
            Services.GetRequiredService<ILoggerFactory>();

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif

        _logger = Services.GetRequiredService<ILogger<App>>();

        _logger.LogInformation("Приложение запущено. Версия {Version}, адрес API {ApiAddress}",
            typeof(App).Assembly.GetName().Version?.ToString(), apiBaseAddress.ToString());

        if (addressProblem is not null)
            _logger.LogWarning(addressError, "{Problem}. Используется адрес по умолчанию {ApiAddress}",
                addressProblem, apiBaseAddress.ToString());
    }

    public static void InitializeExceptionHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private static void OnDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        _logger.LogCritical(e.ExceptionObject as Exception, "Необработанное исключение, приложение будет закрыто");
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Необработанное исключение в задаче без await");
        e.SetObserved();
    }
}
