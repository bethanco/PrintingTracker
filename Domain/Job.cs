
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrintingJobTracker.Domain;

public enum JobStatus
{
    Received,
    Printing,
    Inserting,
    Mailed,
    Delivered,
    Exception
}

public class Job
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string ClientName { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string JobName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required, StringLength(20)]
    public string Carrier { get; set; } = "USPS";

    public JobStatus CurrentStatus { get; set; } = JobStatus.Received;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime SLA_MailBy { get; set; } = DateTime.UtcNow.AddDays(5);

    public List<JobStatusHistory> History { get; set; } = new();
}

public class JobStatusHistory
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job? Job { get; set; }
    public JobStatus Status { get; set; }
    [StringLength(500)]
    public string? Note { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
