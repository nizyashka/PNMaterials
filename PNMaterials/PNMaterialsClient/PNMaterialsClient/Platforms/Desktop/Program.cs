using Uno.UI.Hosting;

namespace PNMaterialsClient;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.InitializeServices();

        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseLinuxFrameBuffer()
            .UseMacOS()
            .UseWin32()
            .Build();

        host.Run();
    }
}
