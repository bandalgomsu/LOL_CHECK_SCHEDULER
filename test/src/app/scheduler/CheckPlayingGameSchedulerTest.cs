using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
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
            S1 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 1명 이상 존재 , 구독자의 토큰 1개 이상 존재 , 푸시 정상 발송  
            S2 => 게임을 시작한 소환사 감지 X
            S3 => 게임을 시작한 소환사의 최근 게임 ID 값이 현재 게임 ID와 같음
            S4 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 존재하지 않음
            S5 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 존재 , 구독자의 토큰이 존재하지 않음 
            S6 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 존재 , 구독자의 토큰이 존재 , 푸시 실패
        **/

        // S1 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 1명 이상 존재 , 구독자의 토큰 1개 이상 존재 , 푸시 정상 발송 
        [Fact(DisplayName = "CHECK_PLAYING_GAME_S1")]
        public async Task CHECK_PLAYING_GAME_S1()
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
            };

            _subscriberService.Setup(service => service.GetSubscribersBySummonerId(summoner.Id!)).ReturnsAsync(
                [subscriber]
            );
            var tokens = new List<string> { "TEST_TOKEN" };

            _deviceService.Setup(service => service.GetDeviceTokensByUserIds(It.IsAny<IEnumerable<long>>())).ReturnsAsync(tokens);
            _fcmClient.Setup(client => client.SendMulticastMessage(It.IsAny<FcmClientData.FmcMulticastMessage>(), It.IsAny<bool>()));

            await _checkPlayingGameScheduler.CheckPlayingGameJob();

            _fcmClient.Verify(client => client.SendMulticastMessage(It.IsAny<FcmClientData.FmcMulticastMessage>(), It.IsAny<bool>()), Times.Once);
        }

        // S2 => 게임을 시작한 소환사 감지 X
        [Fact(DisplayName = "CHECK_PLAYING_GAME_S2")]
        public async Task CHECK_PLAYING_GAME_S2()
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
                new RiotClientData.CurrentGameInfo { IsCurrentPlayingGame = false }
            );

            await _checkPlayingGameScheduler.CheckPlayingGameJob();

            _subscriberService.Verify(service => service.GetSubscribersBySummonerId(It.IsAny<long>()), Times.Never);
        }

        // S3 => 게임을 시작한 소환사의 최근 게임 ID 값이 현재 게임 ID와 같음
        [Fact(DisplayName = "CHECK_PLAYING_GAME_S3")]
        public async Task CHECK_PLAYING_GAME_S3()
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
                new RiotClientData.CurrentGameInfo { GameId = 1 }
            );

            await _checkPlayingGameScheduler.CheckPlayingGameJob();

            _subscriberService.Verify(service => service.GetSubscribersBySummonerId(It.IsAny<long>()), Times.Never);
        }

        // S4 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 존재하지 않음
        [Fact(DisplayName = "CHECK_PLAYING_GAME_S4")]
        public async Task CHECK_PLAYING_GAME_S4()
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

            _subscriberService.Setup(service => service.GetSubscribersBySummonerId(summoner.Id!)).ReturnsAsync(
                []
            );

            await _checkPlayingGameScheduler.CheckPlayingGameJob();

            _fcmClient.Verify(client => client.SendMulticastMessage(It.IsAny<FcmClientData.FmcMulticastMessage>(), It.IsAny<bool>()), Times.Never);
        }

        // S5 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 존재 , 구독자의 토큰이 존재하지 않음 
        [Fact(DisplayName = "CHECK_PLAYING_GAME_S5")]
        public async Task CHECK_PLAYING_GAME_S5()
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
            };

            _subscriberService.Setup(service => service.GetSubscribersBySummonerId(summoner.Id!)).ReturnsAsync(
                [subscriber]
            );

            _deviceService.Setup(service => service.GetDeviceTokensByUserIds(It.IsAny<IEnumerable<long>>())).ReturnsAsync([]);

            await _checkPlayingGameScheduler.CheckPlayingGameJob();

            _fcmClient.Verify(client => client.SendMulticastMessage(It.IsAny<FcmClientData.FmcMulticastMessage>(), It.IsAny<bool>()), Times.Never);
        }

        // S6 => 게임을 시작한 소환사를 1명 이상 감지 , 해당 소환사의 최근 게임 ID가 진행중인 게임 ID와 다름 , 해당 소환사의 구독자 존재 , 구독자의 토큰이 존재 , 푸시 실패
        [Fact(DisplayName = "CHECK_PLAYING_GAME_S6")]
        public async Task CHECK_PLAYING_GAME_S6()
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
            };

            _subscriberService.Setup(service => service.GetSubscribersBySummonerId(summoner.Id!)).ReturnsAsync(
                [subscriber]
            );
            var tokens = new List<string> { "TEST_TOKEN" };

            _deviceService.Setup(service => service.GetDeviceTokensByUserIds(It.IsAny<IEnumerable<long>>())).ReturnsAsync(tokens);
            _fcmClient.Setup(client => client.SendMulticastMessage(It.IsAny<FcmClientData.FmcMulticastMessage>(), It.IsAny<bool>()))
                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(async () => await _checkPlayingGameScheduler.CheckPlayingGameJob());
        }
    }
}