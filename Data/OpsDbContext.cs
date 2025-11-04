
using System;
using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Domain;

namespace PrintingJobTracker.Data;

public class OpsDbContext : DbContext
{
    public OpsDbContext(DbContextOptions<OpsDbContext> options) : base(options) { }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobStatusHistory> JobStatusHistory => Set<JobStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Job>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ClientName).HasMaxLength(100).IsRequired();
            e.Property(x => x.JobName).HasMaxLength(120).IsRequired();
            e.Property(x => x.Carrier).HasMaxLength(20).IsRequired();
            e.Property(x => x.CurrentStatus).HasConversion<string>().IsRequired();
            e.HasMany(x => x.History)
                .WithOne(h => h.Job!)
                .HasForeignKey(h => h.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobStatusHistory>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<string>().IsRequired();
            e.Property(x => x.Note).HasMaxLength(500);
        });
    }
}

public static class DataSeeder
{
    private static readonly string[] Carriers = new[] { "USPS", "UPS", "FedEx" };
    private static readonly Random Rng = new Random();

    public static void Seed(OpsDbContext db)
    {
        if (db.Jobs.Any()) return;

        var jobs = new List<Job>();
        for (int i = 1; i <= 12; i++)
        {
            var status = (JobStatus)(i % 5); // spread statuses
            var job = new Job
            {
                ClientName = $"Client {i}",
                JobName = $"Mailing Campaign {i}",
                Quantity = 1000 + i * 50,
                Carrier = Carriers[i % Carriers.Length],
                CurrentStatus = status,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                SLA_MailBy = DateTime.UtcNow.AddDays(7 - (i % 3))
            };
            jobs.Add(job);
        }

        db.Jobs.AddRange(jobs);
        db.SaveChanges();

        // Seed initial history
        foreach (var job in db.Jobs)
        {
            var path = GetPath(job.CurrentStatus);
            var date = job.CreatedAt;
            foreach (var step in path)
            {
                date = date.AddHours(6);
                db.JobStatusHistory.Add(new JobStatusHistory
                {
                    JobId = job.Id,
                    Status = step,
                    Note = step == JobStatus.Received ? "Job created" : null,
                    ChangedAt = date
                });
            }
        }
        db.SaveChanges();
    }

    private static IEnumerable<JobStatus> GetPath(JobStatus current)
    {
        var ordered = new[] { JobStatus.Received, JobStatus.Printing, JobStatus.Inserting, JobStatus.Mailed, JobStatus.Delivered };
        foreach (var s in ordered)
        {
            yield return s;
            if (s == current) yield break;
        }
    }
}
