using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Employer;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class EmployerApiService(IHttpClientFactory clientFactory) : IEmployerService
{ 
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<ServiceResult<EmployerDto>> GetAsync(string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/{employerId}");

        if (!response.IsSuccessStatusCode)
            return ServiceResult<EmployerDto>.Build.Failure(response.StatusCode);

        var employerDto = await ConvertResponseToEmployerDtoAsync(response);
        return employerDto != null ? 
            ServiceResult<EmployerDto>.Build.Success(employerDto, response.StatusCode) :
            ServiceResult<EmployerDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<EmployerDto>> PatchAsync(EmployerDto employerDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var content = SerializeEmployerDtoToContent(employerDto);
        var response = await _client.PatchAsync($"Employers/{employerDto.Id}", content);

        if (! response.IsSuccessStatusCode)
            return ServiceResult<EmployerDto>.Build.Failure(response.StatusCode);
        
        var employer = await ConvertResponseToEmployerDtoAsync(response);
        return employer != null ? 
            ServiceResult<EmployerDto>.Build.Success(employerDto, response.StatusCode) :
            ServiceResult<EmployerDto>.Build.Failure(response.StatusCode);
    }
    
    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    private static async Task<EmployerDto?> ConvertResponseToEmployerDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<EmployerDto>(responseString);
    }

    private static StringContent SerializeEmployerDtoToContent(EmployerDto employerDto)
    {
        var jsonContent = JsonSerializerService.Serialize(employerDto);
        return new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}