using CasinoAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly CasinoAppBackendDbContext context;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(CasinoAppBackendDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public virtual async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
            => await dbSet.AddRangeAsync(entities);

        public Task UpdateAsync(T entity)
        {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task<T?> GetAsync(Guid id) => await dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        public virtual async Task<int> GetCountAsync() => await dbSet.CountAsync();
        public IQueryable<T> Query() => dbSet.AsQueryable();
    }
}
