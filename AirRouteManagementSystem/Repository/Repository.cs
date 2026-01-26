using AirRouteManagementSystem.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AirRouteManagementSystem.Reposatory
{
    public class Repository<T>: IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _dBContext;
        public DbSet<T> _dbSet;

        public Repository(ApplicationDBContext dBContext)
        {
            _dBContext = dBContext;
            _dbSet =  _dBContext.Set<T>();
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
        {
            var entities = await _dbSet.AddAsync(entity);

            return entities.Entity;
        }

        public void Remove(T entity) => _dbSet.Remove(entity);
        public void Update(T entity) => _dbSet.Update(entity);

        public async Task<IEnumerable<T>> GetAsync
            (
               Expression<Func<T, bool>>? expression = null,
               Expression<Func<T, object>>[]? Include = null,
               CancellationToken cancellationToken = default,
               bool tracking = true
            )
        {
            var entities = _dbSet.AsQueryable();

            if (expression is not null)
                entities = entities.Where(expression);

            if (Include is not null)
            {
                foreach (var entity in Include)
                {
                    entities = entities.Include(entity);
                }
            }

            if (!tracking)
                entities = entities.AsNoTracking();
            return await entities.ToListAsync();
        }

        public async Task<T?> GetOneAsync
            (
               Expression<Func<T, bool>>? expression = null,
               Expression<Func<T, object>>[]? Include = null,
               CancellationToken cancellationToken = default,
               bool tracking = true
            )
        {
            return (await GetAsync(expression, Include, cancellationToken, tracking)).FirstOrDefault();
        }

    }
}
