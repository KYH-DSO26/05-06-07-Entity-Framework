using System.Text.Json.Serialization;

namespace FailSafeRentals.Data;

public class WeatherResponse
{
    [JsonPropertyName("current_weather")]
    public CurrentWeather? CurrentWeather { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("windspeed")]
    public double Windspeed { get; set; }
}
