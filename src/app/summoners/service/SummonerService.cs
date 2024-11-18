using lol_check_scheduler.src.app.summoners.repository;
using lol_check_scheduler.src.app.summoners.repository.interfaces;
using lol_check_scheduler.src.app.summoners.service.interfaces;

namespace lol_check_scheduler.src.app.summoners.service
{
    public class SummonerService : ISummonerService
    {
        private readonly ISummonerRepository _summonerRepository;

        public SummonerService(ISummonerRepository SummonerRepository)
        {
            _summonerRepository = SummonerRepository;
        }

        public async Task<IEnumerable<Summoner>> GetSummonersByTopN(int n)
        {
            return await _summonerRepository.FindAllByTopN(n);
        }

        public async Task<Summoner> UpdateSummoner(Summoner summoner)
        {
            return await _summonerRepository.Update(summoner);
        }
    }
}