using System.Net.Mime;
using System.Text;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Services.Model.Interfaces;

namespace Ergasia_WebApp.Services.Model;

public class EmployerRatingApiService(IHttpClientFactory clientFactory)
    : IEmployerRatingService
{
    private readonly HttpClient _client = clientFactory.CreateClient("API");

    public async Task<ServiceResult<EmployerRatingDto>> GetAsync(string employerId, string workerId,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/{employerId}/Ratings/{workerId}");

        if (!response.IsSuccessStatusCode)
            return ServiceResult<EmployerRatingDto>.Build.Failure(response.StatusCode);

        var employerRatingDto = await ConvertResponseToRatingDtoAsync(response);
        return employerRatingDto != null ? 
            ServiceResult<EmployerRatingDto>.Build.Success(employerRatingDto, response.StatusCode) :
            ServiceResult<EmployerRatingDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<IEnumerable<EmployerRatingDto>>> GetAllAsync(string employerId,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/{employerId}/Ratings");

        if (!response.IsSuccessStatusCode)
            return ServiceResult<IEnumerable<EmployerRatingDto>>.Build.Failure(response.StatusCode);

        var enumerableRatingDto = await ConvertResponseToEnumerableRatingDtoAsync(response);
        return enumerableRatingDto != null ? 
            ServiceResult<IEnumerable<EmployerRatingDto>>.Build.Success(enumerableRatingDto, response.StatusCode) :
            ServiceResult<IEnumerable<EmployerRatingDto>>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<float>> GetAverageRatingAsync(string employerId)
    {
        var response = await _client.GetAsync($"Employers/{employerId}/Average-rating");

        if (!response.IsSuccessStatusCode)
            return ServiceResult<float>.Build.Failure(response.StatusCode);

        var averageRating = await ConvertResponseToFloatAsync(response);
        return averageRating != null ?
            ServiceResult<float>.Build.Success(averageRating.Value, response.StatusCode) :
            ServiceResult<float>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<EmployerRatingDto>> PostAsync(RatingDto ratingDto,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);

        var content = SerializeStringToContent(new VerbalRatingDto(ratingDto.VerbalRating));
        var response =
            await _client.PostAsync(
                $"Employers/{ratingDto.EmployerId}/Ratings?workerId={ratingDto.WorkerId}&numericalRating={ratingDto.NumericalRating}",
                content);

        if (!response.IsSuccessStatusCode)
            return ServiceResult<EmployerRatingDto>.Build.Failure(response.StatusCode);

        var employerRatingDto = await ConvertResponseToRatingDtoAsync(response);
        return employerRatingDto != null ? 
            ServiceResult<EmployerRatingDto>.Build.Success(employerRatingDto, response.StatusCode) :
            ServiceResult<EmployerRatingDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<EmployerRatingDto>> PatchAsync(RatingDto ratingDto,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);

        var content = SerializeStringToContent(new VerbalRatingDto(ratingDto.VerbalRating));
        var response =
            await _client.PatchAsync(
                $"Employers/{ratingDto.EmployerId}/Ratings/{ratingDto.WorkerId}?numericalRating={ratingDto.NumericalRating}",
                content);

        if (!response.IsSuccessStatusCode)
            return ServiceResult<EmployerRatingDto>.Build.Failure(response.StatusCode);

        var employerRatingDto = await ConvertResponseToRatingDtoAsync(response);
        return employerRatingDto != null ? 
            ServiceResult<EmployerRatingDto>.Build.Success(employerRatingDto, response.StatusCode) :
            ServiceResult<EmployerRatingDto>.Build.Failure(response.StatusCode);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string employerId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.DeleteAsync($"Employers/{employerId}/Ratings/{workerId}");

        return response.IsSuccessStatusCode ?
            ServiceResult<bool>.Build.Success(true, response.StatusCode) :
            ServiceResult<bool>.Build.Failure(response.StatusCode);
    }

    //Helpers
    private void RegisterAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
    }

    private static async Task<EmployerRatingDto?> ConvertResponseToRatingDtoAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<EmployerRatingDto>(responseString);
    }

    private static async Task<IEnumerable<EmployerRatingDto>?> ConvertResponseToEnumerableRatingDtoAsync(
        HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<IEnumerable<EmployerRatingDto>>(responseString);
    }

    private static async Task<float?> ConvertResponseToFloatAsync(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(responseString) ? null : JsonSerializerService.Deserialize<float>(responseString);
    }

    private static StringContent SerializeStringToContent(VerbalRatingDto verbalRatingDto)
    {
        var json = JsonSerializerService.Serialize(verbalRatingDto);
        return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}