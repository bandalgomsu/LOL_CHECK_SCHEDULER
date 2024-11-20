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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());

            _riotClient = new RiotClient(mockHttpClientFactory.Object, mockConfiguration.Object);
        }

        [Fact(DisplayName = "GET_PUUID_SUCCESS")]
        public async Task GET_PUUID_SUCCESS()
        {
            var gameName = "반달곰수";
            var tagLine = "KR1";

            var response = await _riotClient.GetPuuid(gameName, tagLine);

            Assert.Equal(gameName, response.GameName);
            Assert.Equal(tagLine, response.TagLine);
            Assert.NotNull(response.Puuid);
        }

    }
}