using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.app.summoners.service.interfaces
{
    public interface ISummonerService
    {
        Task<IEnumerable<Summoner>> GetSummonersByTopN(int n);

        Task<Summoner> GetSummonerByPuuid(string puuid);
        Task<Summoner> UpdateSummoner(Summoner summoner);
        Task<Summoner> PatchSummoner(Summoner summoner);
        Task<Summoner> saveSummoner(Summoner summoner);
    }
}