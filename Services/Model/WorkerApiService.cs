using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Worker;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class WorkerApiService(IHttpClientFactory clientFactory) : IWorkerService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<ServiceResult<WorkerDto>> GetAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerDto>.Build.Failure(response.StatusCode);

        var workerDto = await ConvertResponseToWorkerDtoAsync(response);
        return workerDto != null ? 
            ServiceResult<WorkerDto>.Build.Success(workerDto, response.StatusCode) :
            ServiceResult<WorkerDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<WorkerDto>>> GetAllAsync(string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync("Workers");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<WorkerDto>>.Build.Failure(response.StatusCode);
        
        var enumerableWorkerDto = await ConvertResponseToEnumerableWorkerDtoAsync(response);
        return enumerableWorkerDto != null ? 
            ServiceResult<IEnumerable<WorkerDto>>.Build.Success(enumerableWorkerDto, response.StatusCode) :
            ServiceResult<IEnumerable<WorkerDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<WorkerDto>> PatchAsync(WorkerDto workerDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var content = SerializeWorkerDtoToContent(workerDto);
        
        var response = await _client.PatchAsync($"Workers/{workerDto.Id}", content);
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerDto>.Build.Failure(response.StatusCode);

        
        var worker = await ConvertResponseToWorkerDtoAsync(response);
        return worker != null ? 
            ServiceResult<WorkerDto>.Build.Success(workerDto, response.StatusCode) :
            ServiceResult<WorkerDto>.Build.Failure(response.StatusCode);
    }

    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }
    
    private static async Task<WorkerDto?> ConvertResponseToWorkerDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerDto>(responseString);
    }
    
    private static async Task<IEnumerable<WorkerDto>?> ConvertResponseToEnumerableWorkerDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<IEnumerable<WorkerDto>>(responseString);
    }

    private static StringContent SerializeWorkerDtoToContent(WorkerDto workerDto)
    {
        var jsonContent = JsonSerializerService.Serialize(workerDto);
        return new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}