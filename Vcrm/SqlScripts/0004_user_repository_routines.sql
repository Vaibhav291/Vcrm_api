IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        UserId       INT IDENTITY(1,1) NOT NULL,
        Username     NVARCHAR(50)  NOT NULL,
        Email        NVARCHAR(150) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Role         NVARCHAR(20)  NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('User'),
        CreatedAt    DATETIME2     NOT NULL,
        CONSTRAINT PK_Users PRIMARY KEY (UserId),
        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END;

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.InsertUser
    @Username NVARCHAR(50),
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(500),
    @Role NVARCHAR(20),
    @CreatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users
    (
        Username,
        Email,
        PasswordHash,
        Role,
        CreatedAt
    )
    OUTPUT
        inserted.UserId,
        inserted.Username,
        inserted.Email,
        inserted.PasswordHash,
        inserted.Role,
        inserted.CreatedAt
    VALUES
    (
        @Username,
        @Email,
        @PasswordHash,
        @Role,
        @CreatedAt
    );
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.GetUserByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UserId,
        Username,
        Email,
        PasswordHash,
        Role,
        CreatedAt
    FROM dbo.Users
    WHERE Username = @Username;
END
');
