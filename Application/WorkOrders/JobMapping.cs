using PrintingJobTracker.Models;
namespace PrintingJobTracker.Application.WorkOrders;

public static class JobMapping
{
    public static JobDto ToDto(this Job e) => new(
        e.Id,
        e.ClientName,
        e.JobName,
        e.Quantity,
        e.Carrier,
        e.CurrentStatus.ToString(),
        e.CreatedAt,
        e.SLA_MailBy
    );
}
