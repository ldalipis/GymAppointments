using EnergyRoom.ViewModels.Profile;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace EnergyRoom.Views.Profile
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage
    {
        public ProfilePage()
        {
            InitializeComponent();

            BindingContext = ProfileViewModel.ProfileInstance;
        }
    }
}