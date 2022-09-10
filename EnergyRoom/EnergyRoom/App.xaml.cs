using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EnergyRoom.Views.Forms;
using EnergyRoom.Services;
using EnergyRoom.Services.Routing;
using Splat;
using EnergyRoom.Services.Identity;
using EnergyRoom.ViewModels;
using System.Collections.Generic;
using EnergyRoom.Models;

namespace EnergyRoom
{
    public partial class App : Application
    {
        public static string BaseImageUrl { get; } = "https://cdn.syncfusion.com/essential-ui-kit-for-xamarin.forms/common/uikitimages/";
        public static int _userId { get; set; }
        public static string _userName { get; set; }
        public static bool _userIsAdmin { get; set; }
        public static string _name { get; set; }
        public static string _phone { get; set; }
        public static string _email { get; set; }
        public static IList<EventTypes> _EventTypes { get; set; }

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzQ1MDE4QDMxMzgyZTMzMmUzMFhubm5kZ3ZVcS9uaTJnMWIxTHBiODIxKzRMWksrTk1yUHNRNC9GcHBYNG89");

            InitializeDi();
            InitializeComponent();

            DependencyService.Register<MySQLDataStore>();

            MainPage = new AppShell();
        }

        private void InitializeDi()
        {
            // Services
            Locator.CurrentMutable.RegisterLazySingleton<IRoutingService>(() => new ShellRoutingService());
            Locator.CurrentMutable.RegisterLazySingleton<IIdentityService>(() => new IdentityServiceStub());

            // ViewModels
            Locator.CurrentMutable.Register(() => new LoadingViewModel());
            Locator.CurrentMutable.Register(() => new ViewModels.Forms.LoginPageViewModel());
            Locator.CurrentMutable.Register(() => new ViewModels.Forms.SignUpPageViewModel());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
