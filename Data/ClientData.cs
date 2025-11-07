namespace Ergasia_WebApp.Data;

public class ClientData(IHttpContextAccessor httpContextAccessor)
{
    public string? Id { get; private set; } = httpContextAccessor.HttpContext?.Request.Cookies["userId"];
    public string? Role { get; private set; } = httpContextAccessor.HttpContext?.Request.Cookies["role"];
    public string? AccessToken { get; private set; } = httpContextAccessor.HttpContext?.Request.Cookies["accessToken"];
}