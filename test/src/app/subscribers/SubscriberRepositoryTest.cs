using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using lol_check_scheduler.src.app.subscribers.repository;
using Moq;
using Xunit;

namespace test.src.app.subscribers
{
    public class SubscriberRepositoryTest : IDisposable
    {
        private readonly DatabaseContext _databaseContext;
        private readonly SubscriberRepository _repository;

        private readonly Mock<IServiceScopeFactory> _serviceScopeFactory = new Mock<IServiceScopeFactory>();
        private readonly Mock<IServiceScope> _serviceScope = new Mock<IServiceScope>();
        private readonly Mock<IServiceProvider> _serviceProvider = new Mock<IServiceProvider>();

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
                    SubscriberId = 50,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _databaseContext.Set<Subscriber>().Add(subscriber);
                _databaseContext.SaveChanges();
            }

            _serviceProvider.Setup(provider => provider.GetService(typeof(DatabaseContext))).Returns(_databaseContext);
            _serviceScope.SetupGet(scope => scope.ServiceProvider).Returns(_serviceProvider.Object);

            var asyncServiceScope = new AsyncServiceScope(_serviceScope.Object);
            _serviceScopeFactory.Setup(factory => factory.CreateScope()).Returns(_serviceScope.Object);

            _repository = new SubscriberRepository(_serviceScopeFactory.Object);
        }

        [Fact(DisplayName = "FIND_ALL_SUCCESS")]
        public async Task FIND_ALL_SUCCESS()
        {
            var subscribers = await _repository.FindAll();

            Assert.Equal(50, subscribers.Count());
        }

        [Fact(DisplayName = "FIND_ALL_BY_CONDITION_SUBSCRIBER_ID_SUCCESS")]
        public async Task FIND_ALL_BY_CONDITION_SUBSCRIBER_ID_SUCCESS()
        {
            var subscribers = await _repository.FindAllByCondition(subscriber => subscriber.SubscriberId == 50);

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
            subscriber!.SummonerId = 2;

            await _repository.Patch(subscriber);

            var updatedsubscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);

            Assert.Equal(2, updatedsubscriber!.SummonerId);
        }

        [Fact(DisplayName = "UPDATE_SUCCESS")]
        public async Task UPDATE_SUCCESS()
        {
            var subscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);
            subscriber!.SummonerId = 2;

            await _repository.Patch(subscriber);

            var updatedsubscriber = await _repository.FindByCondition(subscriber => subscriber.Id == 1);

            Assert.Equal(2, updatedsubscriber!.SummonerId);
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