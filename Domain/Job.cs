using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrintingJobTracker.Models
{
    [Table("Job")]
public class Job
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string ClientName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string JobName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, MaxLength(50)]
        public string Carrier { get; set; } = "USPS";

        [Required]
        public JobStatus CurrentStatus { get; set; } = JobStatus.Received;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime SLA_MailBy { get; set; }

        public List<JobStatusHistory> StatusHistory { get; set; } = new();
    }
}