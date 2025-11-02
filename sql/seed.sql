
USE PrintingJobTrackerDb;
GO

-- Seed sample jobs
INSERT INTO dbo.Jobs (ClientName, JobName, Quantity, Carrier, CurrentStatus, CreatedAt, SLA_MailBy)
VALUES
('Citibank','Mailing-01',1200,'USPS','Received', SYSUTCDATETIME(), DATEADD(day,7,SYSUTCDATETIME())),
('BAC','Mailing-02',3000,'UPS','Printing', DATEADD(day,-2,SYSUTCDATETIME()), DATEADD(day,5,SYSUTCDATETIME())),
('Bimbo','Mailing-03',2500,'FedEx','Inserting', DATEADD(day,-3,SYSUTCDATETIME()), DATEADD(day,6,SYSUTCDATETIME())),
('Dynacast','Mailing-04',1800,'USPS','Mailed', DATEADD(day,-4,SYSUTCDATETIME()), DATEADD(day,3,SYSUTCDATETIME())),
('PMONC','Mailing-05',900,'UPS','Delivered', DATEADD(day,-5,SYSUTCDATETIME()), DATEADD(day,2,SYSUTCDATETIME())),
('FORCO','Mailing-06',4000,'FedEx','Printing', DATEADD(day,-6,SYSUTCDATETIME()), DATEADD(day,8,SYSUTCDATETIME())),
('ALFA','Mailing-07',2200,'USPS','Received', DATEADD(day,-7,SYSUTCDATETIME()), DATEADD(day,9,SYSUTCDATETIME())),
('Grupo Zeas','Mailing-08',3300,'UPS','Inserting', DATEADD(day,-1,SYSUTCDATETIME()), DATEADD(day,4,SYSUTCDATETIME())),
('INTACO','Mailing-09',1400,'FedEx','Mailed', DATEADD(day,-8,SYSUTCDATETIME()), DATEADD(day,6,SYSUTCDATETIME())),
('Citi CR','Mailing-10',2750,'USPS','Received', DATEADD(day,-9,SYSUTCDATETIME()), DATEADD(day,10,SYSUTCDATETIME()));
GO

-- Seed minimal history for each job
DECLARE @jobId INT;
DECLARE job_cursor CURSOR FOR SELECT Id, CurrentStatus FROM dbo.Jobs;
OPEN job_cursor;
DECLARE @status NVARCHAR(50);
FETCH NEXT FROM job_cursor INTO @jobId, @status;
WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO dbo.JobStatusHistories(JobId, Status, Note) VALUES(@jobId, 'Received', 'Auto-seed: Received');
    IF @status IN ('Printing','Inserting','Mailed','Delivered') INSERT INTO dbo.JobStatusHistories(JobId, Status, Note) VALUES(@jobId, 'Printing', 'Auto-seed: Printing');
    IF @status IN ('Inserting','Mailed','Delivered') INSERT INTO dbo.JobStatusHistories(JobId, Status, Note) VALUES(@jobId, 'Inserting', 'Auto-seed: Inserting');
    IF @status IN ('Mailed','Delivered') INSERT INTO dbo.JobStatusHistories(JobId, Status, Note) VALUES(@jobId, 'Mailed', 'Auto-seed: Mailed');
    IF @status = 'Delivered' INSERT INTO dbo.JobStatusHistories(JobId, Status, Note) VALUES(@jobId, 'Delivered', 'Auto-seed: Delivered');
    FETCH NEXT FROM job_cursor INTO @jobId, @status;
END
CLOSE job_cursor; DEALLOCATE job_cursor;
GO
