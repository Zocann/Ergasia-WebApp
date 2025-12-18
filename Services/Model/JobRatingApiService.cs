using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class JobRatingApiService(IHttpClientFactory clientFactory) : IJobRatingService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<ServiceResult<float>> GetAverageRatingAsync(string jobId)
    {
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Average-rating");
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<float>.Build.Failure(response.StatusCode);

        var average = await ConvertResponseToFloatAsync(response);
        return average != null ?
            ServiceResult<float>.Build.Success(average.Value, response.StatusCode) :
            ServiceResult<float>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<WorkerJobDto>> PatchAsync(JobRatingDto ratingDto, string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var content = SerializeStringToContent(new VerbalRatingDto(ratingDto.VerbalRating));
        var response =
            await _client.PatchAsync(
                $"Employers/{employerId}/Jobs/{ratingDto.JobId}/Ratings/{ratingDto.WorkerId}?numericalRating={ratingDto.NumericalRating}",
                content);
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerJobDto>.Build.Failure(response.StatusCode);

        var jobRatingDto = await ConvertResponseToRatingDtoAsync(response);
        return jobRatingDto != null ?
            ServiceResult<WorkerJobDto>.Build.Success(jobRatingDto, response.StatusCode) :
            ServiceResult<WorkerJobDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string employerId, string jobId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.DeleteAsync($"Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}");
        
        return response.IsSuccessStatusCode ?
            ServiceResult<bool>.Build.Success(true, response.StatusCode) :
            ServiceResult<bool>.Build.Failure(response.StatusCode);
    }
    
    //Helpers
    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    private static async Task<WorkerJobDto?> ConvertResponseToRatingDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerJobDto>(responseString);
    }

    private static async Task<float?> ConvertResponseToFloatAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(responseString) ? null : JsonSerializerService.Deserialize<float>(responseString);
    }

    private static StringContent SerializeStringToContent(VerbalRatingDto verbalRating)
    {
        var json = JsonSerializerService.Serialize(verbalRating);
        return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}