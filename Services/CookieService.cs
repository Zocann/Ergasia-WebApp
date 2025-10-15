using Ergasia_WebApp.Services.Interfaces;

namespace Ergasia_WebApp.Services;

public class CookieService(IHttpContextAccessor contextAccessor) : ICookieService
{
    public void AddCookie(string cookieName, string cookie, DateTime expiration)
    {
        contextAccessor.HttpContext?.Response.Cookies.Append(cookieName, cookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = expiration
        });
    }

    public void AddCookie(string cookieName, string cookie)
    {
        contextAccessor.HttpContext?.Response.Cookies.Append(cookieName, cookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true
        });
    }

    public void RevokeCookie(string cookieName)
    {
        contextAccessor.HttpContext?.Response.Cookies.Delete(cookieName);
    }
}