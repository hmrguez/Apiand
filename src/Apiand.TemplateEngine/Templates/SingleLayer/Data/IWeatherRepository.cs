using XXXnameXXX.Models;

namespace XXXnameXXX.Data;

public interface IWeatherRepository : IRepository<WeatherRecord>
{
    // You can add weather-specific methods here if needed
    Task<IEnumerable<WeatherRecord>> GetRecentWeatherAsync(int days);
}
