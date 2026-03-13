using System.Text.Json;

namespace CitizensApi.Services;

public class ExternalObjectService
{
    private readonly HttpClient _httpClient;
    private readonly string _objectsApiUrl;
    private readonly ILogger<ExternalObjectService> _logger;

    public ExternalObjectService(HttpClient httpClient, IConfiguration configuration, ILogger<ExternalObjectService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _objectsApiUrl = configuration["AppSettings:ObjectsApiUrl"] ?? "https://api.restful-api.dev/objects";
    }

    public async Task<string> GetRandomObjectNameAsync()
    {
        _logger.LogInformation("External API request executed");

        var response = await _httpClient.GetAsync(_objectsApiUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(json);

        var items = document.RootElement.EnumerateArray().ToList();

        if (items.Count == 0)
        {
            return "Unknown Asset";
        }

        var random = new Random();
        var selected = items[random.Next(items.Count)];

        if (selected.TryGetProperty("name", out var nameProperty))
        {
            return nameProperty.GetString() ?? "Unknown Asset";
        }

        return "Unknown Asset";
    }
}