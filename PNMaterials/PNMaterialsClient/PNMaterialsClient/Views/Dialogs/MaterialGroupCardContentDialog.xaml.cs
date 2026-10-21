using System.ComponentModel;
using PNMaterialsClient.ViewModels;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class MaterialGroupCardContentDialog : ContentDialog
{
    private readonly int _id;

    public MaterialGroupCardViewModel ViewModel { get; }
    public ICommand ExitCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    public bool IsDeleted { get; private set; }

    public MaterialGroupCardContentDialog(int id)
    {
        this.InitializeComponent();
        _id = id;
        ViewModel = App.Services.GetRequiredService<MaterialGroupCardViewModel>();
        ExitCommand = new RelayCommand(Hide);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => ViewModel.Id != 0);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        Opened += OnOpened;
    }

    private async void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        await ViewModel.LoadAsync(_id);
    }

    private async Task DeleteAsync()
    {
        if (await ViewModel.DeleteAsync())
        {
            IsDeleted = true;
            Hide();
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MaterialGroupCardViewModel.Id))
            DeleteCommand.NotifyCanExecuteChanged();
    }
}
