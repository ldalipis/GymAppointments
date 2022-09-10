using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Splat;
using EnergyRoom.Services;
using EnergyRoom.Services.Routing;
using Plugin.FacebookClient;
using Plugin.Media;
using System.IO;
using EnergyRoom.Resources;
using Plugin.Media.Abstractions;
using System.Collections.Generic;
using System;

namespace EnergyRoom.ViewModels.Profile
{
    [Preserve(AllMembers = true)]
    public class ProfileViewModel : BaseViewModel
    {
        private IRoutingService _navigationService;
        private MediaFile _mediaFile;

        private static ProfileViewModel profileViewModel;
        public static ProfileViewModel ProfileInstance => profileViewModel ??= new ProfileViewModel();

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private string newPhone;
        public string NewPhone
        {
            get => newPhone;
            set
            {
                newPhone = value;
                OnPropertyChanged("NewPhone");
            }
        }

        private string newEmail;
        public string NewEmail
        {
            get => newEmail;
            set
            {
                newEmail = value;
                OnPropertyChanged("NewEmail");
            }
        }

        private byte[] userImage;
        public byte[] UserImage
        {
            get => userImage;
            set
            {
                userImage = value;
                OnPropertyChanged("UserImage");
            }
        }

        private bool displayGetPhotoPopup;
        public bool DisplayGetPhotoPopup
        {
            get => displayGetPhotoPopup;
            set
            {
                displayGetPhotoPopup = value;
                OnPropertyChanged("DisplayGetPhotoPopup");
            }
        }

        private bool displayChangePhonePopup;
        public bool DisplayChangePhonePopup
        {
            get => displayChangePhonePopup;
            set
            {
                displayChangePhonePopup = value;
                OnPropertyChanged("DisplayChangePhonePopup");
            }
        }

        private bool displayChangeEmailPopup;
        public bool DisplayChangeEmailPopup
        {
            get => displayChangeEmailPopup;
            set
            {
                displayChangeEmailPopup = value;
                OnPropertyChanged("DisplayChangeEmailPopup");
            }
        }

        #region Constructor

        public ProfileViewModel()
        {
            ExecuteLogoutCommand = new Command(ExecuteLogout);
            ExecuteGetPhotoCommand = new Command(GetPhoto);
            ExecuteGetUserDataCommand = new Command(GetUserData);
            GetPhotoFromDeviceCommand = new Command(GetPhotoFromDevice);
            GetPhotoFromCameraCommand = new Command(GetPhotoFromCamera);
            ChangePhoneCommand = new Command(ChangePhone);
            ChangeEmailCommand = new Command(RequestChangeEmail);
            EditCommand = new Command(EditButtonClicked);
            PhoneCommand = new Command(PhoneClicked);
            EmailCommand = new Command(EmailClicked);
            //NotificationCommand = new Command(this.NotificationOptionClicked);

            Title = "Profile";
            _navigationService = Locator.Current.GetService<IRoutingService>();

            MessagingCenter.Subscribe(this, "acceptchangeemail", (object obj, string token) =>
            {
                AcceptChangeEmail(token);
            });
        }

        #endregion

        #region Command

        public Command ExecuteLogoutCommand { get; set; }
        public Command ExecuteGetUserDataCommand { get; set; }
        public Command ExecuteGetPhotoCommand { get; set; }
        public Command GetPhotoFromDeviceCommand { get; set; }
        public Command GetPhotoFromCameraCommand { get; set; }
        public Command ChangePhoneCommand { get; set; }
        public Command ChangeEmailCommand { get; set; }
        public Command EditCommand { get; set; }
        public Command PhoneCommand { get; set; }
        public Command EmailCommand { get; set; }
        public Command NotificationCommand { get; set; }

        #endregion

        #region Methods

        private void GetUserData()
        {
            Name = Xamarin.Essentials.SecureStorage.GetAsync("userName").Result;
            NewPhone = Xamarin.Essentials.SecureStorage.GetAsync("userPhone").Result;
            NewEmail = Xamarin.Essentials.SecureStorage.GetAsync("userEmail").Result;
        }

        private void ExecuteLogout()
        {
            if (CrossFacebookClient.Current.IsLoggedIn)
            {
                CrossFacebookClient.Current.Logout();
            }

            Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
            Xamarin.Essentials.SecureStorage.SetAsync("userId", "0");
            Xamarin.Essentials.SecureStorage.SetAsync("userIsAdmin", "0");
            Xamarin.Essentials.SecureStorage.SetAsync("fullName", "");
            Xamarin.Essentials.SecureStorage.SetAsync("userName", "");
            Xamarin.Essentials.SecureStorage.SetAsync("userPhone", "");
            Xamarin.Essentials.SecureStorage.SetAsync("userEmail", "");

            _navigationService.NavigateTo("///login");
        }

        // Get photo from DB
        private void GetPhoto()
        {
            UserImage = Task.Run(async () => await MySQLDataStore.Instance.GetUserPhoto(App._userId)).Result;
        }

        private void EditButtonClicked(object obj)
        {
            DisplayGetPhotoPopup = true;
        }

        private async void GetPhotoFromDevice()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("Photos Not Supported", "Permission not granted to photos.", "OK");
                return;
            }
            _mediaFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Small,
            });


            if (_mediaFile == null)
                return;

            DoUploadPhoto();
        }

        private async void GetPhotoFromCamera()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Images",
                SaveToAlbum = false,
                CompressionQuality = 75,
                CustomPhotoSize = 50,
                PhotoSize = PhotoSize.Small,
                MaxWidthHeight = 2000,
                DefaultCamera = CameraDevice.Front
            });

            if (_mediaFile == null)
                return;

            DoUploadPhoto();
        }

        private async void DoUploadPhoto()
        {
            if (_mediaFile == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "There was an error when trying to get your image.", "OK");
                return;
            }
            else
            {
                UploadImage(_mediaFile.GetStream());
            }
        }

        private async void UploadImage(Stream stream)
        {
            byte[] ImageData = new byte[stream.Length];
            int streamLength = (int)stream.Length;
            stream.Read(ImageData, 0, System.Convert.ToInt32(stream.Length));
            stream.Close();
            var model = MySQLDataStore.Instance;
            var uploadCompleted = Task.Run(async () => await model.UploadPhoto(App._userId, ImageData, streamLength)).Result;

            if (uploadCompleted)
            {
                UserImage = ImageData;
                await Application.Current.MainPage.DisplayAlert(AppResources.UploadedText, AppResources.UploadSuccessfulText, "OK");
            }
        }
        private async void PhoneClicked(object obj)
        {
            Application.Current.Resources.TryGetValue("Gray-100", out var retVal);
            (obj as Grid).BackgroundColor = (Color)retVal;
            await Task.Delay(100);
            (obj as Grid).BackgroundColor = Color.Transparent;
            DisplayChangePhonePopup = true;
        }

        private async void EmailClicked(object obj)
        {
            Application.Current.Resources.TryGetValue("Gray-100", out var retVal);
            (obj as Grid).BackgroundColor = (Color)retVal;
            await Task.Delay(100);
            (obj as Grid).BackgroundColor = Color.Transparent;
            DisplayChangeEmailPopup = true;
        }

        //private async void NotificationOptionClicked(object obj)
        //{
        //    Application.Current.Resources.TryGetValue("Gray-100", out var retVal);
        //    (obj as Grid).BackgroundColor = (Color)retVal;
        //    await Task.Delay(100);
        //    (obj as Grid).BackgroundColor = Color.Transparent;
        //}

        private async void ChangePhone(object obj)
        {
            var model = MySQLDataStore.Instance;
            var changeSuccessful = Task.Run(async () => await model.ChangePhone(App._userId, NewPhone)).Result;

            if (changeSuccessful)
            {
                await Application.Current.MainPage.DisplayAlert(AppResources.NotificeText, AppResources.PhoneChangeSuccessfulText, "OK");
                await Xamarin.Essentials.SecureStorage.SetAsync("userPhone", App._phone);
            }
            else
            {
                NewPhone = Xamarin.Essentials.SecureStorage.GetAsync("userPhone").Result;
            }
        }

        private async void RequestChangeEmail(object obj)
        {
            string existingEmail = Xamarin.Essentials.SecureStorage.GetAsync("userEmail").Result;
            var model = MySQLDataStore.Instance;
            var emailExists = Task.Run(async () => await model.CheckExistingEmail(NewEmail)).Result;

            if (emailExists)
            {
                NewEmail = existingEmail;
                await Application.Current.MainPage.DisplayAlert(AppResources.EmailInUseTitle, AppResources.EmailInUseDescr, "OK");
            }
            else
            {
                string token = Helpers.Encryption.Sha256Hash(existingEmail+NewEmail);

                Helpers.EmailHelper.SendEmailChangeRequest(token, NewEmail);
                _ = Task.Run(async () => await model.RequestChangeEmail(App._userId, existingEmail, NewEmail, token)).Result;
                await Application.Current.MainPage.DisplayAlert(AppResources.ChangeEmailRequestSentTitle, AppResources.ChangeEmailRequestSentDescr, "OK");
            }
        }

        private async void AcceptChangeEmail(string token)
        {
            var model = MySQLDataStore.Instance;
            _ = Task.Run(async () => await model.ChangeEmail(App._userId, token)).Result;
            await Application.Current.MainPage.DisplayAlert(AppResources.ChangeEmailAcceptedTitle, AppResources.ChangeEmailAcceptedDescr, "OK");
            ExecuteLogout();
        }

        #endregion
    }
}
