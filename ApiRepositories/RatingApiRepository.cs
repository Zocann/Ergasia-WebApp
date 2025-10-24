using System.Net;
using System.Text;
using Ergasia_WebApp.ApiRepositories.Interfaces;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.DTOs.Rating;
using Ergasia_WebApp.Services;

namespace Ergasia_WebApp.ApiRepositories;

public class RatingApiRepository(IHttpContextAccessor contextAccessor, IHttpClientFactory clientFactory) : IRatingApiRepository
{
    public required HttpContext Context = contextAccessor.HttpContext ?? throw new InvalidOperationException("No HttpContextAccessor");
    private readonly HttpClient _client = clientFactory.CreateClient("API");
    
    public async Task<EmployerRatingDto?> GetEmployerRatingAsync(string employerId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/{employerId}/Ratings/{workerId}");
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<EmployerRatingDto?>(responseString);
    }

    public async Task<List<EmployerRatingDto>?> GetEmployerRatingsAsync(string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Employers/{employerId}/Ratings");
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<EmployerRatingDto>>(responseString);
    }

    public async Task<decimal?> GetEmployerAverageRating(string employerId)
    {
        var response = await _client.GetAsync($"Employers/{employerId}/Average-rating");
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(responseString) ? null : JsonSerializerService.Deserialize<decimal?>(responseString);
    }

    public async Task<EmployerRatingDto?> PostEmployerRatingAsync(string employerId, string workerId, string numRating, string verbalRating,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var json = JsonSerializerService.Serialize(new { verbalRating });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response =
            await _client.PostAsync($"Employers/{employerId}/Ratings?workerId={workerId}&numericalRating={numRating}",
                content);

        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<EmployerRatingDto?>(responseString);
    }

    public async Task<EmployerRatingDto?> PatchEmployerRatingAsync(string employerId, string workerId, string numRating, string verbalRating,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var json = JsonSerializerService.Serialize(new { verbalRating });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response =
            await _client.PatchAsync($"Employers/{employerId}/Ratings/{workerId}?numericalRating={numRating}",
                content);

       if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<EmployerRatingDto?>(responseString);
    }

    public async Task<bool> DeleteEmployerRatingAsync(string employerId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.DeleteAsync($"Employers/{employerId}/Ratings/{workerId}");
        
        if (response.StatusCode == HttpStatusCode.Unauthorized) Context.Response.StatusCode = 401;
        
        return response.IsSuccessStatusCode;
    }

    public async Task<WorkerRatingDto?> GetWorkerRatingAsync(string workerId, string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.GetAsync($"Workers/{workerId}/Ratings/{employerId}");
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerRatingDto?>(responseString);
    }

    public async Task<List<WorkerRatingDto>?> GetWorkerRatingsAsync(string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.GetAsync($"Workers/{workerId}/Ratings");
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<List<WorkerRatingDto>?>(responseString);
    }

    public async Task<decimal?> GetWorkerAverageRating(string workerId)
    {
        var response = await _client.GetAsync($"Workers/{workerId}/Average-rating");
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(responseString) ? null : JsonSerializerService.Deserialize<decimal?>(responseString);
    }

    public async Task<WorkerRatingDto?> PostWorkerRatingAsync(string workerId, string employerId, string numRating, string verbalRating,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var json = JsonSerializerService.Serialize(new { verbalRating });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response =
            await _client.PostAsync($"Workers/{workerId}/Ratings?employerId={employerId}&numericalRating={numRating}",
                content);
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerRatingDto?>(responseString);
    }

    public async Task<WorkerRatingDto?> PatchWorkerRatingAsync(string workerId, string employerId, string numRating, string verbalRating,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var json = JsonSerializerService.Serialize(new { verbalRating });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response =
            await _client.PatchAsync($"Workers/{workerId}/Ratings/{employerId}?numericalRating={numRating}",
                content);
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerRatingDto?>(responseString);
    }

    public async Task<bool> DeleteWorkerRatingAsync(string workerId, string employerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        var response = await _client.DeleteAsync($"Workers/{workerId}/Ratings/{employerId}");
        
        if (response.StatusCode == HttpStatusCode.Unauthorized) Context.Response.StatusCode = 401;
        
        return response.IsSuccessStatusCode;
    }

    public async Task<decimal?> GetJobAverageRating(string jobId)
    {
        var response = await _client.GetAsync($"Employers/employer-id/Jobs/{jobId}/Average-rating");
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(responseString) ? null : JsonSerializerService.Deserialize<decimal?>(responseString);
    }

    public async Task<WorkerJobDto?> PatchJobRatingAsync(string employerId, string jobId, string workerId, string numRating, string verbalRating,
        string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var json = JsonSerializerService.Serialize(new { verbalRating });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response =
            await _client.PatchAsync(
                $"Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}?numericalRating={numRating}",
                content);
        
        if (!ManageResponse(response)) return null;

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializerService.Deserialize<WorkerJobDto?>(responseString);
    }

    public async Task<bool> DeleteJobRatingAsync(string employerId, string jobId, string workerId, string accessToken)
    {
        RegisterAuthorizationHeader(accessToken);
        
        var response = await _client.DeleteAsync($"Employers/{employerId}/Jobs/{jobId}/Ratings/{workerId}");
        if (response.StatusCode == HttpStatusCode.Unauthorized) Context.Response.StatusCode = 401;

        return response.IsSuccessStatusCode;
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