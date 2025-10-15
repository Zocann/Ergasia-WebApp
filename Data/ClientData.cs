namespace Ergasia_WebApp.Data;

public class ClientData(IHttpContextAccessor httpContextAccessor)
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    
    
    public bool GetId()
    {
        if (string.IsNullOrEmpty(httpContextAccessor.HttpContext?.Request.Cookies["userId"])) return false;
        Id = httpContextAccessor.HttpContext.Request.Cookies["userId"] ?? throw new InvalidOperationException("No userId in cookies");
        return true;

    }
    
    public bool GetRole()
    {
        if (string.IsNullOrEmpty(httpContextAccessor.HttpContext?.Request.Cookies["userRole"])) return false;
        
        Role = httpContextAccessor.HttpContext.Request.Cookies["userRole"] ?? throw new InvalidOperationException("No userRole in cookies");
        return true;

    }
    
    public bool GetAccessToken()
    {
        if (string.IsNullOrEmpty(httpContextAccessor.HttpContext?.Request.Cookies["accessToken"])) return false;
        AccessToken = httpContextAccessor.HttpContext.Request.Cookies["accessToken"] ?? throw new InvalidOperationException("No accessToken in cookies");
        return true;

    }
}