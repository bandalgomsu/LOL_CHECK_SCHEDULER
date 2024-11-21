using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.devices.repository.interfaces;
using lol_check_scheduler.src.app.devices.service.interfaces;

namespace lol_check_scheduler.src.app.devices.service
{
    public class DeviceService(IDeviceRepository deviceRepository) : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository = deviceRepository;

        public async Task<IEnumerable<string>> GetDeviceTokensByUserIds(IEnumerable<long> userIds)
        {
            var devices = await _deviceRepository.FindAllByCondition(device => userIds.Contains(device.UserId));

            return devices.Select(device => device.DeviceToken!);
        }
    }
}