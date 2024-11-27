using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;
using Quartz;

namespace lol_check_scheduler.src.app.job
{
    /**
    **  챌린저 소환사 DB 적재 작업
    **/
    public class WarmUpChallengerSummonersJob(
        IRiotClient riotClient
    ) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await WarmUpChallengerSummoners();
        }

        public async Task WarmUpChallengerSummoners()
        {

        }
    }
}