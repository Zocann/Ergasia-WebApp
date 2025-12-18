using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class WorkerJobApiService(IHttpClientFactory clientFactory) : IWorkerJobService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
     public async Task<ServiceResult<WorkerJobDto>> GetAsync(string jobId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Workers/{workerId}");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerJobDto>.Build.Failure(response.StatusCode);

        var workerJobDto = await ConvertResponseToWorkerJobDtoAsync(response);
        return workerJobDto != null ?
            ServiceResult<WorkerJobDto>.Build.Success(workerJobDto, response.StatusCode) :
            ServiceResult<WorkerJobDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetByJobIdAsync(string jobId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Workers");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<WorkerJobDto>>.Build.Failure(response.StatusCode);
        
        var enumerableWorkerJobDto = await ConvertResponseToEnumerableWorkerJobDtoAsync(response);
        return enumerableWorkerJobDto != null ?
            ServiceResult<IEnumerable<WorkerJobDto>>.Build.Success(enumerableWorkerJobDto, response.StatusCode) :
            ServiceResult<IEnumerable<WorkerJobDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<WorkerJobDto>>> GetByWorkerIdAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}/Jobs");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<WorkerJobDto>>.Build.Failure(response.StatusCode);
        
        var enumerableWorkerJobDto = await ConvertResponseToEnumerableWorkerJobDtoAsync(response);
        return enumerableWorkerJobDto != null ?
            ServiceResult<IEnumerable<WorkerJobDto>>.Build.Success(enumerableWorkerJobDto, response.StatusCode) :
            ServiceResult<IEnumerable<WorkerJobDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<WorkerJobDto>> PostAsync(string jobId, string employerId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.PostAsync($"Employers/{employerId}/Jobs/{jobId}/Workers/{workerId}", null);
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerJobDto>.Build.Failure(response.StatusCode);
        
        var workerJobDto = await ConvertResponseToWorkerJobDtoAsync(response);
        return workerJobDto != null ?
            ServiceResult<WorkerJobDto>.Build.Success(workerJobDto, response.StatusCode) :
            ServiceResult<WorkerJobDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<int>> GetAvailableWorkSpotsAsync(string jobId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Work-spots");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<int>.Build.Failure(response.StatusCode);

        var workSpots = await ConvertResponseToIntAsync(response);
        return workSpots != null ?
            ServiceResult<int>.Build.Success(workSpots.Value, response.StatusCode) :
            ServiceResult<int>.Build.Failure(response.StatusCode);
    }
    
    //Helper functions
    private void RegisterAuthorizationHeader(string accessToken)
    {
        if (_client.DefaultRequestHeaders.Authorization == null) 
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }
    
    private static async Task<WorkerJobDto?> ConvertResponseToWorkerJobDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerJobDto>(responseString);
    }
    private static async Task<int?> ConvertResponseToIntAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return int.TryParse(responseString, out var result) ? result : null;
    }
    
    private static async Task<IEnumerable<WorkerJobDto>?> ConvertResponseToEnumerableWorkerJobDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<IEnumerable<WorkerJobDto>>(responseString);
    }
}