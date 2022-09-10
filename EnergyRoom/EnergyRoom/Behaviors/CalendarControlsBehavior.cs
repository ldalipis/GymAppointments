using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EnergyRoom.Models;
using EnergyRoom.Services;
using EnergyRoom.ViewModels;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;
namespace EnergyRoom.Behaviors
{
    public class CalendarControlsBehavior : Behavior<ContentPage>
    {
        SfSchedule schedule = null;
        Button refreshBtn;

        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);

            schedule = bindable.FindByName<SfSchedule>("schedule");

            refreshBtn = bindable.FindByName<Button>("refreshBtn");
            refreshBtn.Clicked += RefreshData;
        }

        private void RefreshData(object sender, EventArgs e)
        {
            var data = MySQLDataStore.Instance.Data;
            var viewModel = schedule.BindingContext as WeekViewPageViewModel;

            var newData = Task.Run(async () => await viewModel.ExecuteLoadDataCommand()).Result;
            
            data.Clear();
            foreach (var item in newData)
            {
                data.Add(item);
            }
        }

    }

}
