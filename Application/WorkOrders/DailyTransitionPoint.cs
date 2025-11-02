namespace PrintingJobTracker.Application.WorkOrders;

public sealed record DailyTransitionPoint(
    System.DateOnly Date,
    string Status,
    int Count
);
