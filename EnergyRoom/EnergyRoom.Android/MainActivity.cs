using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Gms.Common;
using Android.Util;
using System.Threading.Tasks;
using Plugin.FirebasePushNotification;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using EnergyRoom.Services;
using EnergyRoom.Droid.Services;
using Plugin.FacebookClient;
using System.Collections.Generic;
using EnergyRoom.Models;
using EnergyRoom.Resources;

namespace EnergyRoom.Droid
{
    [Activity(Label = "EnergyRoom", Icon = "@mipmap/icon", Exported = true, Theme = "@style/MainTheme", LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "https",
        DataHost = "energyroom.ldalipis.gr",
        DataPath = "/acceptchangeemail/")]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "MainActivity";

        public const string API_KEY = "";

        internal static readonly string CHANNEL_ID = "";
        internal static readonly int NOTIFICATION_ID = 100;

        string msgText;

        IMicrophoneService micService;
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("");

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            FacebookClientManager.Initialize(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            IsPlayServicesAvailable();

            FirebasePushNotificationManager.ProcessIntent(this, Intent);

            MessagingCenter.Subscribe<object, object>(this, "meeting", (object obj, object meeting) =>
            {
                SendMessage(meeting);
            });

            micService = DependencyService.Resolve<IMicrophoneService>();

            /*FirebaseMessaging.Instance.SubscribeToTopic("newEvent");
            FirebaseMessaging.Instance.SubscribeToTopic("newZoom");

            Log.Debug(TAG, "FCMToken: " + Xamarin.Essentials.SecureStorage.GetAsync("FCMToken").Result);*/
        }

        private void SendMessage(object meeting)
        {
            var jGcmData = new JObject();
            var nData = new JObject();
            var jData = new JObject();

            nData.Add("title", AppResources.NewEventText);
            nData.Add("body", (meeting as Meeting).EventTitle + " " + AppResources.AtText + " " + (meeting as Meeting).Date);

            jData.Add("android_channel_id", CHANNEL_ID);
            jData.Add("message", (meeting as Meeting).EventTitle + " " + AppResources.AtText + " " + (meeting as Meeting).Date);
            jData.Add("collapse_key", "gr.ldalipis.energyroom");
            jData.Add("event", "refresh");

            jGcmData.Add("to", "/topics/newEvent");
            jGcmData.Add("notification", nData);
            jGcmData.Add("data", jData);

            var url = new Uri("https://fcm.googleapis.com/fcm/send");
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "Authorization", "key=" + API_KEY);

                    Task.WaitAll(client.PostAsync(url,
                        new StringContent(jGcmData.ToString(), Encoding.Default, "application/json"))
                            .ContinueWith(response =>
                            {
                                Log.Debug(TAG, "response: " + response);
                                Log.Debug(TAG, "Message sent");
                            }));
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, "Unable to send GCM message: " + e.StackTrace);
            }
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    msgText = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msgText = "This device is not supported";
                    Finish();
                }
                Log.Info(TAG, msgText);
                return false;
            }
            else
            {
                msgText = "Google Play Services is available.";
                Log.Info(TAG, msgText);
                return true;
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            FirebasePushNotificationManager.ProcessIntent(this, intent);

            var data = intent.Data;
            string scheme = data.Scheme;
            string host = data.Host;
            IList<string> parameters = data.PathSegments;
            string first = parameters[0];
            string queryParam = data.GetQueryParameter("token");

            if (host == "energyroom.ldalipis.gr" && first == "acceptchangeemail" && !string.IsNullOrWhiteSpace(queryParam))
            {
                MessagingCenter.Send<object, string>(this, "acceptchangeemail", queryParam);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            FacebookClientManager.OnActivityResult(requestCode, resultCode, intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            try
            {
                switch (requestCode)
                {
                    case AndroidMicrophoneService.RecordAudioPermissionCode:
                        if (grantResults[0] == Permission.Granted)
                        {
                            micService.OnRequestPermissionResult(true);
                        }
                        else
                        {
                            micService.OnRequestPermissionResult(false);
                        }
                        break;
                }
            }
            catch
            {

            }
        }
    }
}