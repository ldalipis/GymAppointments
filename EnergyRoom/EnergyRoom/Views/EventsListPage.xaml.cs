using EnergyRoom.Services;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace EnergyRoom.Views
{
    /// <summary>
    /// Page to display Events list.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsListPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventsListPage" /> class.
        /// </summary>
        public EventsListPage()
        {
            InitializeComponent();
            BindingContext = EventsListDataService.Instance.EventsListViewModel;
        }

    }
}