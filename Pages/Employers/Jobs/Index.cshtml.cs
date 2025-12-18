using System.Net;
using Ergasia_WebApp.Data;
using Ergasia_WebApp.DTOs.Job;
using Ergasia_WebApp.Services.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ergasia_WebApp.Pages.Employers.Jobs;

public class Index(
    IJobService jobService,
    IWorkerJobService workerJobService) : PageModel
{
    private ClientData _clientData = new(new HttpContextAccessor());

    public required List<JobDto> Jobs { get; set; } = [];
    public required List<JobDto> FinishedJobs { get; set; } = [];

    public Dictionary<string, int> JobRequestCount { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (_clientData.Id == null) return RedirectToPage("/Error");
        if (_clientData.AccessToken == null) return Unauthorized();

        var jobServiceResult = await jobService.GetFromEmployerAsync(_clientData.Id, _clientData.AccessToken);
        if (!jobServiceResult.IsSuccess)
        {
            if (jobServiceResult.StatusCode == HttpStatusCode.Unauthorized) return Unauthorized();
            return Page();
        }

        var jobs = jobServiceResult.Data.ToList();

        FinishedJobs = GetFinishedJobs(jobs);
        Jobs = GetUpcommingJobs(jobs);

        foreach (var job in Jobs)
        {
            if (job.Id == null) continue;

            var count = await GetJobRequestsCountAsync(job.Id, _clientData.AccessToken);
            JobRequestCount.Add(job.Id, count);
        }

        return Page();
    }

    private static List<JobDto> GetFinishedJobs(List<JobDto> allJobs)
    {
        return allJobs
            .Where(j => j.DateOfBegin.AddDays(j.Duration) < DateTime.UtcNow)
            .OrderByDescending(j => j.DateOfBegin).ToList();
    }

    private static List<JobDto> GetUpcommingJobs(List<JobDto> allJobs)
    {
        return allJobs
            .Where(j => j.DateOfBegin.AddDays(j.Duration) > DateTime.UtcNow)
            .OrderBy(j => j.DateOfBegin).ToList();
    }

    private async Task<int> GetJobRequestsCountAsync(string jobId, string accessToken)
    {
        var serviceResult = await workerJobService.GetByJobIdAsync(jobId, accessToken);
        return serviceResult.IsSuccess ? serviceResult.Data.Count() : 0;
    }
}