using System.Linq.Expressions;
using Apiand.Extensions.DDD;

namespace XXXnameXXX.Shared.Data;

public interface IRepository<T> where T : Entity
{
    // Read operations
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);

    // Create operations
    Task<T> InsertAsync(T entity);
    Task InsertManyAsync(IEnumerable<T> entities);

    // Update operations
    Task<bool> UpdateAsync(string id, T entity);
    Task<bool> UpdateManyAsync(IEnumerable<T> entities);

    // Delete operations
    Task<bool> DeleteAsync(string id);
    Task<bool> DeleteAsync(Expression<Func<T, bool>> filter);
}