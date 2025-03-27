namespace TypingMaster.Client.Models;

public class WebServiceResponse
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public System.Net.HttpStatusCode StatusCode { get; set; }
}
