using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using lol_check_scheduler.src.app.summoners.repository;
using Xunit;

namespace test.src.app.summoners
{
    public class SummonerRepositoryTest
    {
        private readonly DatabaseContext _databaseContext;
        private readonly SummonerRepository _summonerRepository;

        public SummonerRepositoryTest()
        {
            var env = DotEnv.Read(new DotEnvOptions(overwriteExistingVars: true, envFilePaths: ["../../../.env"]));

            var host = env["TEST_DB_HOST"];
            var name = env["TEST_DB_NAME"];
            var username = env["TEST_DB_USERNAME"];
            var password = env["TEST_DB_PASSWORD"];

            var connection = $"Server={host};Database={name};User={username};Password={password}";

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseMySQL(connection)
                .Options;

            _databaseContext = new DatabaseContext(options);
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Database.EnsureCreated();
            _databaseContext.Database.Migrate();

            _summonerRepository = new SummonerRepository(_databaseContext);

            for (int i = 0; i < 50; i++)
            {
                var summoner = new Summoner
                {
                    Puuid = i.ToString() + ": PUUID",
                    GameName = i.ToString() + ": GAME_NAME",
                    TagLine = i.ToString() + ": TAG_LINE",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _summonerRepository.Create(summoner).Wait();
            }
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();
        }



        [Fact(DisplayName = "FIND_ALL_BY_TOP_N_성공_테스트")]
        public async Task FIND_ALL_BY_TOP_N_성공_테스트()
        {
            var summoners = await _summonerRepository.FindAllByTopN(20);

            Assert.Equal(20, summoners.Count());
        }
    }
}