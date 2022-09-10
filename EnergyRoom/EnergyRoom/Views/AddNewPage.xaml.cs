using System;
using EnergyRoom.Services;
using EnergyRoom.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EnergyRoom.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddNewPage : ContentPage
	{
        public AddNewPage(DateTime SelectedDateTime, int SelectedEvent = 1)
		{
			InitializeComponent();
            BaseViewModel.Instance.SelectedDateTime = SelectedDateTime;
            BaseViewModel.Instance.SelectedEvent = SelectedEvent;
            BindingContext = new AddNewPageViewModel();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync(true);
        }
    }
}