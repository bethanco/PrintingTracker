
using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Data;
using PrintingJobTracker.Domain;

namespace PrintingJobTracker.Services;

public class WorkOrdersService : IWorkOrdersService
{
    private readonly OpsDbContext _db;
    private static readonly Dictionary<JobStatus, JobStatus?> Next = new()
    {
        { JobStatus.Received, JobStatus.Printing },
        { JobStatus.Printing, JobStatus.Inserting },
        { JobStatus.Inserting, JobStatus.Mailed },
        { JobStatus.Mailed, JobStatus.Delivered },
        { JobStatus.Delivered, null },
        { JobStatus.Exception, null }
    };

    public WorkOrdersService(OpsDbContext db) => _db = db;

    public async Task<List<Job>> GetJobsAsync(JobStatus? statusFilter = null)
    {
        var q = _db.Jobs.AsNoTracking().OrderByDescending(j => j.CreatedAt);
        if (statusFilter.HasValue) return await q.Where(j => j.CurrentStatus == statusFilter.Value).ToListAsync();
        return await q.ToListAsync();
    }

    public Task<Job?> GetJobAsync(int id) =>
        _db.Jobs.Include(j => j.History.OrderBy(h => h.ChangedAt)).FirstOrDefaultAsync(j => j.Id == id);

    public async Task<Job> CreateJobAsync(Job job)
    {
        job.CurrentStatus = JobStatus.Received;
        job.CreatedAt = DateTime.UtcNow;
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
        _db.JobStatusHistory.Add(new JobStatusHistory
        {
            JobId = job.Id,
            Status = JobStatus.Received,
            Note = "Created",
            ChangedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return job;
    }

    public async Task AdvanceStatusAsync(int jobId)
    {
        var job = await _db.Jobs.FirstAsync(j => j.Id == jobId);
        var next = Next[job.CurrentStatus];
        if (next is null) return;
        job.CurrentStatus = next.Value;
        _db.JobStatusHistory.Add(new JobStatusHistory
        {
            JobId = jobId,
            Status = next.Value,
            ChangedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    public async Task MarkExceptionAsync(int jobId, string note)
    {
        if (string.IsNullOrWhiteSpace(note)) throw new ArgumentException("Note is required for exceptions.");
        var job = await _db.Jobs.FirstAsync(j => j.Id == jobId);
        job.CurrentStatus = JobStatus.Exception;
        _db.JobStatusHistory.Add(new JobStatusHistory
        {
            JobId = jobId,
            Status = JobStatus.Exception,
            Note = note,
            ChangedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    public async Task<Dictionary<JobStatus, int>> GetCountsByStatusAsync()
    {
        var groups = await _db.Jobs.GroupBy(j => j.CurrentStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync();
        return groups.ToDictionary(x => x.Status, x => x.Count);
    }
}
