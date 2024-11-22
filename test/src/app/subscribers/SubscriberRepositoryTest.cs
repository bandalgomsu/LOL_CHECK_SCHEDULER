using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using lol_check_scheduler.src.app.subscribers.repository;
using Xunit;

namespace test.src.app.subscribers
{
    public class SubscriberRepositoryTest
    {
        private readonly DatabaseContext _databaseContext;
        private readonly SubscriberRepository _repository;

        public SubscriberRepositoryTest()
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
                var subscriber = new Subscriber
                {
                    SummonerId = i,
                    SubscriberId = i,
                    SummonerGameName = "TEST_GAME_NAME",
                    SummonerTagLine = "TEST_TAG_LINE",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _databaseContext.Set<Subscriber>().Add(subscriber);
                _databaseContext.SaveChanges();
            }
            _repository = new SubscriberRepository(_databaseContext);
        }

        [Fact(DisplayName = "FIND_ALL_SUCCESS")]
        public async Task FIND_ALL_SUCCESS()
        {
            var subscribers = await _repository.FindAll();

            Assert.Equal(50, subscribers.Count());
        }

        [Fact(DisplayName = "FIND_ALL_BY_CONDITION_SUMMONER_GAME_NAME_SUCCESS")]
        public async Task FIND_ALL_BY_CONDITION_SUMMONER_GAME_NAME_SUCCESS()
        {
            var subscribers = await _repository.FindAllByCondition(subscriber => subscriber.SummonerGameName == "TEST_GAME_NAME");

            Assert.Equal(50, subscribers.Count());
        }

        [Fact(DisplayName = "FIND_BY_CONDITION_ID_SUCCESS")]
        public async Task FIND_BY_CONDITION_ID_SUCCESS()
        {
            var subscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);

            Assert.Equal(1, subscriber!.Id);
        }

        [Fact(DisplayName = "CREATE_SUCCESS")]
        public async Task CREATE_SUCCESS()
        {
            var subscriber = new Subscriber
            {
                SummonerId = 51,
                SubscriberId = 51,
                SummonerGameName = "CREATE_GAME_NAME",
                SummonerTagLine = "CREATE_TAG_LINE",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _repository.Create(subscriber);

            var subscribers = await _repository.FindAll();

            Assert.Equal(51, subscribers.Count());
        }

        [Fact(DisplayName = "PATCH_SUCCESS")]
        public async Task PATCH_SUCCESS()
        {
            var subscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);
            subscriber!.SummonerGameName = "PATCH";

            await _repository.Patch(subscriber);

            var updatedsubscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);

            Assert.Equal("PATCH", updatedsubscriber!.SummonerGameName);
        }

        [Fact(DisplayName = "UPDATE_SUCCESS")]
        public async Task UPDATE_SUCCESS()
        {
            var subscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);
            subscriber!.SummonerGameName = "UPDATE";

            await _repository.Patch(subscriber);

            var updatedsubscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);

            Assert.Equal("UPDATE", updatedsubscriber!.SummonerGameName);
        }

        [Fact(DisplayName = "DELETE_SUCCESS")]
        public async Task DELETE_SUCCESS()
        {
            var subscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);

            await _repository.Delete(subscriber!);

            var subscribers = await _repository.FindAll();

            Assert.Equal(49, subscribers.Count());
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();
        }
    }
}