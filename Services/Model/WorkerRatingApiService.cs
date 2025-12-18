using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class WorkerRatingApiService(IHttpClientFactory clientFactory) : IWorkerRatingService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");

    public async Task<ServiceResult<WorkerRatingDto>> GetAsync(string workerId, string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}/Ratings/{employerId}");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerRatingDto>.Build.Failure(response.StatusCode);

        var workerRatingDto = await ConvertResponseToRatingDtoAsync(response);
        return workerRatingDto != null ?
            ServiceResult<WorkerRatingDto>.Build.Success(workerRatingDto, response.StatusCode) :
             ServiceResult<WorkerRatingDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<WorkerRatingDto>>> GetAllAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}/Ratings");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<WorkerRatingDto>>.Build.Failure(response.StatusCode);

        var enumerableRatingDto = await ConvertResponseToEnumerableRatingDtoAsync(response);
        return enumerableRatingDto != null ?
            ServiceResult<IEnumerable<WorkerRatingDto>>.Build.Success(enumerableRatingDto, response.StatusCode) :
            ServiceResult<IEnumerable<WorkerRatingDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<float>> GetAverageRatingAsync(string workerId)
    {
        var response = await _client.GetAsync($"Workers/{workerId}/Average-rating");
        if (! response.IsSuccessStatusCode)
            return ServiceResult<float>.Build.Failure(response.StatusCode);

        var averageRating = await ConvertResponseToFloatAsync(response);
        return averageRating != null ?
            ServiceResult<float>.Build.Success(averageRating.Value, response.StatusCode) :
            ServiceResult<float>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<WorkerRatingDto>> PostAsync(RatingDto ratingDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var content = SerializeVerbalRatingToContent(new VerbalRatingDto(ratingDto.VerbalRating));
        var response =
            await _client.PostAsync($"Workers/{ratingDto.WorkerId}/Ratings?employerId={ratingDto.EmployerId}&numericalRating={ratingDto.NumericalRating}",
                content);
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerRatingDto>.Build.Failure(response.StatusCode);

        var workerRatingDto = await ConvertResponseToRatingDtoAsync(response);
        return workerRatingDto != null ?
            ServiceResult<WorkerRatingDto>.Build.Success(workerRatingDto, response.StatusCode) :
            ServiceResult<WorkerRatingDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<WorkerRatingDto>> PatchAsync(RatingDto ratingDto, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var content = SerializeVerbalRatingToContent(new VerbalRatingDto(ratingDto.VerbalRating));
        var response =
            await _client.PatchAsync($"Workers/{ratingDto.WorkerId}/Ratings/{ratingDto.EmployerId}?numericalRating={ratingDto.NumericalRating}",
                content);
        
        if (! response.IsSuccessStatusCode)
            return ServiceResult<WorkerRatingDto>.Build.Failure(response.StatusCode);

        var workerRatingDto = await ConvertResponseToRatingDtoAsync(response);
        return workerRatingDto != null ?
            ServiceResult<WorkerRatingDto>.Build.Success(workerRatingDto, response.StatusCode) :
            ServiceResult<WorkerRatingDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string workerId, string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.DeleteAsync($"Workers/{workerId}/Ratings/{employerId}");
        
        return response.IsSuccessStatusCode ?
            ServiceResult<bool>.Build.Success(true, response.StatusCode) :
            ServiceResult<bool>.Build.Failure(response.StatusCode);
    }
    
    //Helpers
    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    private static async Task<WorkerRatingDto?> ConvertResponseToRatingDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerRatingDto>(responseString);
    }

    private static async Task<IEnumerable<WorkerRatingDto>?> ConvertResponseToEnumerableRatingDtoAsync(
        HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<IEnumerable<WorkerRatingDto>>(responseString);
    }

    private static async Task<float?> ConvertResponseToFloatAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(responseString) ? null : JsonSerializerService.Deserialize<float>(responseString);
    }

    private static StringContent SerializeVerbalRatingToContent(VerbalRatingDto verbalRatingDto)
    {
        var json = JsonSerializerService.Serialize(verbalRatingDto);
        return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}