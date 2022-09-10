using System.Threading.Tasks;

namespace EnergyRoom.Services
{
    public interface IMicrophoneService
    {
        Task<bool> GetPermissionAsync();
        void OnRequestPermissionResult(bool isGranted);

    }
}
