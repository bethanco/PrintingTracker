namespace PrintingJobTracker.Application.WorkOrders;

public sealed record KpiSummary(
    int Total,
    int Delivered,
    int Exception,
    int DueSoon,
    int Overdue
);
