using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace XXXnameXXX.Data;

public class DapperRepository<T> : IRepository<T> where T : class
{
    private readonly string _connectionString;
    private readonly string _tableName;

    public DapperRepository(IConfiguration configuration, string tableName)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new ArgumentNullException(nameof(configuration), "Connection string not found");
        _tableName = tableName;
    }

    protected IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<T>($"SELECT * FROM \"{_tableName}\"");
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<T>(
            $"SELECT * FROM \"{_tableName}\" WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task<int> AddAsync(T entity)
    {
        using var connection = CreateConnection();
        return await connection.InsertAsync(entity);
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        using var connection = CreateConnection();
        return await connection.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(
            $"DELETE FROM \"{_tableName}\" WHERE \"Id\" = @Id",
            new { Id = id });

        return rowsAffected > 0;
    }
}