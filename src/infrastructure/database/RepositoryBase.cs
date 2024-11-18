namespace lol_check_scheduler.src.infrastructure.database
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected DatabaseContext databaseContext { get; set; }
        public RepositoryBase(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }
        public async Task<IEnumerable<T>> FindAll()
        {
            return await databaseContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllByCondition(Expression<Func<T, bool>> expression)
        {
            return await databaseContext.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<T?> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return await databaseContext.Set<T>().FirstOrDefaultAsync(expression);
        }
        public async Task<T> Create(T entity)
        {
            await databaseContext.Set<T>().AddAsync(entity);
            await databaseContext.SaveChangesAsync();

            return entity;
        }
        public async Task<T> Update(T entity)
        {
            databaseContext.Set<T>().Update(entity);
            await databaseContext.SaveChangesAsync();

            return entity;
        }
        public async void Delete(T entity)
        {
            databaseContext.Set<T>().Remove(entity);
            await databaseContext.SaveChangesAsync();
        }
    }
}