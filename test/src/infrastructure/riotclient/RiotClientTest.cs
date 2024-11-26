using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using dotenv.net;
using lol_check_scheduler.src.common.exception;
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

        [Fact(DisplayName = "GET_PUUID_FAILURE_THROW_BY_NOT_FOUND_SUMMONER")]
        public async Task GET_PUUID_FAILURE_THROW_BY_NOT_FOUND_SUMMONER()
        {
            var gameName = "asd반달asd";
            var tagLine = "KR1";

            var exception = await Assert.ThrowsAnyAsync<BusinessException>(() => _riotClient.GetPuuid(gameName, tagLine));

            Assert.Equal("RC01", exception.ErrorCode.Code);
            Assert.Equal("RIOT_CLIENT_SUMMONER_NOT_FOUND", exception.ErrorCode.Message);
        }

        [Fact(DisplayName = "GET_PUUID_FAILURE_THROW_BY_EXTERNAL_ERROR")]
        public async Task GET_PUUID_FAILURE_THROW_BY_EXTERNAL_ERROR()
        {
            var gameName = "asd반달as!@$!@$!@$!@$!@$!@$@!%!@%!@#!@#!@#!@#!@$!@#!d";
            var tagLine = "KR1";

            var exception = await Assert.ThrowsAnyAsync<BusinessException>(() => _riotClient.GetPuuid(gameName, tagLine));

            Assert.Equal("RC02", exception.ErrorCode.Code);
            Assert.Equal("RIOT_CLIENT_EXTERNAL_ERROR", exception.ErrorCode.Message);
        }

        [Fact(DisplayName = "GET_CURRENT_GAME_INFO_SUCCESS_BY_NOT_PLAYED_SUMMONER")]
        public async Task GET_CURRENT_GAME_INFO_SUCCESS_BY_NOT_PLAYED_SUMMONER()
        {
            var gameName = "반달곰수";
            var tagLine = "KR1";

            var puuid = (await _riotClient.GetPuuid(gameName, tagLine)).Puuid;

            var response = await _riotClient.GetCurrentGameInfo(puuid);

            Assert.False(response.IsCurrentPlayingGame); // 실제로 게임 안하고 있어서 연산결과를 뒤집음
        }

        [Fact(DisplayName = "GET_CURRENT_GAME_INFO_FAILURE_BY_EXTERNAL_ERROR")]
        public async Task GET_CURRENT_GAME_INFO_FAILURE_BY_EXTERNAL_ERROR()
        {
            var puuid = "ASDASDASDASDASDQWF!@D!@D!@D!F";

            var exception = await Assert.ThrowsAnyAsync<BusinessException>(() => _riotClient.GetCurrentGameInfo(puuid));

            Assert.Equal("RC02", exception.ErrorCode.Code);
            Assert.Equal("RIOT_CLIENT_EXTERNAL_ERROR", exception.ErrorCode.Message);
        }

    }
}