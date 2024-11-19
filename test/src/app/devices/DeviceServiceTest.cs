using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.devices.repository;
using lol_check_scheduler.src.app.devices.repository.interfaces;
using lol_check_scheduler.src.app.devices.service;
using Moq;
using Xunit;

namespace test.src.app.devices
{
    public class DeviceServiceTest
    {
        private readonly Mock<IDeviceRepository> _deviceRepository = new Mock<IDeviceRepository>();
        private readonly DeviceService _deviceService;

        public DeviceServiceTest()
        {
            _deviceService = new DeviceService(_deviceRepository.Object);
        }

        [Fact(DisplayName = "GET_DEVICE_TOKENS_BY_USER_IDS")]
        public async Task GET_DEVICE_TOKENS_BY_USER_IDS_SUCCESS()
        {
            IEnumerable<long> userIds = [1, 2, 3];

            IEnumerable<Device> devices = [new Device{
                DeviceToken = "TEST",
                UserId = 1,
            }];

            _deviceRepository.Setup(repo => repo.FindAllByCondition(device => userIds.Contains(device.UserId)))
                .ReturnsAsync(devices);

            var response = await _deviceService.GetDeviceTokensByUserIds(userIds);

            Assert.Single(response);
            Assert.Equal("TEST", response.First());
        }
    }
}