using System.Net;
using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.Services;

namespace Ergasia_WebApp.ApiRepositories;

public class EmployerApiRepository(IHttpContextAccessor contextAccessor, IHttpClientFactory clientFactory) : IEmployerApiRepository
{
    public required HttpContext Context = contextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContextAccessor");
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<EmployerDto?> GetAsync(string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/{employerId}");
        
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<EmployerDto>(responseString);
    }

    public async Task<EmployerDto?> PatchAsync(EmployerDto employerDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var jsonContent = JsonSerializerService.Serialize(employerDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.PatchAsync($"Employers/{employerDto.Id}", content);

        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<EmployerDto?>(responseString);
    }
    
    private void RegisterAuthorizationHeader(string accessToken)
    {
        if (_client.DefaultRequestHeaders.Authorization == null) 
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    private bool ManageResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized) Context.Response.StatusCode = 401;
        return response.IsSuccessStatusCode;
    }
}