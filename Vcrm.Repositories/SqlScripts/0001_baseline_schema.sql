IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers (
        CustomerId       INT IDENTITY(1,1) NOT NULL,
        FirstName        NVARCHAR(50)  NOT NULL,
        LastName         NVARCHAR(50)  NOT NULL,
        CompanyName      NVARCHAR(100) NULL,
        Email            NVARCHAR(150) NOT NULL,
        Phone            NVARCHAR(20)  NULL,
        Industry         NVARCHAR(100) NULL,
        Address          NVARCHAR(250) NULL,
        AssignedToUserId INT           NULL,
        CreatedAt        DATETIME2     NOT NULL,
        UpdatedAt        DATETIME2     NOT NULL,
        IsActive         BIT           NOT NULL CONSTRAINT DF_Customers_IsActive DEFAULT (1),
        CONSTRAINT PK_Customers PRIMARY KEY (CustomerId)
    );
END
