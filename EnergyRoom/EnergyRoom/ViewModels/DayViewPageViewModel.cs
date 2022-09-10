using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EnergyRoom.Models;
using EnergyRoom.Services;
using EnergyRoom.Views;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;

namespace EnergyRoom.ViewModels
{
    public class DayViewPageViewModel : BaseViewModel
    {
        public ICommand ScheduleCellDoubleTapped { get; set; }

        public DayViewPageViewModel()
        {
            ScheduleCellDoubleTapped = new Command<CellTappedEventArgs>(DoubleTapped);
        }

        private void DoubleTapped(CellTappedEventArgs args)
        {
            if (App._userIsAdmin)
            {
                SelectedDateTime = args.Datetime;
                Application.Current.MainPage.Navigation.PushAsync(new AddNewPage(DateTime.Now), true);
            }
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
