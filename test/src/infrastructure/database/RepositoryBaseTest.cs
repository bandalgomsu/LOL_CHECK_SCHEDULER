using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using Moq;
using Xunit;

namespace test.src.infrastructure.database
{
    public class TestRepository : RepositoryBase<Summoner>
    {
        public TestRepository(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }
    }

    public class RepositoryBaseTest : IDisposable
    {
        private readonly DatabaseContext _databaseContext;
        private readonly RepositoryBase<Summoner> _repositoryBase;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactory = new Mock<IServiceScopeFactory>();
        private readonly Mock<IServiceScope> _serviceScope = new Mock<IServiceScope>();
        private readonly Mock<IServiceProvider> _serviceProvider = new Mock<IServiceProvider>();

        public RepositoryBaseTest()
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
            // _databaseContext.Database.Migrate();

            for (int i = 0; i < 50; i++)
            {
                var summoner = new Summoner
                {
                    Puuid = i.ToString() + ": PUUID",
                    GameName = i.ToString() + ": GAME_NAME",
                    TagLine = "TAG_LINE",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                _databaseContext.Set<Summoner>().Add(summoner);
                _databaseContext.SaveChanges();
            }

            _serviceProvider.Setup(provider => provider.GetService(typeof(DatabaseContext))).Returns(_databaseContext);
            _serviceScope.SetupGet(scope => scope.ServiceProvider).Returns(_serviceProvider.Object);

            var asyncServiceScope = new AsyncServiceScope(_serviceScope.Object);
            _serviceScopeFactory.Setup(factory => factory.CreateScope()).Returns(_serviceScope.Object);

            _repositoryBase = new TestRepository(_serviceScopeFactory.Object);
        }

        [Fact(DisplayName = "FIND_ALL_SUCCESS")]
        public async Task FIND_ALL_SUCCESS()
        {
            var summoners = await _repositoryBase.FindAll();

            Assert.Equal(50, summoners.Count());
        }

        [Fact(DisplayName = "FIND_ALL_BY_CONDITION_TAG_LINE_SUCCESS")]
        public async Task FIND_ALL_BY_CONDITION_TAG_LINE_SUCCESS()
        {
            var summoners = await _repositoryBase.FindAllByCondition(summoner => summoner.TagLine == "TAG_LINE");

            Assert.Equal(50, summoners.Count());
        }

        [Fact(DisplayName = "FIND_BY_CONDITION_ID_SUCCESS")]
        public async Task FIND_BY_CONDITION_ID_SUCCESS()
        {
            var summoner = await _repositoryBase.FindByCondition(summoner => summoner.Id == 1);

            Assert.Equal(1, summoner!.Id);
        }

        [Fact(DisplayName = "CREATE_SUCCESS")]
        public async Task CREATE_SUCCESS()
        {
            var summoner = new Summoner
            {
                Puuid = "CREATE",
                GameName = "CREATE",
                TagLine = "CREATE",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _repositoryBase.Create(summoner);

            var summoners = await _repositoryBase.FindAll();

            Assert.Equal(51, summoners.Count());
        }

        [Fact(DisplayName = "PATCH_SUCCESS")]
        public async Task PATCH_SUCCESS()
        {
            var summoner = await _repositoryBase.FindByCondition(summoner => summoner.Id == 1);
            summoner!.Puuid = "PATCH";

            await _repositoryBase.Patch(summoner);

            var updatedSummoner = await _repositoryBase.FindByCondition(summoner => summoner.Id == 1);

            Assert.Equal("PATCH", updatedSummoner!.Puuid);
        }

        [Fact(DisplayName = "UPDATE_SUCCESS")]
        public async Task UPDATE_SUCCESS()
        {
            var summoner = await _repositoryBase.FindByCondition(summoner => summoner.Id == 1);
            summoner!.Puuid = "UPDATE";

            await _repositoryBase.Update(summoner);

            var updatedSummoner = await _repositoryBase.FindByCondition(summoner => summoner.Id == 1);

            Assert.Equal("UPDATE", updatedSummoner!.Puuid);
        }

        [Fact(DisplayName = "DELETE_SUCCESS")]
        public async Task DELETE_SUCCESS()
        {
            var summoner = await _repositoryBase.FindByCondition(summoner => summoner.Id == 1);

            await _repositoryBase.Delete(summoner!);

            var summoners = await _repositoryBase.FindAll();

            Assert.Equal(49, summoners.Count());
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();
        }
    }
}