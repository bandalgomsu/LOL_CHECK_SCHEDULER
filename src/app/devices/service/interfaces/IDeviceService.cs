using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.app.devices.service.interfaces
{
    public interface IDeviceService
    {
        Task<IEnumerable<string?>> GetDeviceTokensByUserIds(IEnumerable<long> userIds);
    }
}