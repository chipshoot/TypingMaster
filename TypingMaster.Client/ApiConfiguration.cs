namespace TypingMaster.Client;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
}

public interface IApiConfiguration
{
    string BuildApiUrl(string endpoint);
}

public class ApiConfiguration : IApiConfiguration
{
    private readonly ApiSettings _apiSettings;

    public ApiConfiguration(ApiSettings apiSettings)
    {
        _apiSettings = apiSettings;
    }

    public string BuildApiUrl(string endpoint)
    {
        return $"{_apiSettings.BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}