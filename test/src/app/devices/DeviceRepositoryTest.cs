using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using lol_check_scheduler.src.app.devices.repository;
using Xunit;

namespace test.src.app.devices
{
    public class DeviceRepositoryTest
    {
        private readonly DatabaseContext _databaseContext;
        private readonly DeviceRepository _repository;

        public DeviceRepositoryTest()
        {
            var env = DotEnv.Read(new DotEnvOptions(overwriteExistingVars: true, envFilePaths: ["../../../.env"]));

            var host = env["TEST_DB_HOST"];
            var name = $"lol_test_{Guid.NewGuid()}";
            var username = env["TEST_DB_USERNAME"];
            var password = env["TEST_DB_PASSWORD"];

            var connection = $"Server={host};Database={name};User={username};Password={password}";

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseMySQL(connection)
                .Options;

            _databaseContext = new DatabaseContext(options);
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Database.EnsureCreated();

            for (int i = 0; i < 50; i++)
            {
                var device = new Device
                {
                    DeviceToken = "TEST",
                    UserId = i,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _databaseContext.Set<Device>().Add(device);
                _databaseContext.SaveChanges();
            }
            _repository = new DeviceRepository(_databaseContext);
        }

        [Fact(DisplayName = "FIND_ALL_SUCCESS")]
        public async Task FIND_ALL_SUCCESS()
        {
            var devices = await _repository.FindAll();

            Assert.Equal(50, devices.Count());
        }

        [Fact(DisplayName = "FIND_ALL_BY_CONDITION_DEVICE_TOKEN_SUCCESS")]
        public async Task FIND_ALL_BY_CONDITION_DEVICE_TOKEN_SUCCESS()
        {
            var devices = await _repository.FindAllByCondition(device => device.DeviceToken == "TEST");

            Assert.Equal(50, devices.Count());
        }

        [Fact(DisplayName = "FIND_BY_CONDITION_ID_SUCCESS")]
        public async Task FIND_BY_CONDITION_ID_SUCCESS()
        {
            var device = await _repository.FindByCondition(device => device.Id == 1);

            Assert.Equal(1, device!.Id);
        }

        [Fact(DisplayName = "CREATE_SUCCESS")]
        public async Task CREATE_SUCCESS()
        {
            var device = new Device
            {
                DeviceToken = "CREATE",
                UserId = 5000,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _repository.Create(device);

            var devices = await _repository.FindAll();

            Assert.Equal(51, devices.Count());
        }

        [Fact(DisplayName = "PATCH_SUCCESS")]
        public async Task PATCH_SUCCESS()
        {
            var device = await _repository.FindByCondition(device => device.Id == 1);
            device!.DeviceToken = "PATCH";

            await _repository.Patch(device);

            var updatedDevice = await _repository.FindByCondition(device => device.Id == 1);

            Assert.Equal("PATCH", updatedDevice!.DeviceToken);
        }

        [Fact(DisplayName = "UPDATE_SUCCESS")]
        public async Task UPDATE_SUCCESS()
        {
            var device = await _repository.FindByCondition(device => device.Id == 1);
            device!.DeviceToken = "UPDATE";

            await _repository.Patch(device);

            var updatedDevice = await _repository.FindByCondition(device => device.Id == 1);

            Assert.Equal("UPDATE", updatedDevice!.DeviceToken);
        }

        [Fact(DisplayName = "DELETE_SUCCESS")]
        public async Task DELETE_SUCCESS()
        {
            var device = await _repository.FindByCondition(device => device.Id == 1);

            await _repository.Delete(device!);

            var devices = await _repository.FindAll();

            Assert.Equal(49, devices.Count());
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();
        }
    }
}