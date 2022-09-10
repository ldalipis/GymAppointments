using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using EnergyRoom.Models;
using EnergyRoom.Services;

namespace EnergyRoom.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        public ObservableCollection<Meeting> Data { get; set; }

        private static BaseViewModel baseViewModel;

        public static BaseViewModel Instance => baseViewModel ??= new BaseViewModel();

        public IList<Meeting> Meetings { get; set; }

        public BaseViewModel()
        {
            Meetings = MySQLDataStore.Instance.Data;
        }

        public bool IsAdmin
        {
            get { return App._userIsAdmin; }
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private int selectedEvent;
        public int SelectedEvent
        {
            get { return selectedEvent; }
            set { SetProperty(ref selectedEvent, value); }
        }

        DateTime selectedDateTime = DateTime.Now;
        public DateTime SelectedDateTime
        {
            get { return selectedDateTime; }
            set { SetProperty(ref selectedDateTime, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
