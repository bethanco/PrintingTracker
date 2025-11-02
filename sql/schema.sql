
-- PrintingJobTracker schema
IF DB_ID('PrintingJobTrackerDb') IS NULL
BEGIN
    CREATE DATABASE PrintingJobTrackerDb;
END
GO
USE PrintingJobTrackerDb;
GO

IF OBJECT_ID('dbo.JobStatusHistories', 'U') IS NOT NULL DROP TABLE dbo.JobStatusHistories;
IF OBJECT_ID('dbo.Jobs', 'U') IS NOT NULL DROP TABLE dbo.Jobs;
GO

CREATE TABLE dbo.Jobs
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClientName NVARCHAR(200) NOT NULL,
    JobName NVARCHAR(200) NOT NULL,
    Quantity INT NOT NULL,
    Carrier NVARCHAR(50) NOT NULL,
    CurrentStatus NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    SLA_MailBy DATETIME2 NOT NULL
);
GO

CREATE TABLE dbo.JobStatusHistories
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JobId INT NOT NULL FOREIGN KEY REFERENCES dbo.Jobs(Id),
    Status NVARCHAR(50) NOT NULL,
    Note NVARCHAR(1000) NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO
