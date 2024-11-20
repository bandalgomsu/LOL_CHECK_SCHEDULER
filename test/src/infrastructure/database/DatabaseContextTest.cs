using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using Xunit;

namespace test.src.infrastructure.database
{
    public class DatabaseContextTest : IDisposable
    {
        private readonly DatabaseContext _databaseContext;

        public DatabaseContextTest()
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
        }

        [Fact(DisplayName = "FIND_ALL_SUCCESS")]
        public async Task FIND_ALL_SUCCESS()
        {
            var summoners = await _databaseContext.Set<Summoner>().ToListAsync();

            Assert.Equal(50, summoners.Count());
        }

        [Fact(DisplayName = "FIND_ALL_BY_TAG_LINE_SUCCESS")]
        public async Task FIND_ALL_BY_TAG_LINE_SUCCESS()
        {
            var summoners = await _databaseContext.Set<Summoner>().Where(summoner => summoner.TagLine == "TAG_LINE").ToListAsync();

            Assert.Equal(50, summoners.Count());
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();
        }
    }
}