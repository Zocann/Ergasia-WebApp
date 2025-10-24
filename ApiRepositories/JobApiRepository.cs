using System.Net;
using System.Text;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services;

namespace Ergasia_WebApp.ApiRepositories;

public class JobApiRepository(IHttpContextAccessor contextAccessor, IHttpClientFactory clientFactory) : IJobApiRepository
{
    public required HttpContext Context = contextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContextAccessor");
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<JobDto?> GetAsync(string jobId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}");
        
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<JobDto>(responseString);
    }

    public async Task<List<JobDto>?> GetFromEmployerAsync(string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/{employerId}/Jobs");
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<JobDto>?>(responseString);
    }

    public async Task<List<JobDto>?> GetAllUpcomingAsync()
    {
        var response = await _client.GetAsync("Jobs");
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<JobDto>>(responseString);
    }

    public async Task<JobDto?> AddAsync(JobDto jobDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var jsonContent = JsonSerializerService.Serialize(jobDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"Employers/{jobDto.EmployerId}/Jobs", content);
        
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<JobDto>(responseString);
    }

    public async Task<JobDto?> PatchAsync(JobDto jobDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var jsonContent = JsonSerializerService.Serialize(jobDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync($"Employers/{jobDto.EmployerId}/Jobs/{jobDto.Id}", content);
        
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<JobDto>(responseString);
    }

    public async Task<WorkerJobDto?> GetWorkerJobAsync(string jobId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Workers/{workerId}");
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerJobDto>(responseString);
    }

    public async Task<List<WorkerJobDto>?> GetWorkerJobsByJobIdAsync(string jobId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Workers");
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<WorkerJobDto>>(responseString);
    }

    public async Task<List<WorkerJobDto>?> GetWorkerJobsByWorkerIdAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}/Jobs");
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<WorkerJobDto>>(responseString);
    }

    public async Task<WorkerJobDto?> PostWorkerJobAsync(string jobId, string employerId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.PostAsync($"Employers/{employerId}/Jobs/{jobId}/Workers/{workerId}", null);
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerJobDto?>(responseString);
    }

    public async Task<int?> GetAvailableWorkSpotsAsync(string jobId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Work-spots");
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return int.TryParse(responseString, out var result) ? result : null;
    }

    
    
    
    
    public async Task<JobRequestDto?> GetJobRequestAsync(string jobId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Requests/{workerId}");
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<JobRequestDto>(responseString);
    }

    public async Task<List<JobRequestDto>?> GetJobRequestsByJobIdAsync(string jobId, string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/{employerId}/Jobs/{jobId}/Requests");
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<JobRequestDto>>(responseString);
    }

    public async Task<List<JobRequestDto>?> GetJobRequestsByWorkerIdAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}/Jobs/Requests");
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<JobRequestDto>>(responseString);
    }

    public async Task<JobRequestDto?> PostJobRequestAsync(string jobId, string workerId, string? message, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var json = JsonSerializerService.Serialize(new { message });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"Employers/employer-id/Jobs/{jobId}/Requests/{workerId}", content);
        
        if (!ManageResponse(response)) return null;
        
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<JobRequestDto>(responseString);
    }

    public async Task<bool> DeleteJobRequest(string jobId, string employerId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.DeleteAsync($"Employers/{employerId}/Jobs/{jobId}/Requests/{workerId}");
        return ManageResponse(response);
    }
    
    //Helper functions
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