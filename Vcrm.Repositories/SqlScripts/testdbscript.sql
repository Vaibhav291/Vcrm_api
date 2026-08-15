CREATE OR ALTER FUNCTION dbo.GetAllCustomers()
RETURNS TABLE
AS
RETURN
(
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
);