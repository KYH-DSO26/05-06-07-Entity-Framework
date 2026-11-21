using System.Net.Http.Json;

namespace FailSafeRentals.Data;

public class WeatherService
{
    private static readonly HttpClient _http = new();

    public async Task<CurrentWeather?> GetGothenburgWeatherAsync()
    {
        string baseUrl = "https://api.open-meteo.com/v1/forecast?";
        string queryParams = "latitude=57.7089&longitude=11.9746&current_weather=true";

        var response = await _http.GetFromJsonAsync<WeatherResponse>(baseUrl + queryParams);
        return response?.CurrentWeather;
    }
}
