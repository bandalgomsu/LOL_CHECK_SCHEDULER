using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.devices.service.interfaces;
using lol_check_scheduler.src.app.scheduler;
using lol_check_scheduler.src.app.subscribers.service.interfaces;
using lol_check_scheduler.src.app.summoners.service.interfaces;
using lol_check_scheduler.src.infrastructure.firebase;
using lol_check_scheduler.src.infrastructure.firebase.interfaces;
using lol_check_scheduler.src.infrastructure.riotclient;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace test.src.app.scheduler
{
    public class CheckPlayingGameSchedulerTest
    {
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactory = new Mock<IServiceScopeFactory>();
        private readonly Mock<IServiceScope> _serviceScope = new Mock<IServiceScope>();
        private readonly Mock<IRiotClient> _riotClient = new Mock<IRiotClient>();
        private readonly Mock<IFcmClient> _fcmClient = new Mock<IFcmClient>();
        private readonly Mock<ISummonerService> _summonerService = new Mock<ISummonerService>();
        private readonly Mock<IDeviceService> _deviceService = new Mock<IDeviceService>();
        private readonly Mock<ISubscriberService> _subscriberService = new Mock<ISubscriberService>();

        private readonly CheckPlayingGameScheduler _checkPlayingGameScheduler;

        public CheckPlayingGameSchedulerTest()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();

            mockServiceProvider
                .Setup(provider => provider.GetService(typeof(ISummonerService)))
                .Returns(_summonerService.Object);

            mockServiceProvider
                .Setup(provider => provider.GetService(typeof(IDeviceService)))
                .Returns(_deviceService.Object);

            mockServiceProvider
                .Setup(provider => provider.GetService(typeof(ISubscriberService)))
                .Returns(_subscriberService.Object);


            _serviceScope.Setup(scope => scope.ServiceProvider).Returns(mockServiceProvider.Object);
            _serviceScopeFactory.Setup(factory => factory.CreateScope()).Returns(_serviceScope.Object);

            _checkPlayingGameScheduler = new CheckPlayingGameScheduler(_serviceScopeFactory.Object, _riotClient.Object, _fcmClient.Object);
        }

        /**
            S1 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 1명 이상 존재 , 구독자의 토큰 1개 이상 존재 ,   
            S2 =>
            S3 =>
        **/

        [Fact(DisplayName = "CHECK_PLAYING_GAME_CHECK_PLAYING_SUMMONER_SUCCESS")]
        public async Task CHECK_PLAYING_GAME_S1_CHECK_PLAYING_SUMMONER()
        {
            var summoner = new Summoner
            {
                Id = 1,
                Puuid = "TEST_PUUID",
                GameName = "TEST_GAME_NAME",
                TagLine = "TEST_TAG_LINE",
                RecentGameId = 1
            };

            _summonerService.Setup(service => service.GetSummonersByTopN(49)).ReturnsAsync([summoner]);
            _riotClient.Setup(client => client.GetCurrentGameInfo(summoner.Puuid!)).ReturnsAsync(
                new RiotClientData.CurrentGameInfo { GameId = 2 }
            );

            var subscriber = new Subscriber
            {
                Id = 1,
                SummonerId = summoner.Id,
                SubscriberId = 1,
                SummonerGameName = summoner.GameName,
                SummonerTagLine = summoner.TagLine,
            };

            _subscriberService.Setup(service => service.GetSubscribersBySummonerId(summoner.Id!)).ReturnsAsync(
                [subscriber]
            );
            var tokens = new List<string> { "TEST_TOKEN" };

            _deviceService.Setup(service => service.GetDeviceTokensByUserIds(It.IsAny<IEnumerable<long>>())).ReturnsAsync(tokens);
            _fcmClient.Setup(client => client.SendMulticastMessage(It.IsAny<FcmClientData.FmcMulticastMessage>()));

            // _summonerService.Setup(service => service.PatchSummoner(summoner));

            await _checkPlayingGameScheduler.CheckPlayingGameJob();
        }
    }
}