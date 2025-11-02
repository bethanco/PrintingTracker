using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Models;
using PrintingJobTracker.Data;

namespace PrintingJobTracker.Services
{
    public class DataSeeder
    {
        private readonly OpsDbContext _db;

        public DataSeeder(OpsDbContext db) => _db = db;

        public async Task SeedAsync()
        {
            if (await _db.Jobs.AnyAsync()) return;

            var carriers = new[] { "USPS", "UPS", "FedEx" };
            var clients = new[] { "Citibank", "BAC", "Citi CR", "Bimbo", "Dynacast", "PMONC", "FORCO", "ALFA", "Grupo Zeas", "INTACO" };
            var rnd = new Random(42);

            for (int i = 1; i <= 10; i++)
            {
                var job = new Job
                {
                    ClientName = clients[(i - 1) % clients.Length],
                    JobName = $"Mailing {DateTime.UtcNow:yyyy-MM}-{i}",
                    Quantity = rnd.Next(500, 5000),
                    Carrier = carriers[rnd.Next(carriers.Length)],
                    CurrentStatus = (JobStatus)rnd.Next(0, 5),
                    CreatedAt = DateTime.UtcNow.AddDays(-rnd.Next(1, 15)),
                    SLA_MailBy = DateTime.UtcNow.AddDays(rnd.Next(1, 10))
                };
                _db.Jobs.Add(job);
                await _db.SaveChangesAsync();

                // Seed minimal history starting from Received up to CurrentStatus
                var path = new List<JobStatus> { JobStatus.Received, JobStatus.Printing, JobStatus.Inserting, JobStatus.Mailed, JobStatus.Delivered };
                foreach (var st in path)
                {
                    AddHistory(job.Id, st, $"Auto-seed: {st}");
                    if (st == job.CurrentStatus) break;
                }
                await _db.SaveChangesAsync();
            }
        }

        private void AddHistory(int jobId, JobStatus status, string note)
        {
            _db.JobStatusHistories.Add(new JobStatusHistory
            {
                JobId = jobId,
                Status = status,
                Note = note,
                ChangedAt = DateTime.UtcNow
            });
        }
    }
}
