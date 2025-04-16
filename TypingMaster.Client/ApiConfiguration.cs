namespace TypingMaster.Client;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string AccountService { get; set; } = string.Empty;

    public string AuthService { get; set; } = string.Empty;

    public string CourseService { get; set; } = string.Empty;

    public string PracticeLogService { get; set; } = string.Empty;

    public string ReportService { get; set; } = string.Empty;
}

public interface IApiConfiguration
{
    string BuildApiUrl(string endpoint);
    ApiSettings ApiSettings { get; }
}

public class ApiConfiguration(ApiSettings apiSettings) : IApiConfiguration
{
    public ApiSettings ApiSettings {get; private set; } = apiSettings;

    public string BuildApiUrl(string endpoint)
    {
        return $"{ApiSettings.BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}