using System.Linq.Expressions;
using Apiand.Extensions.DDD;

namespace XXXnameXXX.Application.Contracts;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
    Task AddAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
}