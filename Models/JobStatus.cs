namespace PrintingJobTracker.Models
{
    public enum JobStatus
    {
        Received = 0,
        Printing = 1,
        Inserting = 2,
        Mailed = 3,
        Delivered = 4,
        Exception = 5
    }
}
