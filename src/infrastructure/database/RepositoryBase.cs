namespace lol_check_scheduler.src.infrastructure.database
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public RepositoryBase(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task<IEnumerable<T>> FindAll()
        {
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                return await databaseContext.Set<T>().ToListAsync();
            }
        }

        public async Task<IEnumerable<T>> FindAllByCondition(Expression<Func<T, bool>> expression)
        {
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                return await databaseContext.Set<T>().Where(expression).ToListAsync();
            }
        }

        public async Task<T?> FindByCondition(Expression<Func<T, bool>> expression)
        {
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                return await databaseContext.Set<T>().FirstOrDefaultAsync(expression);
            }
        }
        public async Task<T> Create(T entity)
        {
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                await databaseContext.Set<T>().AddAsync(entity);
                await databaseContext.SaveChangesAsync();
                return entity;
            }
        }

        public async Task<T> Patch(T entity)
        {
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                await databaseContext.SaveChangesAsync();

                return entity;
            }
        }

        public async Task<T> Update(T entity)
        {
            using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                databaseContext.Set<T>().Attach(entity);
                databaseContext.Set<T>().Update(entity);
                await databaseContext.SaveChangesAsync();

                return entity;
            }
        }
        public async Task Delete(T entity)
        {
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var databaseContext = scope.ServiceProvider.GetService<DatabaseContext>()!;

                databaseContext.Set<T>().Remove(entity);
                await databaseContext.SaveChangesAsync();
            }
        }
    }
}