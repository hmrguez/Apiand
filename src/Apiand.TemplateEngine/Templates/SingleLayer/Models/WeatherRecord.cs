using System;
using Dapper.Contrib.Extensions;

namespace XXXnameXXX.Models;

[Table("WeatherRecords")]
public class WeatherRecord
{
    [Key]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
