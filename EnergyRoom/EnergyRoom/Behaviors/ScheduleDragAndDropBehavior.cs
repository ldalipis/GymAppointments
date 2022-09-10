using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;

using EnergyRoom.Models;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;
using EnergyRoom.ViewModels;
using EnergyRoom.Services;
using EnergyRoom.Views;

namespace EnergyRoom.Behaviors
{
    public class ScheduleDragAndDropBehavior : Behavior<SfSchedule>
    {
        public View AssociatedObject { get; private set; }

        void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        protected override void OnAttachedTo(SfSchedule bindable)
        {
            base.OnAttachedTo(bindable);

            AssociatedObject = bindable;

            bindable.BindingContextChanged += OnBindingContextChanged;
            bindable.AppointmentDrop += Bindable_AppointmentDrop;
            bindable.AppointmentDragStarting += Bindable_AppointmentDragStarting;
        }
        
        private void Bindable_AppointmentDragStarting(object sender, AppointmentDragStartingEventArgs e)
        {
            if (!App._userIsAdmin)
            {
                e.Cancel = true;
            }
        }

        private void Bindable_AppointmentDrop(object sender, AppointmentDropEventArgs e)
        {
            if (!App._userIsAdmin)
            {
                e.Cancel = true;
            }
            else
            {
                var appointment = e.Appointment;

                (appointment as Meeting).StartTime = e.DropTime;

                e.Cancel = false;
                var dropTime = e.DropTime;

                //var changedResource = e.DropResourceItem;

                Item newItem = new Item()
                {
                    Id = (appointment as Meeting).EventId,
                    Date = dropTime.ToString(),
                    TypeId = (appointment as Meeting).TypeId
                };

                Task.Run(async () => await MySQLDataStore.Instance.WeekViewPageViewModel.ExecuteUpdateItem((appointment as Meeting).EventId, dropTime.ToString(), (appointment as Meeting).TypeId));
                EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
            }
        }

        protected override void OnDetachingFrom(SfSchedule bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            bindable.AppointmentDrop -= Bindable_AppointmentDrop;
            AssociatedObject = null;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }
}