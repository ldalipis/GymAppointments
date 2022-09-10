using System.Collections.Generic;
using System.Threading.Tasks;
using EnergyRoom.Models;

namespace EnergyRoom.Services
{
    public interface IDataStore<T>
    {
        Task<long> AddItemAsync(Meeting meeting);
        Task<bool> UpdateItemAsync(T newitem);
        Task<bool> DeleteItemAsync(int id);
        Task<Meeting> GetItemAsync(int id);
        Task<IEnumerable<Meeting>> GetItemsAsync(bool forceRefresh = false);
        Task<long> UserSignUpAsync(string name, string phone, string email, string username, string password);
        Task<byte[]> GetUserPhoto(int id);
        Task<bool> UploadPhoto(int userId, byte[] img, int imgLength);
        Task<bool> ChangePhone(int userId, string phone);
        Task<string> FindEmail(string cred);
        Task<bool> CheckExistingEmail(string email);
        Task<bool> RequestChangeEmail(int userId, string email, string newEmail, string token);
        Task<int> ChangeEmail(int userId, string token);
        Task<Users> UserLoginFBAsync(string email);
        Task<Users> UserLoginAsync(string email, string password);
        Task<IEnumerable<EventTypes>> GetEventTypesAsync();
        Task<long> AddAppointmentAsync(int eventId, int userId);
        Task<bool> DeleteAppointmentAsync(long id);
        Task<IEnumerable<Appointments>> GetEventAppointmentsAsync(int eventId);
        Task<IEnumerable<Appointments>> GetAdminAppointmentsAsync();
        Task<IEnumerable<Appointments>> GetUserAppointmentsAsync();
    }
}
