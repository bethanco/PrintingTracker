using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrintingJobTracker.Models
{
    [Table("JobStatusHistory")]
public class JobStatusHistory
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        [Required]
        public JobStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}