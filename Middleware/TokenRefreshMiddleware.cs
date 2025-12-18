using System.Net;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services;
using Ergasia_WebApp.Services.Interfaces;

namespace Ergasia_WebApp.Middleware;

public class TokenRefreshMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICookieService cookieService, IHttpClientFactory clientFactory)
    {
        await next(context);
        var path = context.Request.Path;

        if (!IsStatusCodeUnauthorized(context.Response.StatusCode)) return;

        var refreshToken = GetRefreshTokenWithContext(context);
        if (string.IsNullOrEmpty(refreshToken)) context.Response.Redirect("/Account/Logout");

        var client = clientFactory.CreateClient("API");
        client = AddHeaderToClient(headerName: "Cookie",
            headerValue: $"refreshToken={refreshToken}", client);

        var refreshResponse = await GetDataFromApiAsync(client, requestUri: "Users/refresh-token");
        if (!refreshResponse.IsSuccessStatusCode) context.Response.Redirect("/Account/Logout");

        var userAsJson = await ConvertHttpResponseMessageToStringAsync(refreshResponse);
        var userDto = DeserializeUserDtoFromJson(userAsJson);

        if (userDto?.AccessToken == null || userDto.RefreshToken == null ||
            userDto.RefreshTokenExpiration == null) return;

        client = AddHeaderToClient(headerName: "Authorization", headerValue: $"Bearer {userDto.AccessToken}", client);
        var roleResponse = await GetDataFromApiAsync(client, requestUri: $"Users/role/{userDto.Id}");

        if (!roleResponse.IsSuccessStatusCode) context.Response.Redirect("/Error");

        var role = await ConvertHttpResponseMessageToStringAsync(roleResponse);
        if (string.IsNullOrEmpty(role)) context.Response.Redirect("/Error");

        UpdateCookies(cookieService, userDto, role);

        context.Response.Redirect(path);
    }

    private static bool IsStatusCodeUnauthorized(int statusCode)
    {
        return statusCode == (int)HttpStatusCode.Unauthorized;
    }

    private static string? GetRefreshTokenWithContext(HttpContext context)
    {
        return context.Request.Cookies["refreshToken"];
    }

    private static HttpClient AddHeaderToClient(string headerName, string headerValue, HttpClient client)
    {
        client.DefaultRequestHeaders.Add(headerName, headerValue);
        return client;
    }

    private static async Task<HttpResponseMessage> GetDataFromApiAsync(HttpClient client, string requestUri)
    {
        return await client.GetAsync(requestUri);
    }

    private static async Task<string> ConvertHttpResponseMessageToStringAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }

    private static UserDto? DeserializeUserDtoFromJson(string json)
    {
        return JsonSerializerService.Deserialize<UserDto>(json);
    }

    private static bool IsValidUser(UserDto? userDto)
    {
        return userDto is { AccessToken: not null, RefreshToken: not null, RefreshTokenExpiration: not null };
    }

    private static void UpdateCookies(ICookieService cookieService, UserDto userDto, string role)
    {
        cookieService.AddCookie("userRole", role, (DateTime)userDto.RefreshTokenExpiration!);

        cookieService.AddCookie("refreshToken", userDto!.RefreshToken,
            (DateTime)userDto.RefreshTokenExpiration);

        cookieService.AddCookie("accessToken", userDto!.AccessToken);

        cookieService.AddCookie("userId", userDto.Id,
            (DateTime)userDto.RefreshTokenExpiration);

        cookieService.AddCookie("userName", $"{userDto.FirstName} {userDto.LastName}",
            (DateTime)userDto.RefreshTokenExpiration);
    }
}

public static class TokenRefreshMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenRefresh(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenRefreshMiddleware>();
    }
}