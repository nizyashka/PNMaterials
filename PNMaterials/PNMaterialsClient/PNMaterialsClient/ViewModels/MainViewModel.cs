using PNMaterialsClient.Services;

namespace PNMaterialsClient.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(ILogger<MainViewModel> logger, AuthService authService) : base(logger, authService)
    {
        
    }
}
