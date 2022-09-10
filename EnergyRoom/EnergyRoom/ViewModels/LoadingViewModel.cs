using EnergyRoom.Models;
using EnergyRoom.Resources;
using EnergyRoom.Services;
using EnergyRoom.Services.Identity;
using EnergyRoom.Services.Routing;
using EnergyRoom.ViewModels.Forms;
using EnergyRoom.ViewModels.Profile;
using Splat;
using System.Threading.Tasks;

namespace EnergyRoom.ViewModels
{
    class LoadingViewModel : BaseViewModel
    {
        private readonly IRoutingService routingService;
        private readonly IIdentityService identityService;

        public LoadingViewModel(IRoutingService routingService = null, IIdentityService identityService = null)
        {
            this.routingService = routingService ?? Locator.Current.GetService<IRoutingService>();
            this.identityService = identityService ?? Locator.Current.GetService<IIdentityService>();
        }

        // Called by the views OnAppearing method
        public async void Init()
        {
            var isAuthenticated = await identityService.VerifyRegistration();
            if (isAuthenticated)
            {
                string email = Xamarin.Essentials.SecureStorage.GetAsync("userEmail").Result;
                var task = Task.Run(async () => await MySQLDataStore.Instance.UserLoginFBAsync(email));
                Users user = task.Result;

                if (user.ID == 0)
                {
                    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(AppResources.NotificeText, AppResources.DisconnectedText, "Ok");
                    ProfileViewModel.ProfileInstance.ExecuteLogoutCommand.Execute(null);
                    await routingService.NavigateTo("///login");
                }
                else
                {
                    LoginPageViewModel.LoginPageViewModelInstance.SetUserDataCommand.Execute(user);
                    await routingService.NavigateTo("///main");
                }
            }
            else
            {
                await routingService.NavigateTo("///login");
            }
        }
    }
}
