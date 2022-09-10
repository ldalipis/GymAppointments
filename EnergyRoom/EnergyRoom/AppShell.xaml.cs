using System;
using System.Collections.Generic;
using EnergyRoom.ViewModels.Forms;
using EnergyRoom.Views.Forms;
using System.Windows.Input;
using Xamarin.Forms;

namespace EnergyRoom
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            //Routing.RegisterRoute("registration", typeof(SignUpPage));
            //Routing.RegisterRoute("login", typeof(LoginPage));
            BindingContext = this;
        }

    }
}
