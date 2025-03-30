using FastEndpoints;
using XXXnameXXX.Data;
using XXXnameXXX.Models;

namespace XXXnameXXX.Endpoints.Weather;

public class AddWeatherRecordRequest
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
}

public class AddWeatherRecordResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddWeatherRecordEndpoint : Endpoint<AddWeatherRecordRequest, AddWeatherRecordResponse>
{
    private readonly IWeatherRepository _weatherRepository;
    
    public AddWeatherRecordEndpoint(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }
    
    public override void Configure()
    {
        Post("/api/weather");
        Summary(s => {
            s.Summary = "Adds a new weather record";
            s.Description = "Creates a new weather record in the database";
        });
    }
    
    public override async Task HandleAsync(AddWeatherRecordRequest req, CancellationToken ct)
    {
        var record = new WeatherRecord
        {
            Date = req.Date,
            TemperatureC = req.TemperatureC,
            Summary = req.Summary
        };
        
        var id = await _weatherRepository.AddAsync(record);
        
        await SendAsync(new AddWeatherRecordResponse
        {
            Id = id,
            Message = "Weather record created successfully"
        }, cancellation: ct);
    }
}
