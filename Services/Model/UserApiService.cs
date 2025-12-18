using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.User;
using Ergasia_WebApp.Helpers;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class UserApiService(IHttpClientFactory clientFactory)
    : IUserService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");

    public async Task<ServiceResult<UserDto>> GetAsync(string userId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Users/{userId}");

        if (!response.IsSuccessStatusCode)
            return ServiceResult<UserDto>.Build.Failure(response.StatusCode);

        var userDto = await ConvertResponseToUserDtoAsync(response);
        return userDto != null ?
            ServiceResult<UserDto>.Build.Success(userDto, response.StatusCode) :
            ServiceResult<UserDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<bool>> ValidateEmailAsync(string email)
    {
        var response = await _client.GetAsync($"Users/email/{email}");
        if (!response.IsSuccessStatusCode)
            return ServiceResult<bool>.Build.Failure(response.StatusCode);
        
        var result = await ConvertResponseToBoolAsync(response);
        return result != null ?
            ServiceResult<bool>.Build.Success(result.Value, response.StatusCode) :
            ServiceResult<bool>.Build.Failure(response.StatusCode);
    }


    public async Task<ServiceResult<string>> GetRoleAsync(string userId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);

        var response = await _client.GetAsync($"Users/role/{userId}");

        if (!response.IsSuccessStatusCode)
            return ServiceResult<string>.Build.Failure(response.StatusCode);

        var role = await ConvertResponseToStringAsync(response);
        return UserRoleValidator.Validate(role) ?
            ServiceResult<string>.Build.Success(role, response.StatusCode) :
            ServiceResult<string>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<UserDto>> LoginAsync(LoginDto loginDto)
    {
        var content = SerializeLoginDtoToContent(loginDto);
        var response = await _client.PostAsync("Users/login", content);

        if (!response.IsSuccessStatusCode)
            return ServiceResult<UserDto>.Build.Failure(response.StatusCode);

        var userDto = await ConvertResponseToUserDtoAsync(response);
        return userDto != null ?
            ServiceResult<UserDto>.Build.Success(userDto, response.StatusCode) :
            ServiceResult<UserDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto, string role)
    {
        var content = SerializeRegisterDtoToContent(registerDto);
        var response = await _client.PostAsync($"Users/register?userType={role}", content);

        if (!response.IsSuccessStatusCode)
            return ServiceResult<UserDto>.Build.Failure(response.StatusCode);

        var userDto = await ConvertResponseToUserDtoAsync(response);
        return userDto != null ?
            ServiceResult<UserDto>.Build.Success(userDto, response.StatusCode) :
            ServiceResult<UserDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<bool>> UploadPictureAsync(IFormFile file, string userId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);

        var content = CreateContentFromFileAsync(file);
        var response = await _client.PostAsync($"Users/{userId}/picture", content);

        if (!response.IsSuccessStatusCode)
            return ServiceResult<bool>.Build.Failure(response.StatusCode);

        var result = await ConvertResponseToBoolAsync(response);
        return result != null ?
            ServiceResult<bool>.Build.Success(true, response.StatusCode) :
            ServiceResult<bool>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<UserDto>> RefreshTokenAsync(string refreshToken)
    {
        RegisterRefreshTokenHeader(refreshToken);
        var response = await _client.GetAsync("Users/refresh-token");

        if (!response.IsSuccessStatusCode)
            return ServiceResult<UserDto>.Build.Failure(response.StatusCode);
        
        var userDto = await ConvertResponseToUserDtoAsync(response);
        return userDto != null ?
            ServiceResult<UserDto>.Build.Success(userDto, response.StatusCode) :
            ServiceResult<UserDto>.Build.Failure(response.StatusCode);
    }

    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }
    
    private void RegisterRefreshTokenHeader(string refreshToken)
    {
        _client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken}");
    }

    private static async Task<UserDto?> ConvertResponseToUserDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<UserDto>(responseString);
    }

    private static async Task<string> ConvertResponseToStringAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
    
    private static async Task<bool?> ConvertResponseToBoolAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<bool>(responseString);
    }

    private static StringContent SerializeLoginDtoToContent(LoginDto loginDto)
    {
        var jsonContent = JsonSerializerService.Serialize(loginDto);
        return new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
    }

    private static StringContent SerializeRegisterDtoToContent(RegisterDto registerDto)
    {
        var jsonContent = JsonSerializerService.Serialize(registerDto);
        return new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
    }

    private static MultipartFormDataContent CreateContentFromFileAsync(IFormFile file)
    {
        var content = new MultipartFormDataContent();

        var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        content.Add(fileContent, "file", file.FileName);
        return content;
    }
}