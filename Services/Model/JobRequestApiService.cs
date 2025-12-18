using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class JobRequestApiService(IHttpClientFactory clientFactory) : IJobRequestService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<ServiceResult<JobRequestDto>> GetAsync(string jobId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Requests/{workerId}");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<JobRequestDto>.Build.Failure(response.StatusCode);

        var jobRequestDto = await ConvertResponseToJobRequestDtoAsync(response);
        return jobRequestDto != null ? 
            ServiceResult<JobRequestDto>.Build.Success(jobRequestDto, response.StatusCode) :
            ServiceResult<JobRequestDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<JobRequestDto>>> GetByJobIdAsync(string jobId, string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/{employerId}/Jobs/{jobId}/Requests");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<JobRequestDto>>.Build.Failure(response.StatusCode);
        
        var enumerableJobRequestDto = await ConvertResponseToEnumerableJobRequestDtoAsync(response);
        return enumerableJobRequestDto != null ? 
            ServiceResult<IEnumerable<JobRequestDto>>.Build.Success(enumerableJobRequestDto, response.StatusCode) :
            ServiceResult<IEnumerable<JobRequestDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<JobRequestDto>>> GetByWorkerIdAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}/Jobs/Requests");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<JobRequestDto>>.Build.Failure(response.StatusCode);
        
        var enumerableJobRequestDto = await ConvertResponseToEnumerableJobRequestDtoAsync(response);
        return enumerableJobRequestDto != null ? 
            ServiceResult<IEnumerable<JobRequestDto>>.Build.Success(enumerableJobRequestDto, response.StatusCode) :
            ServiceResult<IEnumerable<JobRequestDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<JobRequestDto>> PostAsync(string jobId, string workerId, string? message, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var content = SerializeStringToContent(message);
        var response = await _client.PostAsync($"Employers/employer-id/Jobs/{jobId}/Requests/{workerId}", content);
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<JobRequestDto>.Build.Failure(response.StatusCode);
        
        var jobRequestDto = await ConvertResponseToJobRequestDtoAsync(response);
        return jobRequestDto != null ? 
            ServiceResult<JobRequestDto>.Build.Success(jobRequestDto, response.StatusCode) :
            ServiceResult<JobRequestDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string jobId, string employerId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.DeleteAsync($"Employers/{employerId}/Jobs/{jobId}/Requests/{workerId}");
        return response.IsSuccessStatusCode ?
            ServiceResult<bool>.Build.Success(true, response.StatusCode) :
            ServiceResult<bool>.Build.Failure(response.StatusCode);
    }
    
    //Helper functions
    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }
    
    private static async Task<JobRequestDto?> ConvertResponseToJobRequestDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<JobRequestDto>(responseString);
    }
    private static async Task<IEnumerable<JobRequestDto>?> ConvertResponseToEnumerableJobRequestDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<IEnumerable<JobRequestDto>>(responseString);
    }
    
    private static StringContent SerializeStringToContent(string? input)
    {
        var json = JsonSerializerService.Serialize(new { input });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}