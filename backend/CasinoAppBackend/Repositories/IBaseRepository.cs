namespace CasinoAppBackend.Repositories
{
    public interface IBaseRepository<T>
    {
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task<T?> GetAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> GetCountAsync();
        IQueryable<T> Query();
    }
}
