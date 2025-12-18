using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class JobApiService(IHttpClientFactory clientFactory) : IJobService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<ServiceResult<JobDto>> GetAsync(string jobId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}");
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<JobDto>.Build.Failure(response.StatusCode);

        var jobDto = await ConvertResponseToJobDtoAsync(response);
        return jobDto != null ?
            ServiceResult<JobDto>.Build.Success(jobDto, response.StatusCode) :
            ServiceResult<JobDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<JobDto>>> GetFromEmployerAsync(string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Employers/{employerId}/Jobs");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<JobDto>>.Build.Failure(response.StatusCode);

        var enumerableJobDto = await ConvertResponseToEnumerableJobDtoAsync(response);
        return enumerableJobDto != null ?
            ServiceResult<IEnumerable<JobDto>>.Build.Success(enumerableJobDto, response.StatusCode) :
            ServiceResult<IEnumerable<JobDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<JobDto>>> GetAllUpcomingAsync()
    {
        var response = await _client.GetAsync("Jobs");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<JobDto>>.Build.Failure(response.StatusCode);

        var enumerableJobDto = await ConvertResponseToEnumerableJobDtoAsync(response);
        return enumerableJobDto != null ?
            ServiceResult<IEnumerable<JobDto>>.Build.Success(enumerableJobDto, response.StatusCode) :
            ServiceResult<IEnumerable<JobDto>>.Build.Failure(response.StatusCode);
    }
    
    public async Task<ServiceResult<JobDto>> AddAsync(JobDto jobDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var content = SerializeJobDtoToContent(jobDto);
        var response = await _client.PostAsync($"Employers/{jobDto.EmployerId}/Jobs", content);
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<JobDto>.Build.Failure(response.StatusCode);
        
        var job = await ConvertResponseToJobDtoAsync(response);
        return job != null ?
            ServiceResult<JobDto>.Build.Success(job, response.StatusCode) :
            ServiceResult<JobDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<JobDto>> PatchAsync(JobDto jobDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var content = SerializeJobDtoToContent(jobDto);
        var response = await _client.PatchAsync($"Employers/{jobDto.EmployerId}/Jobs/{jobDto.Id}", content);
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<JobDto>.Build.Failure(response.StatusCode);
        
        var job = await ConvertResponseToJobDtoAsync(response);
        return job != null ?
            ServiceResult<JobDto>.Build.Success(job, response.StatusCode) :
            ServiceResult<JobDto>.Build.Failure(response.StatusCode);
    }
    
    //Helper functions
    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }
    
    private static async Task<JobDto?> ConvertResponseToJobDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<JobDto>(responseString);
    }
    private static async Task<IEnumerable<JobDto>?> ConvertResponseToEnumerableJobDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<IEnumerable<JobDto>>(responseString);
    }

    private static StringContent SerializeJobDtoToContent(JobDto jobDto)
    {
        var jsonContent = JsonSerializerService.Serialize(jobDto);
        return new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}