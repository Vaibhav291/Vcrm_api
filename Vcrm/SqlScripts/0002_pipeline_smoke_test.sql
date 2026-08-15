IF OBJECT_ID(N'dbo.PipelineSmokeTest', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PipelineSmokeTest (
        Id           INT IDENTITY(1,1) NOT NULL,
        CreatedAtUtc DATETIME2         NOT NULL,
        CONSTRAINT PK_PipelineSmokeTest PRIMARY KEY (Id)
    );
END
