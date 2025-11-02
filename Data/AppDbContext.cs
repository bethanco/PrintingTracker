using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Models;

namespace PrintingJobTracker.Data
{
    public class OpsDbContext : DbContext
    {
        public OpsDbContext(DbContextOptions<OpsDbContext> options) : base(options) { }

        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<JobStatusHistory> JobStatusHistories => Set<JobStatusHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.ClientName).HasMaxLength(200).IsRequired();
                e.Property(x => x.JobName).HasMaxLength(200).IsRequired();
                e.Property(x => x.Carrier).HasMaxLength(50).IsRequired();
                e.Property(x => x.CurrentStatus).HasConversion<string>().HasMaxLength(50).IsRequired();
                e.HasMany(x => x.StatusHistory).WithOne().HasForeignKey(x => x.JobId);
            });

            modelBuilder.Entity<JobStatusHistory>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
                e.Property(x => x.Note).HasMaxLength(1000);
            });
        }
    }
}
