using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.job;
using lol_check_scheduler.src.app.summoners.exception;
using lol_check_scheduler.src.app.summoners.service.interfaces;
using lol_check_scheduler.src.common.exception;
using lol_check_scheduler.src.infrastructure.riotclient;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;
using Moq;
using Xunit;

namespace test.src.app.job
{
    public class WarmUpSummonerJobTest
    {
        private readonly Mock<IRiotClient> _riotClient = new Mock<IRiotClient>();
        private readonly Mock<ISummonerService> _summonerService = new Mock<ISummonerService>();

        private readonly WarmUpSummonerJob _warmUpSummonerJob;

        public WarmUpSummonerJobTest()
        {
            _warmUpSummonerJob = new WarmUpSummonerJob(_riotClient.Object, _summonerService.Object);
        }

        [Fact(DisplayName = "WARM_UP_SUMMONERS_SUCCESS")]
        public async void WARM_UP_SUMMONERS_SUCCESS()
        {
            var exist = "EXISTS_SUMMONER_IN_DB";
            var notExists = "NOT_EXISTS_SUMMONER_IN_DB";

            var existingLeagueItem = new RiotClientData.LeagueItemDTO
            {
                SummonerId = exist
            };

            var notExistingLeagueItemFailure = new RiotClientData.LeagueItemDTO
            {
                SummonerId = notExists
            };

            var leagueList = new RiotClientData.LeagueListDTO
            {
                Entries = new List<RiotClientData.LeagueItemDTO>
                {
                    existingLeagueItem,
                    notExistingLeagueItemFailure
                }
            };

            _riotClient.Setup(client => client.GetLeagueListInChallengerLeagues()).ReturnsAsync(leagueList);
            _riotClient.Setup(client => client.GetLeagueListInGrandMasterLeagues()).ReturnsAsync(leagueList);
            _riotClient.Setup(client => client.GetLeagueListInMasterLeagues()).ReturnsAsync(leagueList);

            var existsPuuid = exist + "PUUID";
            var notExistsPuuid = notExists + "PUUID";

            var existingSummonerInfo = new RiotClientData.GetSummonerInfoResponse
            {
                Puuid = existsPuuid
            };

            var notExistingSummonerInfo = new RiotClientData.GetSummonerInfoResponse
            {
                Puuid = notExistsPuuid
            };

            _riotClient.Setup(client => client.GetSummonerInfoBySummonerId(exist)).ReturnsAsync(existingSummonerInfo);
            _riotClient.Setup(client => client.GetSummonerInfoBySummonerId(notExists)).ReturnsAsync(notExistingSummonerInfo);

            var existingGameName = "EXIST_GAME_NAME";
            var existingTagLine = "EXIST_TAG_LINE";

            var existingNewGameName = "EXIST_NEW_GAME_NAME";
            var existingNewTagLine = "EXIST_NEW_TAG_LINE";


            var notExistingGameName = "NOT_EXIST_GAME_NAME";
            var notExistingTagLine = "NOT_EXIST_TAG_LINE";



            var existsSummonerAccountInfo = new RiotClientData.GetSummonerAccountInfoResponse
            {
                Puuid = existsPuuid,
                GameName = existingNewGameName,
                TagLine = existingNewTagLine,
            };

            var notExistsSummonerAccountInfo = new RiotClientData.GetSummonerAccountInfoResponse
            {
                Puuid = notExistsPuuid,
                GameName = notExistingGameName,
                TagLine = notExistingTagLine,
            };

            _riotClient.Setup(client => client.GetSummonerAccountInfoByPuuid(existsPuuid)).ReturnsAsync(existsSummonerAccountInfo);
            _riotClient.Setup(client => client.GetSummonerAccountInfoByPuuid(notExistsPuuid)).ReturnsAsync(notExistsSummonerAccountInfo);

            var existingSummoner = new Summoner
            {
                Puuid = existsPuuid,
                GameName = existingGameName,
                TagLine = existingTagLine,
            };

            var notExistingSummoner = new Summoner
            {
                Puuid = notExistsPuuid,
                GameName = notExistingGameName,
                TagLine = notExistingTagLine,
            };

            _summonerService.Setup(service => service.GetSummonerByPuuid(existsPuuid)).ReturnsAsync(existingSummoner);
            _summonerService.Setup(service => service.GetSummonerByPuuid(notExistsPuuid)).ThrowsAsync(new BusinessException(SummonerErrorCode.SUMMONER_NOT_FOUND));

            _summonerService.Setup(service => service.PatchSummoner(existingSummoner)).ReturnsAsync(existingSummoner);
            _summonerService.Setup(service => service.saveSummoner(notExistingSummoner)).ReturnsAsync(notExistingSummoner);

            await _warmUpSummonerJob.WarmUpSummoners();

            _summonerService.Verify(service => service.saveSummoner(It.IsAny<Summoner>()), Times.AtLeastOnce);
            _summonerService.Verify(service => service.PatchSummoner(existingSummoner), Times.AtLeastOnce);
        }
    }
}