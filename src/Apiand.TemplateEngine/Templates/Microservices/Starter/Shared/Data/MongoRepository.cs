using MongoDB.Driver;
using System.Linq.Expressions;
using Apiand.Extensions.DDD;

namespace XXXnameXXX.Shared.Data;

public class MongoRepository<T> : IRepository<T> where T : Entity
{
    private readonly IMongoCollection<T> _collection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(typeof(T).Name);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<T> InsertAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task InsertManyAsync(IEnumerable<T> entities)
    {
        await _collection.InsertManyAsync(entities);
    }

    public async Task<bool> UpdateAsync(string id, T entity)
    {
        var filter = _filterBuilder.Eq("_id", id);
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateManyAsync(IEnumerable<T> entities)
    {
        var bulkOps = new List<WriteModel<T>>();

        foreach (var entity in entities)
        {
            var id = entity.Id;
            var filter = _filterBuilder.Eq("_id", id);
            var replaceOne = new ReplaceOneModel<T>(filter, entity);
            bulkOps.Add(replaceOne);
        }

        if (!bulkOps.Any())
            return false;

        var result = await _collection.BulkWriteAsync(bulkOps);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = _filterBuilder.Eq("_id", id);
        var result = await _collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> filter)
    {
        var result = await _collection.DeleteManyAsync(filter);
        return result.DeletedCount > 0;
    }
}