using EnergyRoom.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.CognitiveServices.Speech;
using EnergyRoom.Droid;
using System;
using EnergyRoom.Models;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.Linq;

namespace EnergyRoom.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WeekViewPage : ContentPage
	{
        SpeechRecognizer recognizer;
        IMicrophoneService micService;
        bool isTranscribing = false;
        bool isBusy = false;

        public readonly List<(string, int)> EventsTypes = new List<(string, int)>
        {
            ("αντιστάσεις", 1),
            ("κρόσφιτ", 2),
            ("πιλάτες", 3),
            ("τρέξιμο", 4)
        };

        public readonly List<(string, int)> DaysNumbers = new List<(string, int)>
        {
            ("κυριακή", 0),
            ("δευτέρα", 1),
            ("τρίτη", 2),
            ("τετάρτη", 3),
            ("πέμπτη", 4),
            ("παρασκευή", 5),
            ("σάββατο", 6)
        };

        public WeekViewPage()
		{
			InitializeComponent();

            BindingContext = MySQLDataStore.Instance.WeekViewPageViewModel;

            micService = DependencyService.Resolve<IMicrophoneService>();

            TranscribeClicked(null, null);
        }

        public void SpeakMultiple(string sentence, SpeechOptions settings)
        {
            isBusy = true;
            Task.WhenAll(
                TextToSpeech.SpeakAsync(sentence, settings),
                TextToSpeech.SpeakAsync("Να το προσθέσω?", settings))
                .ContinueWith((t) => { isBusy = false; }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task SpeakNow(string sentence)
        {
            var locales = await TextToSpeech.GetLocalesAsync();
            var locale = locales.Where(x => x.Language == "el").Single();

            var settings = new SpeechOptions()
            {
                Volume = .75f,
                Pitch = 1.0f,
                Locale = locale
            };

            SpeakMultiple(sentence, settings);

            //await TextToSpeech.SpeakAsync(sentence, settings).ContinueWith((t) =>
            //{
            //    // Logic that will run after utterance finishes.

            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }

        async void TranscribeClicked(object sender, EventArgs e)
        {
            bool isMicEnabled = await micService.GetPermissionAsync();

            // EARLY OUT: make sure mic is accessible
            if (!isMicEnabled)
            {
                UpdateTranscription("Please grant access to the microphone!");
                return;
            }

            // initialize speech recognizer 
            if (recognizer == null)
            {
                var config = SpeechConfig.FromSubscription(Constants.CognitiveServicesApiKey, Constants.CognitiveServicesRegion);
                recognizer = new SpeechRecognizer(config, "el-GR");
                recognizer.Recognized += (obj, args) =>
                {
                    UpdateTranscription(args.Result.Text);
                };
            }

            //if already transcribing, stop speech recognizer
            //if (isTranscribing)
            //{
            //    try
            //    {
            //        await recognizer.StopContinuousRecognitionAsync();
            //    }
            //    catch (Exception ex)
            //    {
            //        UpdateTranscription(ex.Message);
            //    }
            //    isTranscribing = false;
            //}

            //if not transcribing, start speech recognizer
            //else
            //{
                Device.BeginInvokeOnMainThread(() =>
                {
                    //InsertDateTimeRecord();
                });
                try
                {
                    await recognizer.StartContinuousRecognitionAsync();
                }
                catch (Exception ex)
                {
                    UpdateTranscription(ex.Message);
                }
                isTranscribing = true;
            //}
            //UpdateDisplayState();
        }

        void UpdateTranscription(string newText)
        {
            //TranscribeClicked(null, null);

            newText = newText.ToLower();

            Device.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrWhiteSpace(newText))
                {
                    //transcribedText.Text += $"{newText}\n";

                    string acceptPattern = @"ναι|o.k.|ok|οκέι|πρόσθεσε|βάλε";
                    Regex acceptRegex = new Regex(acceptPattern);
                    if (acceptRegex.Matches(newText).Count > 0)
                    {
                        MySQLDataStore.Instance.WeekViewPageViewModel.CloseAddNewPageCommand.Execute(null);
                        MySQLDataStore.Instance.AddNewPageViewModel.Submit.Execute(null);
                    }

                    string rejectPattern = @"όχι|άκυρο";
                    Regex rejectRegex = new Regex(rejectPattern);
                    if (rejectRegex.Matches(newText).Count > 0)
                    {
                        MySQLDataStore.Instance.WeekViewPageViewModel.CloseAddNewPageCommand.Execute(null);
                    }

                    string typesPattern = @"αντιστάσεις|κρόσφιτ|πιλάτες|τρέξιμο";
                    Regex typesRegex = new Regex(typesPattern);
                    string typeFound = "";
                    foreach (Match match in typesRegex.Matches(newText))
                    {
                        typeFound = match.Value;
                    }

                    string daysPattern = @"κυριακή|δευτέρα|τρίτη|τετάρτη|πέμπτη|παρασκευή|σάββατο";
                    Regex daysRegex = new Regex(daysPattern);
                    string dayFound = "";
                    foreach (Match match in daysRegex.Matches(newText))
                    {
                        dayFound = match.Value;
                    }

                    string morningEveningPattern = @"πρωί|απόγευμα|βράδυ|απόψε";
                    Regex morningEveningRegex = new Regex(morningEveningPattern);
                    string morningEveningFound = "";
                    foreach (Match match in morningEveningRegex.Matches(newText))
                    {
                        morningEveningFound = match.Value;
                    }

                    string hour12Pattern = @"1[0-9]|[0-9]";
                    Regex hour12Regex = new Regex(hour12Pattern);
                    string hour12Found = "";
                    foreach (Match match in hour12Regex.Matches(newText))
                    {
                        hour12Found = match.Value;
                    }

                    string hour24Pattern = @"([01]?[0-9]|2[0-3]):[0-5][0-9]";
                    Regex hour24Regex = new Regex(hour24Pattern);
                    string hour24Found = "";
                    foreach (Match match in hour24Regex.Matches(newText))
                    {
                        hour24Found = match.Value;
                    }

                    int TypeNum = EventsTypes
                        .Where(x => x.Item1 == typeFound)
                        .Select(x => x.Item2)
                        .SingleOrDefault();

                    if (!string.IsNullOrWhiteSpace(typeFound) && TypeNum != 0 && !string.IsNullOrWhiteSpace(dayFound) && (!string.IsNullOrWhiteSpace(hour12Found) || !string.IsNullOrWhiteSpace(hour24Found)))
                    {
                        string morningEvening = !string.IsNullOrWhiteSpace(morningEveningFound) ? (morningEveningFound == "πρωί" ? "am" : "pm") : "pm";
                        string morningEveningSpeech = !string.IsNullOrWhiteSpace(morningEveningFound) && morningEveningFound == "πρωί" ? "πρωί" : "απόγευμα";
                        string hour = !string.IsNullOrWhiteSpace(hour24Found) ? hour24Found : hour12Found + morningEvening;
                        string hourSpeech = !string.IsNullOrWhiteSpace(hour24Found) ? hour24Found : hour12Found;
                        int DayNum = DaysNumbers
                                        .Where(x => x.Item1 == dayFound)
                                        .Select(x => x.Item2)
                                        .Single();
                        CultureInfo myCI = new CultureInfo("el-GR");
                        DateTime DesiredDay = GetNextWeekday(DateTime.Today, DayNum);
                        DateTime DesiredDayAndTime = new DateTime(DesiredDay.Year, DesiredDay.Month, DesiredDay.Day, DateTime.Parse(hour).Hour, DateTime.Parse(hour).Minute, 0);

                        var page = Navigation.NavigationStack.Last();
                        bool isShowingAddNewPage = page != null && page is AddNewPage;
                        if (!isShowingAddNewPage)
                        {
                            Task.Run(async () => await SpeakNow($"{typeFound} την {dayFound} στις {DesiredDayAndTime:dd MM} και ώρα {hourSpeech} το {morningEveningSpeech}."));
                            MySQLDataStore.Instance.WeekViewPageViewModel.VAAddNewEventCommand.Execute(new Meeting(DesiredDayAndTime, DesiredDayAndTime.AddHours(1), TypeNum));
                        }
                    }
                }
                
                //TranscribeClicked(null, null);
            });
        }

        //void InsertDateTimeRecord()
        //{
        //    var msg = $"=================\n{DateTime.Now}\n=================";
        //    UpdateTranscription(msg);
        //}

        //void UpdateDisplayState()
        //{
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        if (isTranscribing)
        //        {
        //            transcribeButton.Text = "Stop";
        //            transcribeButton.BackgroundColor = Color.Red;
        //            transcribingIndicator.IsRunning = true;
        //        }
        //        else
        //        {
        //            transcribeButton.Text = "Transcribe";
        //            transcribeButton.BackgroundColor = Color.Green;
        //            transcribingIndicator.IsRunning = false;
        //        }
        //    });
        //}

        public static DateTime GetNextWeekday(DateTime start, int day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = (day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe(this, "events", (object obj, string events) =>
            {
                if (events == "refresh")
                {
                    Device.BeginInvokeOnMainThread(() => {
                        _ = MySQLDataStore.Instance.WeekViewPageViewModel.ExecuteLoadDataCommand(); 
                        MySQLDataStore.Instance.WeekViewPageViewModel.ExecuteGetUserAppointmentsCommand.Execute(null);
                        EventsListDataService.Instance.EventsListViewModel.RefreshListCommand.Execute(null);
                    });
                }
            });

            TranscribeClicked(null, null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<string>(this, "events");
            Task.Run(async () => await recognizer.StopContinuousRecognitionAsync());
        }
    }
}