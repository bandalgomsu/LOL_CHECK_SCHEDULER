using lol_check_scheduler.src.app.summoners.exception;
using lol_check_scheduler.src.app.summoners.repository;
using lol_check_scheduler.src.app.summoners.repository.interfaces;
using lol_check_scheduler.src.app.summoners.service.interfaces;
using lol_check_scheduler.src.common.exception;

namespace lol_check_scheduler.src.app.summoners.service
{
    public class SummonerService(ISummonerRepository SummonerRepository) : ISummonerService
    {
        private readonly ISummonerRepository _summonerRepository = SummonerRepository;

        public async Task<Summoner> GetSummonerByPuuid(string puuid)
        {
            return await _summonerRepository.FindByCondition(summoner => summoner.Puuid == puuid) ?? throw new BusinessException(SummonerErrorCode.SUMMONER_NOT_FOUND);
        }

        public async Task<IEnumerable<Summoner>> GetSummonersByTopN(int n)
        {
            return await _summonerRepository.FindAllByTopN(n);
        }

        public async Task<Summoner> PatchSummoner(Summoner summoner)
        {
            return await _summonerRepository.Patch(summoner);
        }

        public async Task<Summoner> saveSummoner(Summoner summoner)
        {
            return await _summonerRepository.Create(summoner);
        }

        public async Task<Summoner> UpdateSummoner(Summoner summoner)
        {
            return await _summonerRepository.Update(summoner);
        }
    }
}