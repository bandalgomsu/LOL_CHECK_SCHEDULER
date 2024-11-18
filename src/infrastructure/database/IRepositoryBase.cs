namespace lol_check_scheduler.src.infrastructure.database
{
    public interface IRepositoryBase<T>
    {
        Task<IEnumerable<T>> FindAll();
        Task<IEnumerable<T>> FindAllByCondition(Expression<Func<T, bool>> expression);
        Task<T?> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> Create(T entity);
        Task<T> Update(T entity);
        void Delete(T entity);
    }
}