using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Services;

namespace Ergasia_WebApp.ApiRepositories;

public class UserApiRepository(IHttpClientFactory clientFactory)
    : IUserApiRepository
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");

    public async Task<UserDto?> GetAsync(string userId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Users/{userId}");
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<UserDto>(responseString);
    }

    public async Task<bool?> ValidateEmailAsync(string email)
    {
        var response = await _client.GetAsync($"Users/email/{email}");
        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => null,
            HttpStatusCode.NotFound => true,
            _ => false
        };
    }


    public async Task<string?> GetRoleAsync(string userId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Users/role/{userId}");
        if (!ManageResponse(response)) return null;
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var jsonContent = JsonSerializerService.Serialize(loginDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.PostAsync("Users/login", content);
        
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<UserDto>(responseString);
    }

    public async Task<UserDto?> RegisterAsync(RegisterDto registerDto, string role)
    {
        var jsonContent = JsonSerializerService.Serialize(registerDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.PostAsync($"Users/register?userType={role}", content);
        
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<UserDto>(responseString);
    }

    public async Task<UserDto?> UploadPictureAsync(IFormFile file, string userId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        using var content = new MultipartFormDataContent();
        
        await using var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        // "file" here must match the parameter name your API expects
        content.Add(fileContent, "file", file.FileName);

        var response = await _client.PostAsync($"Users/{userId}/picture", content);

        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<UserDto>(responseString);
    }

    public async Task<UserDto?> RefreshTokensAsync(string refreshToken)
    {
        _client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken}");
        
        var response = await _client.GetAsync("Users/refresh-token");
        
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializerService.Deserialize<UserDto>(responseString);
        }

        return null;
    }

    private void RegisterAuthorizationHeader(string accessToken)
    {
        if (_client.DefaultRequestHeaders.Authorization == null) 
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    private bool ManageResponse(HttpResponseMessage response)
    {
        var httpContextAccessor = new HttpContextAccessor();
        if (httpContextAccessor.HttpContext == null) throw new ApplicationException("No context accessor");
        if (response.StatusCode == HttpStatusCode.Unauthorized) httpContextAccessor.HttpContext.Response.StatusCode = 401;
        return response.IsSuccessStatusCode;
    }
}