using PNMaterialsContracts;

namespace PNMaterialsClient.Views.Dialogs;

public sealed partial class UserProfileContentDialog : ContentDialog
{
    private int _id { get; set; }
    private ProfileDto _userProfile { get; set; }

    public UserProfileContentDialog(int id, ProfileDto profileDto)
    {
        this.InitializeComponent();
        _id = id;
        _userProfile = profileDto;
    }
}
