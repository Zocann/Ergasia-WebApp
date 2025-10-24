using System.Net;
using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services;

namespace Ergasia_WebApp.ApiRepositories;

public class WorkerApiRepository(IHttpContextAccessor contextAccessor, IHttpClientFactory clientFactory) : IWorkerApiRepository
{
    public required HttpContext Context = contextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContextAccessor");
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<WorkerDto?> GetAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}");
        if (! ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerDto>(responseString);
    }

    public async Task<List<WorkerDto>?> GetAllAsync(string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync("Workers");
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<WorkerDto>>(responseString);
    }

    public async Task<WorkerDto?> PatchAsync(WorkerDto workerDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var jsonContent = JsonSerializerService.Serialize(workerDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.PatchAsync($"Workers/{workerDto.Id}", content);
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerDto?>(responseString);
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