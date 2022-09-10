using EnergyRoom.Helpers;
using EnergyRoom.Models;
using EnergyRoom.ViewModels;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EnergyRoom.Services
{
    public class MySQLDataStore : IDataStore<Item>
    {
        private List<Meeting> items;
        public List<EventTypes> evTypes;
        public Users User { get; set; }
        public ObservableCollection<Meeting> Data { get; set; }

        private static MySQLDataStore mySQLDataStore;
        public static MySQLDataStore Instance => mySQLDataStore ??= new MySQLDataStore();

        private WeekViewPageViewModel weekViewPageViewModel;
        public WeekViewPageViewModel WeekViewPageViewModel => weekViewPageViewModel ??= new WeekViewPageViewModel();

        private AddNewPageViewModel addNewPageViewModel;
        public AddNewPageViewModel AddNewPageViewModel => addNewPageViewModel ??= new AddNewPageViewModel();
        
        private static readonly string ConnectionString = "Server=95.216.9.112;User ID=malonslaught_nrgl;Password=o2$eKx02;Database=malonslaught_nrgapp";

        public MySQLDataStore()
        {
            items = new List<Meeting>();
            Data = new ObservableCollection<Meeting>();
            User = new Users();

            Task.Run(async () => await GetItemsAsync());
            Task.Run(async () => await GetEventTypesAsync());
        }

        public async Task<long> AddItemAsync(Meeting meeting)
        {
            //items.Add(item);

            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("INSERT INTO `events` (`id`, `date`, `starttime`, `endtime`, `typeid`, `userid`, `eventenabled`) VALUES (NULL, ?date, ?starttime, ?endtime, ?typeid, ?userid, 1);", connection);
            command.Parameters.Add("?date", MySqlDbType.DateTime).Value = meeting.StartTime;
            command.Parameters.Add("?starttime", MySqlDbType.DateTime).Value = meeting.StartTime;
            command.Parameters.Add("?endtime", MySqlDbType.DateTime).Value = meeting.EndTime;
            command.Parameters.Add("?typeid", MySqlDbType.Int32).Value = meeting.TypeId;
            command.Parameters.Add("?userid", MySqlDbType.Int32).Value = meeting.UserId;

            command.ExecuteNonQuery();

            long newId = command.LastInsertedId;

            connection.Close();

            return await Task.FromResult(newId);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            //var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            //items.Remove(oldItem);
            //items.Add(item);

            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("UPDATE `events` SET `date` = ?date, `typeid` = ?typeid WHERE id = ?id;", connection);
            command.Parameters.Add("?date", MySqlDbType.DateTime).Value = Convert.ToDateTime(item.Date);
            command.Parameters.Add("?typeid", MySqlDbType.Int32).Value = item.TypeId;
            command.Parameters.Add("?id", MySqlDbType.Int32).Value = item.Id;

            command.ExecuteNonQuery();

            connection.Close();

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            //var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            //items.Remove(oldItem);

            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("DELETE FROM `events` WHERE `id` = ?id;", connection);
            command.Parameters.Add("?id", MySqlDbType.Int32).Value = id;

            command.ExecuteNonQuery();

            connection.Close();

            return await Task.FromResult(true);
        }

        public async Task<Meeting> GetItemAsync(int id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.EventId == id));
        }

        public async Task<IEnumerable<Meeting>> GetItemsAsync(bool forceRefresh = false)
        {
            items.Clear();
            Data.Clear();

            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT e.*, count(a.id) as eventcount, t.*, u.* FROM `events` e LEFT JOIN `appointments` a ON e.id = a.eventid LEFT JOIN `types` t ON e.typeid = t.id INNER JOIN `users` u ON e.userid = u.id GROUP BY e.id;", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Meeting(reader.GetInt32("id"), reader.GetInt32("typeid"), reader.GetInt32("userid"), reader.GetString("fullname"), reader.GetString("username"), DateTime.Parse(reader.GetDateTime("starttime").ToString()), DateTime.Parse(reader.GetDateTime("endtime").ToString()), reader.GetInt32("eventcount")));
                Data.Add(new Meeting(reader.GetInt32("id"), reader.GetInt32("typeid"), reader.GetInt32("userid"), reader.GetString("fullname"), reader.GetString("username"), DateTime.Parse(reader.GetDateTime("starttime").ToString()), DateTime.Parse(reader.GetDateTime("endtime").ToString()), reader.GetInt32("eventcount")));
            }

            connection.Close();

            return await Task.FromResult(Data);
        }

        public async Task<byte[]> GetUserPhoto(int id = 0)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT `photo`, `photosize` FROM `users` WHERE id = ?id;", connection);
            command.Parameters.Add("?id", MySqlDbType.Int32).Value = id != 0 ? id : App._userId;

            byte[] rawData;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();

                uint FileSize = reader.GetUInt32(reader.GetOrdinal("photosize"));
                rawData = new byte[FileSize];

                reader.GetBytes(reader.GetOrdinal("photo"), 0, rawData, 0, (int)FileSize);

                connection.Close();
            }

            return await Task.FromResult(rawData);
        }

        public async Task<bool> UploadPhoto(int userId, byte[] img, int imgLength)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("UPDATE `users` SET `photo` = ?userImage, `photosize` = ?photoSize WHERE id = ?id;", connection);
            command.Parameters.AddWithValue("?userImage", img);
            command.Parameters.Add("?photoSize", MySqlDbType.UInt32, 256).Value = imgLength;
            command.Parameters.Add("?id", MySqlDbType.Int32).Value = userId;

            command.ExecuteNonQuery();

            connection.Close();

            return await Task.FromResult(true);
        }

        public async Task<bool> ChangePhone(int userId, string phone)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("UPDATE `users` SET `phone` = ?phone WHERE id = ?userId;", connection);
            command.Parameters.Add("?userId", MySqlDbType.Int32).Value = userId;
            command.Parameters.Add("?phone", MySqlDbType.VarChar).Value = phone;

            command.ExecuteNonQuery();

            connection.Close();

            return await Task.FromResult(true);
        }

        public async Task<string> FindEmail(string cred)
        {
            using var emailconnection = new MySqlConnection(ConnectionString);
            emailconnection.Open();

            using var checkemailcommand = new MySqlCommand("SELECT `email` FROM `users` WHERE username = ?cred OR `email` = ?cred;", emailconnection);
            checkemailcommand.Parameters.Add("?cred", MySqlDbType.VarChar).Value = cred;

            using var emailreader = checkemailcommand.ExecuteReader();
            emailreader.Read();

            string email = "";
            if (emailreader.HasRows)
            {
                email = emailreader.GetString(emailreader.GetOrdinal("email"));
            }

            emailconnection.Close();

            return await Task.FromResult(email);
        }


        public async Task<bool> CheckExistingEmail(string email)
        {
            using var emailconnection = new MySqlConnection(ConnectionString);
            emailconnection.Open();

            using var checkemailcommand = new MySqlCommand("SELECT count(*) as `exists` FROM `users` WHERE email = ?email;", emailconnection);
            checkemailcommand.Parameters.Add("?email", MySqlDbType.VarChar).Value = email;

            using var emailreader = checkemailcommand.ExecuteReader();
            emailreader.Read();

            int exists = emailreader.GetInt32(emailreader.GetOrdinal("exists"));

            emailconnection.Close();

            return await Task.FromResult(exists > 0);
        }

        public async Task<bool> RequestChangeEmail(int userId, string email, string newEmail, string token)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("UPDATE `users` SET `changeemailtoken` = ?token, `changeemailnewemail` = ?newEmail WHERE id = ?userid AND `email` = ?email;", connection);
            command.Parameters.Add("?token", MySqlDbType.VarChar).Value = token;
            command.Parameters.Add("?newEmail", MySqlDbType.VarChar).Value = newEmail;
            command.Parameters.Add("?userid", MySqlDbType.Int32).Value = userId;
            command.Parameters.Add("?email", MySqlDbType.VarChar).Value = email;

            command.ExecuteNonQuery();

            connection.Close();

            return await Task.FromResult(true);
        }

        public async Task<int> ChangeEmail(int userId, string token)
        {
            //if (Task.Run(async() => await CheckExistingEmail(email)).Result)
            //{
            //    return await Task.FromResult(-1);
            //}

            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("UPDATE `users` SET `email` = `changeemailnewemail`, `changeemailtoken` = '', `changeemailnewemail` = '' WHERE id = ?userId AND `changeemailtoken` = ?token;", connection);
            command.Parameters.Add("?userId", MySqlDbType.Int32).Value = userId;
            command.Parameters.Add("?token", MySqlDbType.VarChar).Value = token;

            command.ExecuteNonQuery();

            connection.Close();

            return await Task.FromResult(0);
        }

        public async Task<long> UserSignUpAsync(string name, string phone, string email, string username, string password)
        {
            using var emailconnection = new MySqlConnection(ConnectionString);
            emailconnection.Open();

            using var checkemailcommand = new MySqlCommand("SELECT count(*) as `exists` FROM `users` WHERE email = ?email;", emailconnection);
            checkemailcommand.Parameters.Add("?email", MySqlDbType.VarChar).Value = email;

            using var emailreader = checkemailcommand.ExecuteReader();
            emailreader.Read();

            int exists = emailreader.GetInt32(emailreader.GetOrdinal("exists"));

            emailconnection.Close();

            if (exists > 0)
            {
                return await Task.FromResult(-1);
            }

            using var usernameconnection = new MySqlConnection(ConnectionString);
            usernameconnection.Open();

            using var checkusernamecommand = new MySqlCommand("SELECT count(*) as `exists` FROM `users` WHERE username = ?username;", usernameconnection);
            checkusernamecommand.Parameters.Add("?username", MySqlDbType.VarChar).Value = username;

            using var usernamereader = checkusernamecommand.ExecuteReader();
            usernamereader.Read();

            exists = usernamereader.GetInt32(usernamereader.GetOrdinal("exists"));

            usernameconnection.Close();

            if (exists > 0)
            {
                return await Task.FromResult(-2);
            }

            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("INSERT INTO `users` (`id`, `fullname`, `phone`, `email`, `username`, `password`, `usertypeid`) VALUES (NULL, ?fullname, ?phone, ?email, ?username, ?password, '2');", connection);
            command.Parameters.Add("?fullname", MySqlDbType.VarChar).Value = name;
            command.Parameters.Add("?phone", MySqlDbType.VarChar).Value = phone;
            command.Parameters.Add("?email", MySqlDbType.VarChar).Value = email;
            command.Parameters.Add("?username", MySqlDbType.VarChar).Value = username;
            command.Parameters.Add("?password", MySqlDbType.VarChar).Value = Encryption.Sha256Hash(password);

            command.ExecuteNonQuery();

            long newId = command.LastInsertedId;

            connection.Close();

            return await Task.FromResult(newId);
        }

        public async Task<Users> UserLoginAsync(string email, string password)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM `users` as u left join `usertypes` as t on u.usertypeid = t.typeid WHERE `email` = ?email AND `password` = ?password;", connection);
            command.Parameters.Add("?email", MySqlDbType.VarChar).Value = email;
            command.Parameters.Add("?password", MySqlDbType.VarChar).Value = Encryption.Sha256Hash(password);

            using var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    User.ID = reader.GetInt32("id");
                    User.Name = reader.GetString("fullname");
                    User.Phone = reader.GetString("phone");
                    User.Email = reader.GetString("email");
                    User.Username = reader.GetString("username");
                    User.UserTypeId = reader.GetInt32("usertypeid");
                    User.UserTypeDescr = reader.GetString("typedescr");
                }
            }
            else
            {
                User.ID = 0;
            }
            connection.Close();

            return await Task.FromResult(User);
        }

        public async Task<Users> UserLoginFBAsync(string email)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM `users` as u left join `usertypes` as t on u.usertypeid = t.typeid WHERE `email` = ?email;", connection);
            command.Parameters.Add("?email", MySqlDbType.VarChar).Value = email;

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                User.ID = reader.GetInt32("id");
                User.Name = reader.GetString("fullname");
                User.Phone = reader.GetString("phone");
                User.Email = reader.GetString("email");
                User.Username = reader.GetString("username");
                User.UserTypeId = reader.GetInt32("usertypeid");
                User.UserTypeDescr = reader.GetString("typedescr");
            }

            connection.Close();

            return await Task.FromResult(User);
        }

        public async Task<IEnumerable<EventTypes>> GetEventTypesAsync()
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM `types`;", connection);

            evTypes = new List<EventTypes>();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                evTypes.Add(new EventTypes() { ID = reader.GetInt32("id"), Name = reader.GetString("title").ToString(), TypeEnabled = reader.GetBoolean("typeenabled") });
            }

            connection.Close();

            return await Task.FromResult(evTypes);
        }

        public async Task<long> AddAppointmentAsync(int eventId, int userId)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("INSERT INTO `appointments` (`id`, `eventid`, `userid`) VALUES (NULL, ?eventid, ?userid);", connection);
            command.Parameters.Add("?eventid", MySqlDbType.Int32).Value = eventId;
            command.Parameters.Add("?userid", MySqlDbType.Int32).Value = userId;

            command.ExecuteNonQuery();
            long newId = command.LastInsertedId;

            connection.Close();

            return await Task.FromResult(newId);
        }

        public async Task<bool> DeleteAppointmentAsync(long id)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("DELETE FROM `appointments` WHERE `id` = ?id;", connection);
            command.Parameters.Add("?id", MySqlDbType.Int32).Value = id;

            command.ExecuteNonQuery();

            connection.Close();

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Appointments>> GetEventAppointmentsAsync(int eventId)
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM `appointments` a LEFT JOIN `users` u ON a.userid=u.id WHERE a.eventid = ?eventid;", connection);
            command.Parameters.Add("?eventid", MySqlDbType.Int32).Value = eventId;

            var eventAppointments = new List<Appointments>();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                eventAppointments.Add(new Appointments() { ID = reader.GetInt32("id"), EventId = reader.GetInt32("eventid"), UserId = reader.GetInt32("userid"), UserFullname = reader.GetString("fullname"), UserPhone = reader.GetString("phone") });
            }

            connection.Close();

            return await Task.FromResult(eventAppointments);
        }

        public async Task<IEnumerable<Appointments>> GetAdminAppointmentsAsync()
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM `events` WHERE `userid` = ?userid;", connection);
            command.Parameters.Add("?userid", MySqlDbType.Int32).Value = App._userId;

            var userAppointments = new List<Appointments>();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                userAppointments.Add(new Appointments() { ID = reader.GetInt32("id"), EventId = reader.GetInt32("id"), UserId = reader.GetInt32("userid") });
            }

            connection.Close();

            return await Task.FromResult(userAppointments);
        }

        public async Task<IEnumerable<Appointments>> GetUserAppointmentsAsync()
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT * FROM `appointments` WHERE `userid` = ?userid;", connection);
            command.Parameters.Add("?userid", MySqlDbType.Int32).Value = App._userId;

            var userAppointments = new List<Appointments>();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                userAppointments.Add(new Appointments() { ID = reader.GetInt32("id"), EventId = reader.GetInt32("eventid"), UserId = reader.GetInt32("userid") });
            }

            connection.Close();

            return await Task.FromResult(userAppointments);
        }

    }
}