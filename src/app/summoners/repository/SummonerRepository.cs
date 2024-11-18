using lol_check_scheduler.src.app.summoners.repository.interfaces;

namespace lol_check_scheduler.src.app.summoners.repository
{
    public class SummonerRepository : RepositoryBase<Summoner>, ISummonerRepository
    {
        public SummonerRepository(DatabaseContext databaseContext) : base(databaseContext)
        {
        }

        public async Task<IEnumerable<Summoner>> FindAllByTopN(int n)
        {
            return await databaseContext.Summoner
                .OrderByDescending(Summoner => Summoner.UpdatedAt) // 엔터티 속성 접근
                .Take(n)
                .ToListAsync();
        }
    }
}