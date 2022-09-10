using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnergyRoom.Services.Identity
{
    interface IIdentityService
    {
        Task<bool> VerifyRegistration();
    }
}
