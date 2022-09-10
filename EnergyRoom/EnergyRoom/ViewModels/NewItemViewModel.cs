using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using EnergyRoom.Models;
using Xamarin.Forms;

namespace EnergyRoom.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string date;
        private int typeid;
        private int userid;

        public NewItemViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return date != null
                && typeid != 0
                && userid != 0;
                //&& !String.IsNullOrWhiteSpace(fullname);
        }

        public string Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        public int TypeId
        {
            get => typeid;
            set => SetProperty(ref typeid, value);
        }

        public int Userid
        {
            get => userid;
            set => SetProperty(ref userid, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Item newItem = new Item()
            {
                Date = date,
                TypeId = typeid,
                UserId = userid
            };

            //await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
