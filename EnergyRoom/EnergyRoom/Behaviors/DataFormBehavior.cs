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
using EnergyRoom.Resources;
using EnergyRoom.Services;
using EnergyRoom.ViewModels;
using Syncfusion.XForms.DataForm;
using Syncfusion.XForms.DataForm.Editors;
using Xamarin.Forms;
namespace EnergyRoom.Behaviors
{
    public class DataFormBehavior : Behavior<ContentPage>
    {
        SfDataForm dataForm = null;

        protected override void OnAttachedTo(ContentPage bindable)
        {
            base.OnAttachedTo(bindable);

            dataForm = bindable.FindByName<SfDataForm>("dataForm");
            dataForm.DataObject = new Meeting(BaseViewModel.Instance.SelectedDateTime, BaseViewModel.Instance.SelectedDateTime.AddHours(1));
            dataForm.RegisterEditor("TypeId", "DropDown");
            dataForm.AutoGeneratingDataFormItem += DataForm_AutoGeneratingDataFormItem;
        }

        private void DataForm_AutoGeneratingDataFormItem(object sender, AutoGeneratingDataFormItemEventArgs e)
        {
            if (e.DataFormItem.Name == "Id")
                e.Cancel = true;
            if (e.DataFormItem.Name == "FullName")
                e.Cancel = true;
            if (e.DataFormItem.Name == "UserName")
                e.Cancel = true;
            if (e.DataFormItem.Name == "EndTime")
                e.Cancel = true;
            if (e.DataFormItem.Name == "IsBooked")
                e.Cancel = true;
            if (e.DataFormItem.Name == "EventCount")
                e.Cancel = true;
            if (e.DataFormItem.Name == "EventId")
                e.Cancel = true;
            if (e.DataFormItem.Name == "Location")
                e.Cancel = true;
            if (e.DataFormItem.Name == "Color")
                e.Cancel = true;
            if (e.DataFormItem.Name == "EventTitle")
                e.Cancel = true;
            if (e.DataFormItem.Name == "EventIcon")
                e.Cancel = true;
            if (e.DataFormItem.Name == "UserId")
            {
                e.Cancel = true;
                dataForm.ItemManager.SetValue(e.DataFormItem, App._userId);
                dataForm.UpdateEditor("UserId");
            }

            var model = MySQLDataStore.Instance;
            var evTypes = model.evTypes;

            if (e.DataFormItem != null && e.DataFormItem.Name == "TypeId")
            {
                e.DataFormItem.LabelText = AppResources.TypeText;

                var list = new List<EventTypes>();
                foreach (EventTypes evType in evTypes)
                {
                    list.Add(new EventTypes() { ID = evType.ID, Name = evType.Name });
                }
                (e.DataFormItem as DataFormDropDownItem).SelectedValuePath = "ID";
                (e.DataFormItem as DataFormDropDownItem).DisplayMemberPath = "Name";
                (e.DataFormItem as DataFormDropDownItem).ItemsSource = list;
            }
            if (e.DataFormItem.Name == "Date")
                e.DataFormItem.LabelText = AppResources.DateText;
            if (e.DataFormItem.Name == "InputStartTime")
                e.DataFormItem.LabelText = AppResources.StartTimeText;
            if (e.DataFormItem.Name == "InputEndTime")
                e.DataFormItem.LabelText = AppResources.EndTimeText;
        }
    }

}
