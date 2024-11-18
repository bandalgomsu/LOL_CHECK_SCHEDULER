using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.devices.repository.interfaces;

namespace lol_check_scheduler.src.app.devices.repository
{
    public class DeviceRepository(DatabaseContext databaseContext) : RepositoryBase<Device>(databaseContext), IDeviceRepository
    {
    }
}