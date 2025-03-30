using FastEndpoints;
using XXXnameXXX.Data;
using XXXnameXXX.Models;

namespace XXXnameXXX.Endpoints.Weather;

public class GetWeatherRecordsRequest
{
    public int? Days { get; set; }
}

public class GetWeatherRecordsEndpoint : Endpoint<GetWeatherRecordsRequest, IEnumerable<WeatherRecord>>
{
    private readonly IWeatherRepository _weatherRepository;
    
    public GetWeatherRecordsEndpoint(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }
    
    public override void Configure()
    {
        Get("/api/weather");
        AllowAnonymous();
        Summary(s => {
            s.Summary = "Gets weather records from database";
            s.Description = "Returns stored weather records from the database";
        });
    }
    
    public override async Task HandleAsync(GetWeatherRecordsRequest req, CancellationToken ct)
    {
        IEnumerable<WeatherRecord> records;
        
        if (req.Days.HasValue)
        {
            records = await _weatherRepository.GetRecentWeatherAsync(req.Days.Value);
        }
        else
        {
            records = await _weatherRepository.GetAllAsync();
        }
        
        await SendOkAsync(records, ct);
    }
}
