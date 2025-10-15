namespace Ergasia_WebApp.Services.Interfaces;

public interface ICookieService
{
    public void AddCookie(string cookieName, string cookie, DateTime expiration);
    public void AddCookie(string cookieName, string cookie);
    public void RevokeCookie(string cookieName);
}