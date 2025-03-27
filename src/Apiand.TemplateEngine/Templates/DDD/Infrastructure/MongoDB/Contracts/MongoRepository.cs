using System.Linq.Expressions;
using Apiand.Extensions.DDD;
using MongoDB.Bson;
using MongoDB.Driver;
using XXXnameXXX.Application.Contracts;

namespace XXXnameXXX.Infrastructure.Contracts;

public class MongoRepository<T>(IMongoDatabase database) : IRepository<T>
    where T : Entity
{
    private readonly IMongoCollection<T> _collection = database.GetCollection<T>(typeof(T).Name);

    public async Task<T> GetByIdAsync(string id)
    {
        return await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {

        if (entity.Id == null)
        {
            entity.Id = ObjectId.GenerateNewId().ToString();
        }
        
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
    }
}