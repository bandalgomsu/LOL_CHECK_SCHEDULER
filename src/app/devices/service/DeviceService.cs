using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.devices.repository.interfaces;

namespace lol_check_scheduler.src.app.devices.service
{
    public class DeviceService
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceService(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public async Task<IEnumerable<String>> GetDeviceTokensByUserIds(IEnumerable<int> userIds)
        {
            var devices = await _deviceRepository.FindAllByCondition(device => userIds.Contains(device.UserId));

            return devices.Select(device => device.DeviceToken);
        }
    }
}