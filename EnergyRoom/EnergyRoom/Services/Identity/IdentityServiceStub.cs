using System;
using System.Threading.Tasks;

namespace EnergyRoom.Services.Identity
{
    class IdentityServiceStub : IIdentityService
    {
        public async Task<bool> VerifyRegistration()
        {
            var isLogged = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
            if (isLogged == "1" && Xamarin.Essentials.SecureStorage.GetAsync("userId").Result != null && Xamarin.Essentials.SecureStorage.GetAsync("userIsAdmin").Result != null)
            {
                App._userId = int.Parse(Xamarin.Essentials.SecureStorage.GetAsync("userId").Result);
                App._userIsAdmin = bool.Parse(Xamarin.Essentials.SecureStorage.GetAsync("userIsAdmin").Result);
                App._name = Xamarin.Essentials.SecureStorage.GetAsync("fullName").Result;
                App._userName = Xamarin.Essentials.SecureStorage.GetAsync("userName").Result;
                App._phone = Xamarin.Essentials.SecureStorage.GetAsync("userPhone").Result;
                App._email = Xamarin.Essentials.SecureStorage.GetAsync("userEmail").Result;

                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
    }
}
