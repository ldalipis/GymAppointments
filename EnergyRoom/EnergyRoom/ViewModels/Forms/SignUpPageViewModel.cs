using EnergyRoom.Models;
using EnergyRoom.Resources;
using EnergyRoom.Services;
using EnergyRoom.Services.Routing;
using Splat;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EnergyRoom.ViewModels.Forms
{
    /// <summary>
    /// ViewModel for sign-up page.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class SignUpPageViewModel : LoginViewModel
    {
        private IRoutingService _navigationService;

        #region Fields

        private string name;

        private string username;
        
        private string phone;

        private string password;

        private string confirmPassword;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="SignUpPageViewModel" /> class.
        /// </summary>
        public SignUpPageViewModel()
        {
            _navigationService = Locator.Current.GetService<IRoutingService>();
            
            LoginCommand = new Command(LoginClicked);
            SignUpCommand = new Command(SignUpClicked);
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the name from user in the Sign Up page.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the username from user in the Sign Up page.
        /// </summary>
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                if (username == value)
                {
                    return;
                }

                username = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the phone from user in the Sign Up page.
        /// </summary>
        public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                if (phone == value)
                {
                    return;
                }

                phone = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the password from users in the Sign Up page.
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }

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

        /// <summary>
        /// Gets or sets the property that bounds with an entry that gets the password confirmation from users in the Sign Up page.
        /// </summary>
        public string ConfirmPassword
        {
            get
            {
                return confirmPassword;
            }

            set
            {
                if (confirmPassword == value)
                {
                    return;
                }

                confirmPassword = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Gets or sets the command that is executed when the Log In button is clicked.
        /// </summary>
        public Command LoginCommand { get; set; }

        /// <summary>
        /// Gets or sets the command that is executed when the Sign Up button is clicked.
        /// </summary>
        public Command SignUpCommand { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Invoked when the Log in button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void LoginClicked(object obj)
        {
            _navigationService.NavigateTo("///login");
        }

        /// <summary>
        /// Invoked when the Sign Up button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void SignUpClicked(object obj)
        {
            if (Password == ConfirmPassword)
            {
                var task = Task.Run(async () => await MySQLDataStore.Instance.UserSignUpAsync(Name, Phone, Email, Username, Password));

                if (task.Result == 0)
                {
                    _navigationService.NavigateTo("///login");
                }
                else if (task.Result == -1)
                {
                    Application.Current.MainPage.DisplayAlert(AppResources.EmailInUseTitle, AppResources.EmailInUseDescr, "Ok");
                }
                else if (task.Result == -2)
                {
                    Application.Current.MainPage.DisplayAlert(AppResources.UsernameInUseTitle, AppResources.UsernameInUseDescr, "Ok");
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert(AppResources.PasswordsDoNotMatchTitle, AppResources.PasswordsDoNotMatchDescr, "Ok");
            }
        }

        #endregion
    }
}