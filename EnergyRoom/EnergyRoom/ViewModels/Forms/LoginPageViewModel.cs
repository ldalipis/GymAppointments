using EnergyRoom.Models;
using EnergyRoom.Resources;
using EnergyRoom.Services;
using EnergyRoom.Services.Routing;
using Splat;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Plugin.FacebookClient;
using System;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using EnergyRoom.ViewModels.Profile;

namespace EnergyRoom.ViewModels.Forms
{
    /// <summary>
    /// ViewModel for login page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class LoginPageViewModel : LoginViewModel
    {
        private static LoginPageViewModel loginPageViewModel;
        public static LoginPageViewModel LoginPageViewModelInstance => loginPageViewModel ??= new LoginPageViewModel();

        private IRoutingService _navigationService;

        string[] permisions = new string[] { "email", "public_profile" };
        public FacebookProfile Profile { get; set; }
        public bool IsLoggedIn { get; set; }

        #region Constructor

        public LoginPageViewModel()
        {
            Profile = new FacebookProfile();

            _navigationService = Locator.Current.GetService<IRoutingService>();

            LoginCommand = new Command(LoginClicked);
            LoginWithFacebookCommand = new Command(async () => await LoginWithFacebookClicked());
            OnShareDataCommand = new Command(async () => await ShareDataAsync());
            OnLoadDataCommand = new Command(async () => await LoadData());
            SignUpCommand = new Command(SignUpClicked);
            ForgotPasswordCommand = new Command(ForgotPasswordClicked);
            SetUserDataCommand = new Command((x) => SetUserData((Users)x));
            SendPasswordResetLinkCommand = new Command(SendPasswordResetLink);
        }

        #endregion

        #region property
        
        private bool displayForgotPasswordPopup;
        public bool DisplayForgotPasswordPopup
        {
            get => displayForgotPasswordPopup;
            set
            {
                displayForgotPasswordPopup = value;
                OnPropertyChanged();
            }
        }

        private string password;
        public string Password
        {
            get => password;
            set
            {
                if (password == value)
                {
                    return;
                }

                password = value;
                OnPropertyChanged();
            }
        }

        private string cred;
        public string Cred
        {
            get => cred;
            set
            {
                if (cred == value)
                {
                    return;
                }

                cred = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        public Command LoginCommand { get; set; }
        public Command LoginWithFacebookCommand { get; set; }
        public Command SignUpCommand { get; set; }
        public Command ForgotPasswordCommand { get; set; }
        public Command OnShareDataCommand { get; set; }
        public Command OnLoadDataCommand { get; set; }
        public Command SetUserDataCommand { get; set; }
        public Command SendPasswordResetLinkCommand { get; set; }

        #endregion

        #region methods

        public async Task LoginWithFacebookClicked()
        {
            if (CrossFacebookClient.Current.IsLoggedIn)
            {
                CrossFacebookClient.Current.Logout();
            }

            FacebookResponse<bool> response = await CrossFacebookClient.Current.LoginAsync(permisions);
            switch (response.Status)
            {
                case FacebookActionStatus.Completed:
                    IsLoggedIn = true;
                    OnLoadDataCommand.Execute(null);
                    break;
                case FacebookActionStatus.Canceled:

                    break;
                case FacebookActionStatus.Unauthorized:
                    await Application.Current.MainPage.DisplayAlert("Unauthorized", response.Message, "Ok");
                    break;
                case FacebookActionStatus.Error:
                    await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Ok");
                    break;
            }

        }

        async Task ShareDataAsync()
        {
            FacebookShareLinkContent linkContent = new FacebookShareLinkContent("Awesome team of developers, making the world a better place one project or plugin at the time!", new Uri("http://www.github.com/crossgeeks"));
            var ret = await CrossFacebookClient.Current.ShareAsync(linkContent);
        }

        public async Task LoadData()
        {
            var jsonData = await CrossFacebookClient.Current.RequestUserDataAsync
            (
                  new string[] { "id", "name", "email", "picture" }, new string[] { }
            );

            var data = JObject.Parse(jsonData.Data);
            Profile = new FacebookProfile()
            {
                FullName = data["name"].ToString(),
                PictureSrc = new UriImageSource { Uri = new Uri($"{data["picture"]["data"]["url"]}") },
                Picture = $"{data["picture"]["data"]["url"]}",
                Email = data["email"].ToString()
            };

            UserLoginFBAsync(Profile);
        }

        private void SetUserData(Users result)
        {
            if (result.ID != 0)
            {
                App._userId = result.ID;
                App._userName = !string.IsNullOrWhiteSpace(result.Username) ? result.Username : result.Name;
                App._userIsAdmin = result.UserTypeId == 1;
                App._name = result.Name;
                App._phone = result.Phone;
                App._email = result.Email;

                Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                Xamarin.Essentials.SecureStorage.SetAsync("userId", App._userId.ToString());
                Xamarin.Essentials.SecureStorage.SetAsync("userIsAdmin", App._userIsAdmin.ToString());
                Xamarin.Essentials.SecureStorage.SetAsync("fullName", App._name);
                Xamarin.Essentials.SecureStorage.SetAsync("userName", App._userName);
                Xamarin.Essentials.SecureStorage.SetAsync("userPhone", App._phone);
                Xamarin.Essentials.SecureStorage.SetAsync("userEmail", App._email);

                ProfileViewModel.ProfileInstance.ExecuteGetUserDataCommand.Execute(null);
                ProfileViewModel.ProfileInstance.ExecuteGetPhotoCommand.Execute(null);
                MySQLDataStore.Instance.WeekViewPageViewModel.ExecuteGetUserAppointmentsCommand.Execute(null);
                _navigationService.NavigateTo("///main");
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(AppResources.UserNotFoundTitle, AppResources.UserNotFoundDescr, "Ok");
            }
        }

        private void UserLoginFBAsync(FacebookProfile profile)
        {
            var task = Task.Run(async () => await MySQLDataStore.Instance.UserLoginFBAsync(profile.Email));
            Users result = task.Result;

            if (result.ID != 0)
            {
                SetUserData(result);
            }
            else
            {
                string RandomPassword = RandomString(12);
                var newUserId = Task.Run(async () => await MySQLDataStore.Instance.UserSignUpAsync(profile.FullName, "", profile.Email, profile.FullName, RandomPassword));
                if (newUserId.Result > 0)
                {
                    using var client = new WebClient();
                    byte[] imageBytes = client.DownloadData(profile.Picture);

                    var uploadPhotoTask = Task.Run(async () => await MySQLDataStore.Instance.UploadPhoto((int)newUserId.Result, imageBytes, imageBytes.Length));
                    if (uploadPhotoTask.Result)
                    {
                        Email = profile.Email;
                        Password = RandomPassword;
                        LoginClicked(null);
                    }
                }
            }
        }

        private void LoginClicked(object obj)
        {
            var task = Task.Run(async () => await MySQLDataStore.Instance.UserLoginAsync(Email, Password));
            Users result = task.Result;
            SetUserData(result);
        }

        private void SignUpClicked(object obj)
        {
            _navigationService.NavigateTo("///registration");
        }

        private async void ForgotPasswordClicked(object obj)
        {
            var label = obj as Label;
            label.BackgroundColor = Color.FromHex("#70FFFFFF");
            await Task.Delay(100);
            label.BackgroundColor = Color.Transparent;
            
            DisplayForgotPasswordPopup = true;
        }

        private async void SendPasswordResetLink()
        {
            var model = MySQLDataStore.Instance;
            var findEmail = Task.Run(async () => await model.FindEmail(Cred)).Result;

            if (!string.IsNullOrWhiteSpace(findEmail))
            {
                Helpers.EmailHelper.SendPasswordReset(findEmail);
            }

            await Application.Current.MainPage.DisplayAlert(AppResources.ForgotPasswordText, AppResources.ForgotPasswordRun, "OK");
            Cred = "";
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion
    }
}