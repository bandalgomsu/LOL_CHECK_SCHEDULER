using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.summoners.service;
using lol_check_scheduler.src.app.summoners.service.interfaces;
using lol_check_scheduler.src.common.exception;
using lol_check_scheduler.src.infrastructure.riotclient;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;
using Quartz;

namespace lol_check_scheduler.src.app.job
{
    public class WarmUpSummonerJob(
        IRiotClient riotClient,
        ISummonerService summonerService
    ) : IJob
    {
        private readonly ILogger<WarmUpSummonerJob> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<WarmUpSummonerJob>();
        public async Task Execute(IJobExecutionContext context)
        {
            await WarmUpSummoners();
        }

        public async Task WarmUpSummoners()
        {
            IEnumerable<Func<Task<RiotClientData.LeagueListDTO>>> leagueFunctions = new List<Func<Task<RiotClientData.LeagueListDTO>>>
            {
                riotClient.GetLeagueListInChallengerLeagues,
                riotClient.GetLeagueListInGrandMasterLeagues,
                riotClient.GetLeagueListInMasterLeagues,
            };

            IEnumerable<Task> tasks = leagueFunctions.Select(async function =>
                {
                    await WarmUpSummonersByLeague(function);
                }
            );

            await Task.WhenAll(tasks);
        }

        private async Task WarmUpSummonersByLeague(Func<Task<RiotClientData.LeagueListDTO>> getLeagueDataFunction)
        {
            var stopwatch = Stopwatch.StartNew();

            var leagueList = await getLeagueDataFunction();
            _logger.LogInformation("SUMMONER_COUNT = {}", leagueList.Entries.Count());

            var tasks = leagueList.Entries.Select(async entry =>
            {
                var summonerInfo = await riotClient.GetSummonerInfoBySummonerId(entry.SummonerId);
                var summonerAccountInfo = await riotClient.GetSummonerAccountInfoByPuuid(summonerInfo.Puuid);

                try
                {
                    var summoner = await summonerService.GetSummonerByPuuid(summonerAccountInfo.Puuid);

                    if (summoner.GameName != summonerAccountInfo.GameName || summoner.TagLine != summonerAccountInfo.TagLine)
                    {
                        summoner.Puuid = summonerAccountInfo.Puuid;
                        summoner.GameName = summonerAccountInfo.GameName;
                        summoner.TagLine = summonerAccountInfo.TagLine;

                        await summonerService.PatchSummoner(summoner);
                    }
                }
                catch (BusinessException)
                {
                    await summonerService.saveSummoner(
                        new Summoner
                        {
                            Puuid = summonerAccountInfo.Puuid,
                            GameName = summonerAccountInfo.GameName,
                            TagLine = summonerAccountInfo.TagLine,
                        }
                    );
                }
            });

            await Task.WhenAll(tasks);

            stopwatch.Stop();

            // _logger.LogInformation("WARM_UP_SUMMONER_COUNT = {}", tasks.Count());
            _logger.LogInformation("WORK_TIME = {}", stopwatch.ElapsedMilliseconds);
        }
    }
}