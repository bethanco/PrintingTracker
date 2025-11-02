using PrintingJobTracker.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PrintingJobTracker.Application.WorkOrders;

namespace PrintingJobTracker.Services
{
    public interface IWorkOrdersService
    {
        Task<List<Job>> GetWorkOrdersAsync(JobStatus? statusFilter = null);
        Task<Job?> GetJobAsync(int id);
        Task<Job> CreateJobAsync(Job job);
        Task<Job?> AdvanceStatusAsync(int jobId);
        Task<Job?> MarkExceptionAsync(int jobId, string note);
        Task<Dictionary<JobStatus, int>> GetCountsByStatusAsync();

        // Optional dashboard/KPI helpers
        Task<KpiSummary> GetKpisAsync(CancellationToken ct = default);
        Task<List<DailyTransitionPoint>> GetDailyTransitionsAsync(int days = 14, CancellationToken ct = default);
    }
}
