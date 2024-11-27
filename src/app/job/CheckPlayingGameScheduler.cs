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

namespace lol_check_scheduler.src.app.job
{
    /**
    **  소환사 게임 시작 감지 및 게임 시작 푸시 알림 발송 작업
    **/
    public class CheckPlayingGameJob(
        IRiotClient riotClient,
        IFcmClient fcmClient,
        ISummonerService summonerService,
        IDeviceService deviceService,
        ISubscriberService subscriberService
        ) : IJob
    {

        private readonly ILogger<CheckPlayingGameJob> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CheckPlayingGameJob>();

        public async Task Execute(IJobExecutionContext context)
        {
            await CheckPlayingGame();
        }

        public async Task CheckPlayingGame()
        {
            IEnumerable<Summoner> summoners = await summonerService.GetSummonersByTopN(49);

            List<Summoner> playingSummoners = new List<Summoner>();

            IEnumerable<Task> tasks = summoners.Select(async summoner =>
            {
                RiotClientData.CurrentGameInfo currentGameInfo = await riotClient.GetCurrentGameInfo(summoner.Puuid!);

                if (currentGameInfo.IsCurrentPlayingGame && summoner.RecentGameId != currentGameInfo.GameId)
                {
                    summoner.RecentGameId = currentGameInfo.GameId;
                    playingSummoners.Add(summoner);
                }
            });

            await Task.WhenAll(tasks);

            _logger.LogInformation("TOTAL_COUNT : {}", playingSummoners.Count());

            if (!playingSummoners.Any())
            {
                return;
            }

            IEnumerable<Summoner> success = await SendMulticastMessageProcess(playingSummoners);

            _logger.LogInformation("SUCCESS_COUNT : {}", success.Count());
            _logger.LogInformation("FAILURE_COUNT : {}", playingSummoners.Count() - success.Count());

            _ = PatchSuccessSummoners(success);
        }

        private async Task<IEnumerable<Summoner>> SendMulticastMessageProcess(IEnumerable<Summoner> forUpdateSummoner)
        {
            var tasks = forUpdateSummoner.Select(async summoner =>
            {
                IEnumerable<string> tokens = await GetTokens(summoner);

                if (tokens.Any())
                {
                    var message = FcmClientData.FmcMulticastMessage.CreateCheckedPlayingGameMessage(tokens, summoner);

                    await fcmClient.SendMulticastMessage(message);
                }

                return summoner;
            });

            return (await Task.WhenAll(tasks)).Where(summoner => summoner != null)!;
        }

        private async Task<IEnumerable<string>> GetTokens(Summoner summoner)
        {
            var subscribers = await subscriberService.GetSubscribersBySummonerId(summoner.Id!);
            var ids = subscribers.Select(subscriber => subscriber.SubscriberId);

            return await deviceService.GetDeviceTokensByUserIds(ids);
        }

        private async Task PatchSuccessSummoners(IEnumerable<Summoner> success)
        {
            foreach (var summoner in success)
            {
                await summonerService.PatchSummoner(summoner);
            }
        }
    }
}