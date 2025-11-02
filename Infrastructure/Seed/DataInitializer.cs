using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Data;
using PrintingJobTracker.Models;

namespace PrintingJobTracker.Infrastructure.Seed
{
    public class DataInitializer
    {
        private readonly OpsDbContext _db;
        public DataInitializer(OpsDbContext db) => _db = db;

        public async Task SeedAsync()
        {
            await _db.Database.MigrateAsync();

            if (!await _db.Jobs.AnyAsync())
            {
                var now = DateTime.UtcNow;
                var jobs = new List<Job>
                {
                    new Job{ ClientName="ACME", JobName="Mailing A", Quantity=1000, Carrier="USPS", CurrentStatus=JobStatus.Received, CreatedAt=now, SLA_MailBy=now.AddDays(7)},
                    new Job{ ClientName="Globex", JobName="Mailing B", Quantity=500, Carrier="UPS", CurrentStatus=JobStatus.Printing, CreatedAt=now, SLA_MailBy=now.AddDays(10)},
                };
                _db.Jobs.AddRange(jobs);
                await _db.SaveChangesAsync();

                foreach (var j in jobs)
                    _db.JobStatusHistories.Add(new JobStatusHistory{ JobId=j.Id, Status=j.CurrentStatus, Note="Seed", ChangedAt=now });

                await _db.SaveChangesAsync();
            }
        }
    }
}
