using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using Uri = Android.Net.Uri;

namespace EnergyRoom.Droid
{
    //You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application
    {
        static readonly string TAG = "MainActivity";

        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CrossCurrentActivity.Current.Init(this);

            //Set the default notification channel for your app when running Android Oreo
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "";

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "";
            }

            //If debug you should reset the token each time.
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, new NotificationUserCategory[]
            {
                new NotificationUserCategory("message",new List<NotificationUserAction> {
                new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground),
                new NotificationUserAction("Forward","Forward",NotificationActionType.Foreground)

            }),
                new NotificationUserCategory("request",new List<NotificationUserAction> {
                new NotificationUserAction("Accept","Accept",NotificationActionType.Default,"check"),
                new NotificationUserAction("Reject","Reject",NotificationActionType.Default,"cancel")
            })

            }, true);
#else
	            FirebasePushNotificationManager.Initialize(this,new NotificationUserCategory[]
		    {
			new NotificationUserCategory("message",new List<NotificationUserAction> {
			    new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground),
			    new NotificationUserAction("Forward","Forward",NotificationActionType.Foreground)

			}),
			new NotificationUserCategory("request",new List<NotificationUserAction> {
			    new NotificationUserAction("Accept","Accept",NotificationActionType.Default,"check"),
			    new NotificationUserAction("Reject","Reject",NotificationActionType.Default,"cancel")
			})

		    },false);
#endif

            //Handle notification when app is closed here
            CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            {
                Log.Debug(TAG, "Received");
                foreach (var data in p.Data)
                {
                    Log.Debug(TAG, $"{data.Key} : {data.Value}");

                    if (data.Key == "event" && (string)data.Value == "refresh")
                    {
                        RefreshEvents();
                    }
                    //else if (data.Key == "event" && (string)data.Value == "zoom")
                    //{
                    //    LaunchZoomUrl();
                    //}
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            {
                Log.Debug(TAG, "Opened");
                foreach (var data in p.Data)
                {
                    Log.Debug(TAG, $"{data.Key} : {data.Value}");

                    if (data.Key == "event" && (string)data.Value == "refresh")
                    {
                        RefreshEvents();
                    }
                    else if (data.Key == "event" && (string)data.Value == "zoom")
                    {
                        LaunchZoomUrl();
                    }
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationAction += (s, p) =>
            {
                Log.Debug(TAG, "Action");

                if (!string.IsNullOrEmpty(p.Identifier))
                {
                    Log.Debug(TAG, $"ActionId: {p.Identifier}");
                    foreach (var data in p.Data)
                    {
                        Log.Debug(TAG, $"{data.Key} : {data.Value}");
                    }
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationDeleted += (s, p) =>
            {
                Log.Debug(TAG, "Deleted");
            };

            CrossFirebasePushNotification.Current.Subscribe("newEvent");
        }

        private void RefreshEvents()
        {
            Xamarin.Forms.MessagingCenter.Send<object, string>(this, "events", "refresh");
        }

        private void LaunchZoomUrl()
        {
            Intent intent = new Intent(Intent.ActionView);
            intent.SetData(Uri.Parse(""));
            intent.SetFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);
        }

    }
}