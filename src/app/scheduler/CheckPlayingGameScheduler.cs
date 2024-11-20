using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.devices.service.interfaces;
using lol_check_scheduler.src.app.subscribers.service.interfaces;
using lol_check_scheduler.src.app.summoners.service.interfaces;
using lol_check_scheduler.src.infrastructure.firebase;
using lol_check_scheduler.src.infrastructure.firebase.interfaces;
using lol_check_scheduler.src.infrastructure.riotclient;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;

namespace lol_check_scheduler.src.app.scheduler
{
    public class CheckPlayingGameScheduler : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRiotClient _riotClient;
        private readonly IFcmClient _fcmClient;

        private Timer? _timer;

        public CheckPlayingGameScheduler(IServiceScopeFactory serviceScopeFactory, IRiotClient riotClient, IFcmClient fcmClient)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _riotClient = riotClient;
            _fcmClient = fcmClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 1분마다 비동기 메서드를 실행하도록 타이머 설정
            _timer = new Timer(ExecuteTaskAsync!, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void ExecuteTaskAsync(object state)
        {
            try
            {
                await CheckPlayingGameJob();
            }
            catch (Exception)
            {
                // 예외 처리
            }
        }

        public async Task CheckPlayingGameJob()
        {
            using (var scope = _serviceScopeFactory.CreateScope()) // 이곳에서 범위(scope) 생성
            {
                var summonerService = scope.ServiceProvider.GetRequiredService<ISummonerService>();
                var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
                var subscriberService = scope.ServiceProvider.GetRequiredService<ISubscriberService>();

                // Summoner 정보를 가져옵니다.
                IEnumerable<Summoner> summoners = await summonerService.GetSummonersByTopN(49);

                List<Summoner> forUpdateSummoner = new List<Summoner>();

                IEnumerable<Task> tasks = summoners.Select(async summoner =>
                {
                    RiotClientData.CurrentGameInfo currentGameInfo = await _riotClient.GetCurrentGameInfo(summoner.Puuid!);

                    if (currentGameInfo.IsCurrentPlayingGame && summoner.RecentGameId != currentGameInfo.GameId)
                    {
                        summoner.RecentGameId = currentGameInfo.GameId;
                        forUpdateSummoner.Add(summoner);
                    }
                });

                await Task.WhenAll(tasks);

                // 업데이트가 필요한 Summoner 목록을 전송합니다.
                IEnumerable<Summoner> success = await SendMulticastMessageProcess(forUpdateSummoner, subscriberService, deviceService);

                _ = Task.Run(async () =>
                    {
                        foreach (var summoner in success)
                        {
                            await summonerService.PatchSummoner(summoner);
                        }
                    }
                );
            }
        }

        private async Task<IEnumerable<Summoner>> SendMulticastMessageProcess(IEnumerable<Summoner> forUpdateSummoner, ISubscriberService subscriberService, IDeviceService deviceService)
        {
            var tasks = forUpdateSummoner.Select(async summoner =>
            {
                IEnumerable<string> tokens = await GetTokens(summoner, subscriberService, deviceService);

                var message = FcmClientData.FmcMulticastMessage.CreateCheckedPlayingGameMessage(tokens, summoner);

                await _fcmClient.SendMulticastMessage(message);

                return summoner;
            });

            return await Task.WhenAll(tasks);
        }

        private async Task<IEnumerable<string>> GetTokens(Summoner summoner, ISubscriberService subscriberService, IDeviceService deviceService)
        {
            var subscribers = await subscriberService.GetSubscribersBySummonerId(summoner.Id!);
            var ids = subscribers.Select(subscriber => subscriber.Id);

            return await deviceService.GetDeviceTokensByUserIds(ids);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
