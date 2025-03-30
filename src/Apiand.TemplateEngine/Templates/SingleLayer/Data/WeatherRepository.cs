using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using XXXnameXXX.Models;

namespace XXXnameXXX.Data;

public class WeatherRepository : DapperRepository<WeatherRecord>, IWeatherRepository
{
    private const string TableName = "WeatherRecords";
    
    public WeatherRepository(IConfiguration configuration) 
        : base(configuration, TableName)
    {
    }
    
    public async Task<IEnumerable<WeatherRecord>> GetRecentWeatherAsync(int days)
    {
        using var connection = CreateConnection();
        var cutoffDate = DateTime.Now.AddDays(-days);
        
        return await connection.QueryAsync<WeatherRecord>(
            $"SELECT * FROM {TableName} WHERE Date >= @CutoffDate ORDER BY Date DESC", 
            new { CutoffDate = cutoffDate });
    }
}
