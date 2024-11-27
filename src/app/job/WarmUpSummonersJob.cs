using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.summoners.service;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;
using Quartz;

namespace lol_check_scheduler.src.app.job
{
    /**
    **  챌린저 소환사 DB 적재 작업
    **/
    public class WarmUpChallengerSummonersJob(
        IRiotClient riotClient,
        SummonerService summonerService
    ) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await WarmUpChallengerSummoners();
        }

        public async Task WarmUpChallengerSummoners()
        {
            var leagueList = await riotClient.GetLeagueListInChallengerLeagues();

            IEnumerable<Task> tasks = leagueList.Entries.Select(async entry =>
            {
                var summonerInfo = await riotClient.GetSummonerInfoBySummonerId(entry.SummonerId);
                var summonerAccountInfo = await riotClient.GetSummonerAccountInfoByPuuid(summonerInfo.Puuid);



                await summonerService.saveSummoner(
                    new Summoner
                    {
                        Puuid = summonerAccountInfo.Puuid,
                        GameName = summonerAccountInfo.GameName,
                        TagLine = summonerAccountInfo.TagLine,
                    }
                );
            });
        }


    }
}