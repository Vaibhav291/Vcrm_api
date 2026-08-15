IF OBJECT_ID('dbo.test_table', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.test_table (
        id INT IDENTITY(1,1) PRIMARY KEY,
        name VARCHAR(100) NOT NULL,
        created_at DATETIME2 DEFAULT SYSUTCDATETIME()
    );
END;