using Microsoft.EntityFrameworkCore;
using PrintingJobTracker.Infrastructure;
using PrintingJobTracker.Application.WorkOrders;
using PrintingJobTracker.Domain;
using Xunit;

public static class TFactory
{
    public static (OpsDbContext db, WorkOrdersService svc) Create()
    {
        var opt = new DbContextOptionsBuilder<OpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new OpsDbContext(opt);
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<WorkOrdersService>.Instance;
        var svc = new WorkOrdersService(db, logger);
        return (db, svc);
    }
}

public class WorkOrdersFlowTests
{
    [Fact]
    public async Task Advance_Flows_To_Delivered()
    {
        var (db, svc) = TFactory.Create();
        var id = await svc.CreateAsync(new Job {
            ClientName = "ACME",
            JobName = "Test",
            Quantity = 100,
            Carrier = "USPS"
        });

        await svc.AdvanceAsync(id); // Received -> Printing
        await svc.AdvanceAsync(id); // -> Inserting
        await svc.AdvanceAsync(id); // -> Mailed
        await svc.AdvanceAsync(id); // -> Delivered
        await svc.AdvanceAsync(id); // stays Delivered

        var item = await svc.GetByIdAsync(id);
        Assert.Equal("Delivered", item!.CurrentStatus);
    }

    [Fact]
    public async Task Mark_Exception_Sets_Status_And_History()
    {
        var (db, svc) = TFactory.Create();
        var id = await svc.CreateAsync(new Job {
            ClientName = "Globex",
            JobName = "Ops",
            Quantity = 200,
            Carrier = "UPS"
        });

        await svc.MarkExceptionAsync(id, "Paper shortage");
        var item = await svc.GetByIdAsync(id);
        Assert.Equal("Exception", item!.CurrentStatus);
        Assert.Contains(item.JobStatusHistory, t => t.Status == "Exception");
    }
}
