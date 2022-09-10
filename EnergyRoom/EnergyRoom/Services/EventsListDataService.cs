using EnergyRoom.ViewModels;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using Xamarin.Forms.Internals;

namespace EnergyRoom.Services
{
    /// <summary>
    /// Data service for task notification page to load the data from json file.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class EventsListDataService
    {
        #region fields 

        private static EventsListDataService instance;

        private EventsListViewModel eventsListViewModel;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="EventsListDataService"/>.
        /// </summary>
        public static EventsListDataService Instance => instance ??= new EventsListDataService();

        /// <summary>
        /// Gets or sets the value of events list page view model.
        /// </summary>
        //public EventsListViewModel EventsListViewModel => eventsListViewModel ??= PopulateData<EventsListViewModel>("notification.json");
        public EventsListViewModel EventsListViewModel => eventsListViewModel ??= new EventsListViewModel();

        #endregion

        #region Methods

        /// <summary>
        /// Populates the data for view model from json file.
        /// </summary>
        /// <typeparam name="T">Type of view model.</typeparam>
        /// <param name="fileName">Json file to fetch data.</param>
        /// <returns>Returns the view model object.</returns>
        private static T PopulateData<T>(string fileName)
        {
            var file = "EnergyRoom.Data." + fileName;

            var assembly = typeof(App).GetTypeInfo().Assembly;

            T obj;

            //using (var stream = assembly.GetManifestResourceStream(file))
            //{
            //    var serializer = new DataContractJsonSerializer(typeof(T));
            //    obj = (T)serializer.ReadObject(stream);
            //}

            using (StreamReader json = new StreamReader(assembly.GetManifestResourceStream(file)))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                obj = (T)serializer.Deserialize(json, typeof(T));
            }

            return obj;
        }

        #endregion
    }
}