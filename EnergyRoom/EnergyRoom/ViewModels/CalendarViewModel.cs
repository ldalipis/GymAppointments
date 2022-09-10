using Syncfusion.SfSchedule.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using EnergyRoom.Models;

namespace EnergyRoom.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        //public ObservableCollection<Meeting> Meetings { get; set; }
        //public ObservableCollection<Item> Items { get; }
        //public Command LoadItemsCommand { get; }

        //List<string> eventNameCollection;
        //List<Color> colorCollection;
        public CalendarViewModel()
        {
            //Title = "Calendar";
            //Items = new ObservableCollection<Item>();
            //LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            //this.Meetings = new ObservableCollection<Meeting>();
            //this.CreateColorCollection();
            //this.CreateAppointments();
        }

        /// <summary>
        /// Creates meetings and stores in a collection.
        /// </summary>
        //private void CreateAppointments()
        //{
        //    LoadItemsCommand.Execute(null);

        //    DateTime date;
        //    DateTime DateFrom = DateTime.Parse(Items.Min(x => x.Date));
        //    DateTime DateTo = DateTime.Parse(Items.Max(x => x.Date));

        //    foreach (Item item in Items)
        //    {
        //        Meeting meeting = new Meeting();
        //        date = DateTime.Parse(item.Date);

        //        meeting.Organizer = item.Fullname;
        //        meeting.EventId = item.Id;
        //        meeting.EventType = item.TypeId;
        //        meeting.BeginTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        //        meeting.EndTime = meeting.BeginTime.AddHours(1);
        //        meeting.EventName = item.Title;
        //        meeting.Color = colorCollection[0];
        //        Meetings.Add(meeting);
        //    }
        //}

        ///// <summary>  
        ///// Creates color collection.  
        ///// </summary>  
        //private void CreateColorCollection()
        //{
        //    colorCollection = new List<Color>();
        //    colorCollection.Add(Color.FromHex("#FF339933"));
        //    colorCollection.Add(Color.FromHex("#FF00ABA9"));
        //    colorCollection.Add(Color.FromHex("#FFE671B8"));
        //}

        //async Task ExecuteLoadItemsCommand()
        //{
        //    IsBusy = true;

        //    try
        //    {
        //        Items.Clear();
        //        var items = await DataStore.GetItemsAsync(true);
        //        foreach (var item in items)
        //        {
        //            Items.Add(item);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}

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

                await DataStore.UpdateItemAsync(newItem);
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