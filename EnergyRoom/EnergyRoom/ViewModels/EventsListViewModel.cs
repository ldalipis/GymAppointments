using EnergyRoom.Models;
using EnergyRoom.Resources;
using EnergyRoom.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EnergyRoom.ViewModels
{
    /// <summary>
    /// ViewModel for task events list page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public class EventsListViewModel : BaseViewModel
    {
        #region Fields

        private static EventsListViewModel eventsListViewModel;
        public static EventsListViewModel EventsListInstance => eventsListViewModel ??= new EventsListViewModel();

        private Command<object> itemTappedCommand;

        private Command<object> backCommand;

        private Command<object> menuCommand;

        private Command<object> markAllReadCommand;

        public ICommand RefreshListCommand { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the command that will be executed when an item is selected.
        /// </summary>
        public Command<object> ItemTappedCommand
        {
            get
            {
                return itemTappedCommand ??= new Command<object>(ItemSelected);
            }
        }

        /// <summary>
        /// Gets the command that will be executed when back button is selected.
        /// </summary>
        public Command<object> BackCommand
        {
            get
            {
                return backCommand ??= new Command<object>(BackButtonClicked);
            }
        }

        /// <summary>
        /// Gets the command that will be executed when menu button is selected.
        /// </summary>
        public Command<object> MenuCommand
        {
            get
            {
                return menuCommand ??= new Command<object>(MenuButtonClicked);
            }
        }

        public Command<object> MarkAllReadCommand
        {
            get
            {
                return markAllReadCommand ??= new Command<object>(MarkAllReadButtonClicked);
            }
        }

        /// <summary>
        /// Gets or sets a collection of value to be displayed in task events list page.
        /// </summary>
        [DataMember(Name = "eventsListPageList")]
        public ObservableCollection<EventsListModel> EventsList { get; set; }

        #endregion

        #region Constructor

        public EventsListViewModel()
        {
            RefreshListCommand = new Command(CreateEventsList);
            EventsList = new ObservableCollection<EventsListModel>();
            CreateEventsList();
        }

        #endregion

        #region Methods

        public void UpdateNotificationsFile(EventsListModel data)
        {
            var file = "EnergyRoom.Data.notification.json";

            var assembly = typeof(App).GetTypeInfo().Assembly;

            using StreamWriter str = new StreamWriter(assembly.GetManifestResourceStream(file));
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(str, data);
        }

        private void ItemSelected(object selectedItem)
        {
            ((selectedItem as Syncfusion.ListView.XForms.ItemTappedEventArgs)?.ItemData as EventsListModel).IsRead = true;
            // Do something
        }

        private void CreateEventsList()
        {
            EventsList.Clear();

            foreach (var item in MySQLDataStore.Instance.WeekViewPageViewModel.AppointmentsMeetings)
            {
                EventsListModel newEvent = new EventsListModel
                {
                    UserName = item.UserName,
                    BackgroundColor = "#837bff",
                    Description = item.EventTitle.ToString(),
                    Detail = AppResources.TrainerIsText + " " + item.FullName,
                    TaskID = item.UserName,
                    Time = item.Date.ToString("dd/MM/yyyy") + " @ " + item.StartTime.ToString("HH:mm") + " - " + item.EndTime.ToString("HH:mm"),
                    IsRead = true
                };

                EventsList.Add(newEvent);
            }
        }

        private void MarkAllReadButtonClicked(object obj)
        {
            foreach (var item in EventsList)
            {
                item.IsRead = true;
            }
        }

        /// <summary>
        /// Invoked when back button is clicked in the task notification page.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void BackButtonClicked(object obj)
        {
            // Do something
        }

        /// <summary>
        /// Invoked when menu button is clicked in the task notification page.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MenuButtonClicked(object obj)
        {
            // Do something
        }

        #endregion
    }
}