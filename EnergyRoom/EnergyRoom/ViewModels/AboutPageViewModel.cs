using Xamarin.Forms;
using Xamarin.Forms.Internals;
using EnergyRoom.Resources;

namespace EnergyRoom.ViewModels
{
    [Preserve(AllMembers = true)]
    public class AboutPageViewModel : BaseViewModel
    {
        #region Fields

        private string productDescription;

        private string productVersion;

        #endregion

        #region Constructor

        public AboutPageViewModel()
        {
            productDescription = AppResources.RecordVoicePrivacy;
            productVersion = AppResources.VersionText + " 1.0";
        }

        #endregion

        #region Properties

        public string ProductDescription
        {
            get
            {
                return productDescription;
            }

            set
            {
                productDescription = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public string ProductVersion
        {
            get
            {
                return productVersion;
            }

            set
            {
                productVersion = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        #endregion
    }
}