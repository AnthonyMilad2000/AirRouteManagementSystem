using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AirRouteManagementSystem.Repository.IRepository
{
    public interface IRepository<T>
    {
        public Task<T> CreateAsync(T entity, CancellationToken cancellationToken);
        public void Remove(T entity) ;
        public void Update(T entity) ;
        public Task<IEnumerable<T>> GetAsync
            (
               Expression<Func<T, bool>>? expression = null,
               Expression<Func<T, object>>[]? Include = null,
               CancellationToken cancellationToken = default,
               bool tracking = true
            );

       public Task<T?> GetOneAsync
            (
               Expression<Func<T, bool>>? expression = null,
               Expression<Func<T, object>>[]? Include = null,
               CancellationToken cancellationToken = default,
               bool tracking = true
            );

    }
}
