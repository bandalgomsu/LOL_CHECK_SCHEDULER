using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using lol_check_scheduler.src.infrastructure.riotclient;
using Moq;
using Xunit;

namespace test.src.infrastructure.riotclient
{
    public class RiotClientTest
    {
        private readonly RiotClient _riotClient;

        public RiotClientTest()
        {
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(config => config["RiotApiKey"])
                .Returns(DotEnv.Read(new DotEnvOptions(envFilePaths: ["../../../.env"]))["RIOT_API_KEY"]!);

            _riotClient = new RiotClient(new HttpClient(), mockConfiguration.Object);
        }

        [Fact(DisplayName = "GET_PUUID_SUCCESS")]
        public async Task GET_PUUID_SUCCESS()
        {
            var gameName = "1";
            var tagLine = "1";

            var response = await _riotClient.GetPuuid(gameName, tagLine);

            Assert.Equal(gameName, response.GameName);
            Assert.Equal(tagLine, response.TagLine);
            Assert.NotNull(response.Puuid);
        }

    }
}