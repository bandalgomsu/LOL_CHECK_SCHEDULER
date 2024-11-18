using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.app.summoners.repository.interfaces
{
    public interface ISummonerRepository : IRepositoryBase<Summoner>
    {
        Task<IEnumerable<Summoner>> FindAllByTopN(int n);
    }
}