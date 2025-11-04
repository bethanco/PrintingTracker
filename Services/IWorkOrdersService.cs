
using PrintingJobTracker.Domain;

namespace PrintingJobTracker.Services;

public interface IWorkOrdersService
{
    Task<List<Job>> GetJobsAsync(JobStatus? statusFilter = null);
    Task<Job?> GetJobAsync(int id);
    Task<Job> CreateJobAsync(Job job);
    Task AdvanceStatusAsync(int jobId);
    Task MarkExceptionAsync(int jobId, string note);
    Task<Dictionary<JobStatus, int>> GetCountsByStatusAsync();
}
