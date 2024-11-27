using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.summoners.repository;
using lol_check_scheduler.src.app.summoners.repository.interfaces;
using lol_check_scheduler.src.app.summoners.service;
using Moq;
using Xunit;

namespace test.src.app.summoners
{
    public class SummonerServiceTest
    {
        private readonly SummonerService _summonerService;
        private readonly Mock<ISummonerRepository> _summonerRepository = new Mock<ISummonerRepository>();

        public SummonerServiceTest()
        {
            _summonerService = new SummonerService(_summonerRepository.Object);
        }

        [Fact(DisplayName = "GET_SUMMONERS_BY_TOP_N_SUCCESS")]
        public async Task GET_SUMMONERS_BY_TOP_N_SUCCESS()
        {
            _summonerRepository.Setup(repo => repo.FindAllByTopN(2))
                .ReturnsAsync([new Summoner { }, new Summoner { }]);

            var response = await _summonerService.GetSummonersByTopN(2);

            Assert.Equal(2, response.Count());
        }

        [Fact(DisplayName = "UPDATE_SUMMONER_SUCCESS")]
        public async Task UPDATE_SUMMONER_SUCCESS()
        {
            var summoner = new Summoner
            {
                Id = 1,
                Puuid = "TEST",
                GameName = "TEST",
                TagLine = "TEST",
            };

            var updateSummoner = new Summoner
            {
                Id = 1,
                Puuid = "UPDATE",
                GameName = "UPDATE",
                TagLine = "TEST",
            };

            _summonerRepository.Setup(repo => repo.Update(summoner))
                .ReturnsAsync(updateSummoner);

            var response = await _summonerService.UpdateSummoner(summoner);

            Assert.Equal(updateSummoner.Puuid, response.Puuid);
            Assert.Equal(updateSummoner.GameName, response.GameName);
        }

        [Fact(DisplayName = "PATCH_SUMMONER_SUCCESS")]
        public async Task PATCH_SUMMONER_SUCCESS()
        {
            var summoner = new Summoner
            {
                Id = 1,
                Puuid = "TEST",
                GameName = "TEST",
                TagLine = "TEST",
            };

            var updateSummoner = new Summoner
            {
                Id = 1,
                Puuid = "UPDATE",
                GameName = "UPDATE",
                TagLine = "TEST",
            };

            _summonerRepository.Setup(repo => repo.Patch(summoner))
                .ReturnsAsync(updateSummoner);

            var response = await _summonerService.PatchSummoner(summoner);

            Assert.Equal(updateSummoner.Puuid, response.Puuid);
            Assert.Equal(updateSummoner.GameName, response.GameName);
        }

        [Fact(DisplayName = "GET_SUMMONER_BY_PUUID_SUCCESS")]
        public async Task GET_SUMMONER_BY_PUUID_SUCCESS()
        {
            var summoner = new Summoner
            {
                Id = 1,
                Puuid = "TEST",
                GameName = "TEST",
                TagLine = "TEST",
                Introduce = "TEST"
            };

            _summonerRepository.Setup(repo => repo.FindByCondition(summoner => summoner.Puuid == "TEST"))
                .ReturnsAsync(summoner);

            var response = await _summonerService.GetSummonerByPuuid("TEST");

            Assert.Equal(summoner.Id, response.Id);
            Assert.Equal(summoner.Puuid, response.Puuid);
            Assert.Equal(summoner.GameName, response.GameName);
            Assert.Equal(summoner.TagLine, response.TagLine);
            Assert.Equal(summoner.Introduce, response.Introduce);
        }
    }
}