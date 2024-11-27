using lol_check_scheduler.src.app.summoners.repository.interfaces;

namespace lol_check_scheduler.src.app.summoners.repository
{
    public class SummonerRepository(IServiceScopeFactory serviceScopeFactory) : RepositoryBase<Summoner>(serviceScopeFactory), ISummonerRepository
    {
        public async Task<IEnumerable<Summoner>> FindAllByTopN(int n)
        {
            using (var scope = serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                return await databaseContext.Summoner!
                .OrderByDescending(Summoner => Summoner.UpdatedAt) // 엔터티 속성 접근
                .Take(n)
                .ToListAsync();
            }
        }
    }
}