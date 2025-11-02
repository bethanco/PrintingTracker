using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Data;
using PrintingJobTracker.Models;
using PrintingJobTracker.Application.WorkOrders;

namespace PrintingJobTracker.Services
{
    public class WorkOrdersService : IWorkOrdersService
    {
        private readonly OpsDbContext _db;

        public WorkOrdersService(OpsDbContext db)
        {
            _db = db;
        }

        public async Task<List<Job>> GetWorkOrdersAsync(JobStatus? statusFilter = null)
        {
            var query = _db.Jobs.AsNoTracking().AsQueryable();
            if (statusFilter.HasValue)
                query = query.Where(j => j.CurrentStatus == statusFilter.Value);

            return await query
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<Job?> GetJobAsync(int id)
        {
            var job = await _db.Jobs
                .Include(j => j.StatusHistory)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job != null)
                job.StatusHistory = job.StatusHistory
                    .OrderByDescending(h => h.ChangedAt)
                    .ToList();

            return job;
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            if (job.CurrentStatus == 0)
                job.CurrentStatus = JobStatus.Received;

            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();

            AddHistory(job.Id, job.CurrentStatus, "Job created");
            await _db.SaveChangesAsync();

            return job;
        }

        public async Task<Job?> AdvanceStatusAsync(int jobId)
        {
            var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
            if (job is null) return null;

            var next = GetNextStatus(job.CurrentStatus);
            if (next == job.CurrentStatus) return job;

            job.CurrentStatus = next;
            AddHistory(job.Id, next, "Advanced status");
            await _db.SaveChangesAsync();

            return job;
        }

        public async Task<Job?> MarkExceptionAsync(int jobId, string note)
        {
            var job = await _db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
            if (job is null) return null;

            job.CurrentStatus = JobStatus.Exception;
            AddHistory(job.Id, JobStatus.Exception, string.IsNullOrWhiteSpace(note) ? "Exception" : note);
            await _db.SaveChangesAsync();

            return job;
        }

        public async Task<Dictionary<JobStatus, int>> GetCountsByStatusAsync()
        {
            var dict = await _db.Jobs.AsNoTracking()
                .GroupBy(j => j.CurrentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            foreach (JobStatus st in (JobStatus[])System.Enum.GetValues(typeof(JobStatus)))
                if (!dict.ContainsKey(st)) dict[st] = 0;

            return dict;
        }

        public async Task<KpiSummary> GetKpisAsync(CancellationToken ct = default)
        {
            var total = await _db.Jobs.AsNoTracking().CountAsync(ct);

            var delivered = await _db.Jobs.AsNoTracking()
                .CountAsync(x => x.CurrentStatus == JobStatus.Delivered, ct);

            var exception = await _db.Jobs.AsNoTracking()
                .CountAsync(x => x.CurrentStatus == JobStatus.Exception, ct);

            var today = System.DateTime.UtcNow.Date;

            var dueSoon = await _db.Jobs.AsNoTracking()
                .CountAsync(x =>
                    x.SLA_MailBy.Date >= today &&
                    x.SLA_MailBy.Date <= today.AddDays(3) &&
                    x.CurrentStatus != JobStatus.Delivered &&
                    x.CurrentStatus != JobStatus.Exception, ct);

            var overdue = await _db.Jobs.AsNoTracking()
                .CountAsync(x =>
                    x.SLA_MailBy.Date < today &&
                    x.CurrentStatus != JobStatus.Delivered &&
                    x.CurrentStatus != JobStatus.Exception, ct);

            return new KpiSummary(total, delivered, exception, dueSoon, overdue);
        }

        public async Task<List<DailyTransitionPoint>> GetDailyTransitionsAsync(int days = 14, CancellationToken ct = default)
        {
            if (days < 1) days = 14;
            var from = System.DateTime.UtcNow.Date.AddDays(1 - days);

            var query = _db.JobStatusHistories.AsNoTracking()
                .Where(t => t.ChangedAt.Date >= from)
                .GroupBy(t => new { Day = System.DateOnly.FromDateTime(t.ChangedAt.Date), t.Status })
                .Select(g => new DailyTransitionPoint(g.Key.Day, g.Key.Status.ToString(), g.Count()))
                .OrderBy(x => x.Date);

            return await query.ToListAsync(ct);
        }

        private void AddHistory(int jobId, JobStatus status, string note)
        {
            _db.JobStatusHistories.Add(new JobStatusHistory
            {
                JobId = jobId,
                Status = status,
                Note = note,
                ChangedAt = System.DateTime.UtcNow
            });
        }

        private JobStatus GetNextStatus(JobStatus current) => current switch
        {
            JobStatus.Received => JobStatus.Printing,
            JobStatus.Printing => JobStatus.Inserting,
            JobStatus.Inserting => JobStatus.Mailed,
            JobStatus.Mailed => JobStatus.Delivered,
            _ => current
        };
    }
}
