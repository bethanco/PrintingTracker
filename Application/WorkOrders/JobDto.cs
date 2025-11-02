namespace PrintingJobTracker.Application.WorkOrders;

public sealed record JobDto(
    int Id,
    string ClientName,
    string JobName,
    int Quantity,
    string Carrier,
    string CurrentStatus,
    System.DateTime CreatedAt,
    System.DateTime? SLA_MailBy);
