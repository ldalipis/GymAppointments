using EnergyRoom.Models;
using EnergyRoom.Services;
using EnergyRoom.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EnergyRoom.ViewModels
{
    public class AddNewPageViewModel : BaseViewModel
    {

        public Meeting NewMeeting { get; set; } = new Meeting(Instance.SelectedDateTime, Instance.SelectedDateTime.AddHours(1));

        public Command Submit { get; set; }

        public AddNewPageViewModel()
        {
            var model = MySQLDataStore.Instance;

            NewMeeting.UserId = App._userId;
            NewMeeting.UserName = App._userName;
            NewMeeting.TypeId = Instance.SelectedEvent;
            NewMeeting.Date = Instance.SelectedDateTime;
            NewMeeting.StartTime = Instance.SelectedDateTime;
            NewMeeting.EndTime = NewMeeting.StartTime.AddHours(1);
            NewMeeting.IsBooked = true;

            Submit = new Command(() => {
                MessagingCenter.Send<object, object>(this, "meeting", NewMeeting);

                var newId = Task.Run(async () => await model.AddItemAsync(NewMeeting)).Result;

                NewMeeting.EventId = (int)newId;
                model.Data.Add(NewMeeting);

                var UserMeetingData = model.Data.Where(x => x.EventId == (int)newId).Single();
                MySQLDataStore.Instance.WeekViewPageViewModel.AppointmentsMeetings.Add(new Meeting((int)newId, UserMeetingData.TypeId, App._userId, UserMeetingData.FullName, UserMeetingData.UserName, UserMeetingData.StartTime, UserMeetingData.EndTime, UserMeetingData.EventCount, true)
                {
                    EventId = (int)newId,
                    TypeId = UserMeetingData.TypeId,
                    UserId = App._userId,
                    FullName = App._name,
                    UserName = App._userName,
                    StartTime = UserMeetingData.StartTime,
                    EndTime = UserMeetingData.EndTime,
                    EventCount = UserMeetingData.EventCount,
                    IsBooked = true,
                });
                MySQLDataStore.Instance.WeekViewPageViewModel.UserAppointments.Add(new Appointments { ID = newId, EventId = (int)newId, UserId = App._userId });
                EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
            });
        }
    }
}
