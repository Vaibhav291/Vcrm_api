EXEC(N'
CREATE OR ALTER PROCEDURE dbo.InsertCustomer
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @CompanyName NVARCHAR(100) = NULL,
    @Email NVARCHAR(150),
    @Phone NVARCHAR(20) = NULL,
    @Industry NVARCHAR(100) = NULL,
    @Address NVARCHAR(250) = NULL,
    @AssignedToUserId INT = NULL,
    @CreatedAt DATETIME2 = NULL,
    @UpdatedAt DATETIME2 = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Customers
    (
        FirstName,
        LastName,
        CompanyName,
        Email,
        Phone,
        Industry,
        Address,
        AssignedToUserId,
        CreatedAt,
        UpdatedAt,
        IsActive
    )
    OUTPUT
        inserted.CustomerId,
        inserted.FirstName,
        inserted.LastName,
        inserted.CompanyName,
        inserted.Email,
        inserted.Phone,
        inserted.Industry,
        inserted.Address,
        inserted.AssignedToUserId,
        inserted.CreatedAt,
        inserted.UpdatedAt,
        inserted.IsActive
    VALUES
    (
        @FirstName,
        @LastName,
        @CompanyName,
        @Email,
        @Phone,
        @Industry,
        @Address,
        @AssignedToUserId,
        ISNULL(@CreatedAt, SYSUTCDATETIME()),
        ISNULL(@UpdatedAt, SYSUTCDATETIME()),
        @IsActive
    );
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.UpdateCustomer
    @CustomerId INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @CompanyName NVARCHAR(100) = NULL,
    @Email NVARCHAR(150),
    @Phone NVARCHAR(20) = NULL,
    @Industry NVARCHAR(100) = NULL,
    @Address NVARCHAR(250) = NULL,
    @AssignedToUserId INT = NULL,
    @UpdatedAt DATETIME2 = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customers
    SET
        FirstName = @FirstName,
        LastName = @LastName,
        CompanyName = @CompanyName,
        Email = @Email,
        Phone = @Phone,
        Industry = @Industry,
        Address = @Address,
        AssignedToUserId = @AssignedToUserId,
        UpdatedAt = ISNULL(@UpdatedAt, SYSUTCDATETIME()),
        IsActive = @IsActive
    WHERE CustomerId = @CustomerId;

    SELECT
        CustomerId,
        FirstName,
        LastName,
        CompanyName,
        Email,
        Phone,
        Industry,
        Address,
        AssignedToUserId,
        CreatedAt,
        UpdatedAt,
        IsActive
    FROM dbo.Customers
    WHERE CustomerId = @CustomerId;
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.DeleteCustomer
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Customers
    OUTPUT
        deleted.CustomerId,
        deleted.FirstName,
        deleted.LastName,
        deleted.CompanyName,
        deleted.Email,
        deleted.Phone,
        deleted.Industry,
        deleted.Address,
        deleted.AssignedToUserId,
        deleted.CreatedAt,
        deleted.UpdatedAt,
        deleted.IsActive
    WHERE CustomerId = @CustomerId;
END
');

-- dbo.GetAllCustomers previously existed as an inline table-valued function (see testdbscript.sql);
-- drop it so it can be recreated below as a procedure.
IF OBJECT_ID(N'dbo.GetAllCustomers', N'IF') IS NOT NULL
BEGIN
    DROP FUNCTION dbo.GetAllCustomers;
END;

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.GetAllCustomers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CustomerId,
        FirstName,
        LastName,
        CompanyName,
        Email,
        Phone,
        Industry,
        Address,
        AssignedToUserId,
        CreatedAt,
        UpdatedAt,
        IsActive
    FROM dbo.Customers
    WHERE IsActive = 1
    ORDER BY CustomerId DESC;
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.GetCustomerById
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CustomerId,
        FirstName,
        LastName,
        CompanyName,
        Email,
        Phone,
        Industry,
        Address,
        AssignedToUserId,
        CreatedAt,
        UpdatedAt,
        IsActive
    FROM dbo.Customers
    WHERE CustomerId = @CustomerId;
END
');


