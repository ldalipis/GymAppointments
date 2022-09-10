using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using EnergyRoom.Models;
using EnergyRoom.Services;
using EnergyRoom.Views;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;
using Xamarin.Essentials;
using EnergyRoom.Resources;
using System.Collections.ObjectModel;

namespace EnergyRoom.ViewModels
{
    public class WeekViewPageViewModel : BaseViewModel
    {
        public DataTemplate AppointmentDataTemplate { get; set; }
        public ICommand ScheduleCellTapped { get; set; }
        public ICommand ScheduleCellDoubleTapped { get; set; }
        public ICommand OpenAdminPopupCommand { get; set; }
        public ICommand CloseAdminPopupCommand { get; set; }
        public ICommand OpenClientPopupCommand { get; set; }
        public ICommand CloseClientPopupCommand { get; set; }
        public ICommand AdminPopupDeclineCommand { get; set; }
        public ICommand ClientPopupAcceptCommand { get; set; }
        public ICommand PhoneTapCommand { get; set; }
        public ICommand ExecuteGetUserAppointmentsCommand { get; set; }
        public ICommand VAAddNewEventCommand {
            get
            {
                return new Command<Meeting>((x) => VAAddNewEvent(x));
            }
        }

        public ICommand CloseAddNewPageCommand
        {
            get
            {
                return new Command (CloseAddNewPage);
            }
        }

        private IEnumerable<Appointments> eventAppointments;

        private IList<Appointments> userAppointments;
        public IList<Appointments> UserAppointments
        {
            get
            {
                return userAppointments;
            }
            set
            {
                userAppointments = value;
                OnPropertyChanged("UserAppointments");
            }
        }

        public ObservableCollection<Meeting> AppointmentsMeetings;

        private ObservableCollection<Item> bookings;
        public ObservableCollection<Item> Bookings
        {
            get
            {
                return bookings;
            }
            set
            {
                bookings = value;
                OnPropertyChanged("Bookings");
            }
        }

        private bool displayAdminPopup;
        public bool DisplayAdminPopup
        {
            get
            {
                return displayAdminPopup;
            }
            set
            {
                displayAdminPopup = value;
                OnPropertyChanged("DisplayAdminPopup");
            }
        }

        private bool displayClientPopup;
        public bool DisplayClientPopup
        {
            get
            {
                return displayClientPopup;
            }
            set
            {
                displayClientPopup = value;
                OnPropertyChanged("DisplayClientPopup");
            }
        }

        string _book;
        public string Book
        {
            get => _book;
            set { SetProperty(ref _book, value); }
        }

        bool _isEventOwner;
        public bool IsEventOwner
        {
            get => _isEventOwner;
            set { SetProperty(ref _isEventOwner, value); }
        }
        
        string _eventGymnast;
        public string EventGymnast
        {
            get => _eventGymnast;
            set { SetProperty(ref _eventGymnast, value); }
        }

        string _detailsText;
        public string DetailsText
        {
            get => _detailsText;
            set { SetProperty(ref _detailsText, value); }
        }

        string _adminText;
        public string AdminText
        {
            get => _adminText;
            set { SetProperty(ref _adminText, value); }
        }

        string _clientText;
        public string ClientText
        {
            get => _clientText;
            set { SetProperty(ref _clientText, value); }
        }

        int _eventId;
        public int EventId
        {
            get => _eventId;
            set { SetProperty(ref _eventId, value); }
        }

        string _eventData;
        public string EventData
        {
            get => _eventData;
            set { SetProperty(ref _eventData, value); }
        }

        int _userId;
        public int UserId
        {
            get => _userId;
            set { SetProperty(ref _userId, value); }
        }
        public WeekViewPageViewModel()
        {
            AppointmentDataTemplate = new DataTemplate(() => new AppointmentDataTemplate());
            ScheduleCellTapped = new Command<CellTappedEventArgs>(Tapped);
            ScheduleCellDoubleTapped = new Command<CellTappedEventArgs>(DoubleTapped);
            OpenAdminPopupCommand = new Command(OpenAdminPopup);
            CloseAdminPopupCommand = new Command(CloseAdminPopup);
            OpenClientPopupCommand = new Command(OpenClientPopup);
            CloseClientPopupCommand = new Command(CloseClientPopup);
            AdminPopupDeclineCommand = new Command(AdminPopupDecline);
            ClientPopupAcceptCommand = new Command(ClientPopupAccept);
            ExecuteGetUserAppointmentsCommand = new Command(ExecuteGetUserAppointments);
            PhoneTapCommand = new Command(PhoneTap);
            AppointmentsMeetings = new ObservableCollection<Meeting>();
            Bookings = new ObservableCollection<Item>();
        }

        private void OpenAdminPopup()
        {
            DisplayAdminPopup = true;
        }

        private void CloseAdminPopup()
        {
            DisplayAdminPopup = false;
        }

        private void OpenClientPopup()
        {
            DisplayClientPopup = true;
        }

        private void CloseClientPopup()
        {
            DisplayClientPopup = false;
        }

        private void AdminPopupDecline()
        {
            if (IsBusy)
            {
                return;
            }

            DisplayAdminPopup = false;

            Task.Run(async () => await ExecuteDeleteItem(EventId));

            var data = MySQLDataStore.Instance.Data;
            var itemToRemove = data.Where((Meeting arg) => arg.EventId == EventId).FirstOrDefault();
            data.Remove(itemToRemove);

            var AppToRemove = AppointmentsMeetings.Where(x => x.EventId == EventId).Single();
            AppointmentsMeetings.Remove(AppToRemove);

            EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
        }

        private void ClientPopupAccept()
        {
            if (IsBusy)
            {
                return;
            }

            DisplayClientPopup = false;

            var data = MySQLDataStore.Instance.Data;
            long appId = 0;

            Appointments userAppointmentsDB = UserAppointments.Where(x => x.EventId == EventId).FirstOrDefault();
            if (userAppointmentsDB != null)
            {
                appId = userAppointmentsDB.ID;
            }

            if (appId == 0)
            {
                Task.Run(async () => await ExecuteAddAppointment(EventId, App._userId));
                data.Where(x => x.EventId == EventId).Single().IsBooked = true;
                data.Where(x => x.EventId == EventId).Single().EventCount += 1;
            }
            else
            {
                Task.Run(async () => await ExecuteWithdrawAppointment(EventId, appId));
                data.Where(x => x.EventId == EventId).Single().IsBooked = false;
                data.Where(x => x.EventId == EventId).Single().EventCount -= 1;
            }
        }

        public async Task<IEnumerable<Meeting>> ExecuteLoadDataCommand()
        {
            return await MySQLDataStore.Instance.GetItemsAsync();
        }

        private void ExecuteGetUserAppointments()
        {
            if (App._userIsAdmin)
            {
                var task = Task.Run(async () => await MySQLDataStore.Instance.GetAdminAppointmentsAsync());
                UserAppointments = task.Result.ToList();
            }
            else
            {
                var task = Task.Run(async () => await MySQLDataStore.Instance.GetUserAppointmentsAsync());
                UserAppointments = task.Result.ToList();
            }

            var data = MySQLDataStore.Instance.Data;

            foreach (Meeting m in data)
            {
                m.IsBooked = false;
            }

            if (UserAppointments != null)
            {
                var crossList = data.Where(item1 => UserAppointments.Any(item2 => item1.EventId == item2.EventId));

                IList<int> crossEventIDs = new List<int>();
                crossEventIDs = crossList.Select(x => x.EventId).ToList();

                foreach (Meeting m in data)
                {
                    if (crossEventIDs.Contains(m.EventId))
                    {
                        m.IsBooked = true;
                    }
                }

                AppointmentsMeetings.Clear();

                foreach (Meeting item in crossList)
                {
                    AppointmentsMeetings.Add(item);
                    data.Where(x => x.EventId == item.EventId).Single().IsBooked = true;
                }
            }

            EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
        }

        private void PhoneTap(object number)
        {
            try
            {
                PhoneDialer.Open((string)number);
            }
            catch (ArgumentNullException anEx)
            {
                // Number was null or white space
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private async Task<bool> LoadEventDataAdminAsync(object appointment, string evDate, string evStartTime, string evEndTime)
        {
            IsBusy = true;

            Bookings.Clear();
            eventAppointments = Task.Run(async () => await MySQLDataStore.Instance.GetEventAppointmentsAsync(EventId)).Result;

            foreach (var ea in eventAppointments)
            {
                byte[] userPhoto = null;
                try
                {
                    userPhoto = Task.Run(async () => await MySQLDataStore.Instance.GetUserPhoto(ea.UserId)).Result;
                }
                catch (Exception ex)
                {
                }

                Bookings.Add(new Item { ImageSrc = userPhoto, FullName = ea.UserFullname, Phone = ea.UserPhone });
            }

            //var bookings = eventAppointments.Select(x => x.UserFullname).ToList();
            IsEventOwner = (appointment as Meeting).UserId == App._userId;
            AdminText = (appointment as Meeting).EventTitle + " @ " + evDate + "\r\n" + evStartTime + " - " + evEndTime;
            EventGymnast = AppResources.GymnastText + ": " + (appointment as Meeting).FullName;
            DetailsText = Bookings.Count > 0 ? AppResources.UsersThatHaveBookedText : AppResources.NoBookingsOnEventText;

            IsBusy = false;

            return await Task.FromResult(true);
        }

        private async Task<bool> LoadEventDataUserAsync(object appointment, string evDate, string evStartTime, string evEndTime)
        {
            IsBusy = true;

            eventAppointments = Task.Run(async () => await MySQLDataStore.Instance.GetEventAppointmentsAsync(EventId)).Result;

            bool eventIsBooked = userAppointments.Where(x => x.EventId == (appointment as Meeting).EventId).Any();
            if (eventIsBooked)
            {
                Book = AppResources.WithdrawText;
                ClientText = (appointment as Meeting).EventTitle + "\n" + AppResources.DateText + ": " + evDate + "\n" + AppResources.TimeText + ": " + evStartTime + " - " + evEndTime + "\n\n" + AppResources.UserHasBookedEventText + ".";
            }
            else
            {
                Book = AppResources.BookText;
                ClientText = (appointment as Meeting).EventTitle + "\n" + AppResources.DateText + ": " + evDate + "\n" + AppResources.TimeText + ": " + evStartTime + " - " + evEndTime + "\n\n" + AppResources.UserCanTakePlaceInEventText + ".";
            }

            IsBusy = false;

            return await Task.FromResult(true);
        }

        private void Tapped(CellTappedEventArgs args)
        {
            Book = EventData = AdminText = ClientText = EventGymnast = DetailsText = "";
            Bookings.Clear();

            var appointment = args.Appointment;

            if (appointment == null)
            {
                return;
            }

            EventId = (appointment as Meeting).EventId;
            EventData = AppResources.EventInfoText;
            string evDate = (appointment as Meeting).Date.Day + "/" + (appointment as Meeting).Date.Month + "/" + (appointment as Meeting).Date.Year;
            string evStartTime = (appointment as Meeting).StartTime.Hour + ":" + (appointment as Meeting).StartTime.Minute;
            string evEndTime = (appointment as Meeting).EndTime.Hour + ":" + (appointment as Meeting).EndTime.Minute;

            if (App._userIsAdmin)
            {
                OpenAdminPopupCommand.Execute(null);
                Task.Run(async () => await LoadEventDataAdminAsync(appointment, evDate, evStartTime, evEndTime));
            }
            else
            {
                OpenClientPopupCommand.Execute(null);
                Task.Run(async () => await LoadEventDataUserAsync(appointment, evDate, evStartTime, evEndTime));
            }
        }

        private void DoubleTapped(CellTappedEventArgs args)
        {
            if (App._userIsAdmin)
            {
                SelectedDateTime = args.Datetime;
                Application.Current.MainPage.Navigation.PushAsync(new AddNewPage(SelectedDateTime), true);
            }
        }

        private void VAAddNewEvent(Meeting details)
        {
            if (App._userIsAdmin)
            {
                int SelectedEvent = details.TypeId;
                SelectedDateTime = details.StartTime;
                Application.Current.MainPage.Navigation.PushAsync(new AddNewPage(SelectedDateTime, SelectedEvent), true);
            }
        }

        private void CloseAddNewPage()
        {
            Application.Current.MainPage.Navigation.PopAsync(true);
        }

        public async Task ExecuteUpdateItem(int eventId, string newTime, int eventType)
        {
            IsBusy = true;

            try
            {
                Item newItem = new Item()
                {
                    Id = eventId,
                    Date = newTime,
                    TypeId = eventType
                };

                await MySQLDataStore.Instance.UpdateItemAsync(newItem);
                EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteDeleteItem(int eventId)
        {
            IsBusy = true;

            try
            {
                await MySQLDataStore.Instance.DeleteItemAsync(eventId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteAddAppointment(int eventId, int userId)
        {
            IsBusy = true;

            try
            {
                long newId = await MySQLDataStore.Instance.AddAppointmentAsync(eventId, userId);
                var data = MySQLDataStore.Instance.Data;
                var UserMeetingData = data.Where(x => x.EventId == EventId).Single();
                AppointmentsMeetings.Add(new Meeting(eventId, UserMeetingData.TypeId, userId, UserMeetingData.FullName, UserMeetingData.UserName, UserMeetingData.StartTime, UserMeetingData.EndTime, UserMeetingData.EventCount, true)
                {
                    EventId = eventId,
                    TypeId = UserMeetingData.TypeId,
                    UserId = userId,
                    FullName = UserMeetingData.FullName,
                    UserName = UserMeetingData.UserName,
                    StartTime = UserMeetingData.StartTime,
                    EndTime = UserMeetingData.EndTime,
                    EventCount = UserMeetingData.EventCount,
                    IsBooked = true,
                });
                UserAppointments.Add(new Appointments { ID = newId, EventId = eventId , UserId = userId });
                EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteWithdrawAppointment(int eventId, long id)
        {
            IsBusy = true;

            try
            {
                await MySQLDataStore.Instance.DeleteAppointmentAsync(id);
                var AppToRemove = AppointmentsMeetings.Where(x => x.EventId == eventId).Single();
                AppointmentsMeetings.Remove(AppToRemove);
                var UsrAppToRemove = UserAppointments.Where(x => x.ID == id).Single();
                UserAppointments.Remove(UsrAppToRemove);
                EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }

}
