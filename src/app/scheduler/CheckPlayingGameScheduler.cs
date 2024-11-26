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
using Quartz;

namespace lol_check_scheduler.src.app.scheduler
{
    public class CheckPlayingGameScheduler : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRiotClient _riotClient;
        private readonly IFcmClient _fcmClient;

        private readonly ILogger<CheckPlayingGameScheduler> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CheckPlayingGameScheduler>();
        public CheckPlayingGameScheduler(IServiceScopeFactory serviceScopeFactory, IRiotClient riotClient, IFcmClient fcmClient)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _riotClient = riotClient;
            _fcmClient = fcmClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await CheckPlayingGameJob();
        }

        public async Task CheckPlayingGameJob()
        {
            using (var scope = _serviceScopeFactory.CreateScope()) // 이곳에서 범위(scope) 생성
            {
                var summonerService = scope.ServiceProvider.GetRequiredService<ISummonerService>();
                var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
                var subscriberService = scope.ServiceProvider.GetRequiredService<ISubscriberService>();

                IEnumerable<Summoner> summoners = await summonerService.GetSummonersByTopN(49);

                List<Summoner> playingSummoners = new List<Summoner>();

                IEnumerable<Task> tasks = summoners.Select(async summoner =>
                {
                    RiotClientData.CurrentGameInfo currentGameInfo = await _riotClient.GetCurrentGameInfo(summoner.Puuid!);

                    if (currentGameInfo.IsCurrentPlayingGame && summoner.RecentGameId != currentGameInfo.GameId)
                    {
                        summoner.RecentGameId = currentGameInfo.GameId;
                        playingSummoners.Add(summoner);
                    }
                });

                await Task.WhenAll(tasks);

                if (!playingSummoners.Any())
                {
                    return;
                }

                IEnumerable<Summoner> success = await SendMulticastMessageProcess(playingSummoners, subscriberService, deviceService);

                _logger.LogInformation("TOTAL_COUNT : {}", playingSummoners.Count());
                _logger.LogInformation("SUCCESS_COUNT : {}", success.Count());
                _logger.LogInformation("FAILURE_COUNT : {}", playingSummoners.Count() - success.Count());

                _ = Task.Run(async () =>
                    {
                        foreach (var summoner in success)
                        {
                            await summonerService.PatchSummoner(summoner!);
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

                if (tokens.Any())
                {
                    var message = FcmClientData.FmcMulticastMessage.CreateCheckedPlayingGameMessage(tokens, summoner);

                    await _fcmClient.SendMulticastMessage(message);
                }

                return summoner;
            });

            return (await Task.WhenAll(tasks)).Where(summoner => summoner != null)!;
        }

        private async Task<IEnumerable<string>> GetTokens(Summoner summoner, ISubscriberService subscriberService, IDeviceService deviceService)
        {
            var subscribers = await subscriberService.GetSubscribersBySummonerId(summoner.Id!);
            var ids = subscribers.Select(subscriber => subscriber.SubscriberId);

            return await deviceService.GetDeviceTokensByUserIds(ids);
        }
    }
}
