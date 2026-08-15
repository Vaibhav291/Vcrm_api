IF COL_LENGTH('dbo.Users', 'IsEmailVerified') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD IsEmailVerified BIT NOT NULL CONSTRAINT DF_Users_IsEmailVerified DEFAULT (0);
END;

IF COL_LENGTH('dbo.Users', 'OtpCode') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD OtpCode NVARCHAR(10) NULL;
END;

IF COL_LENGTH('dbo.Users', 'OtpExpiresAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD OtpExpiresAt DATETIME2 NULL;
END;

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.InsertUser
    @Username NVARCHAR(50),
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(500),
    @Role NVARCHAR(20),
    @CreatedAt DATETIME2,
    @IsEmailVerified BIT,
    @OtpCode NVARCHAR(10),
    @OtpExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users
    (
        Username,
        Email,
        PasswordHash,
        Role,
        CreatedAt,
        IsEmailVerified,
        OtpCode,
        OtpExpiresAt
    )
    OUTPUT
        inserted.UserId,
        inserted.Username,
        inserted.Email,
        inserted.PasswordHash,
        inserted.Role,
        inserted.CreatedAt,
        inserted.IsEmailVerified,
        inserted.OtpCode,
        inserted.OtpExpiresAt
    VALUES
    (
        @Username,
        @Email,
        @PasswordHash,
        @Role,
        @CreatedAt,
        @IsEmailVerified,
        @OtpCode,
        @OtpExpiresAt
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
        CreatedAt,
        IsEmailVerified,
        OtpCode,
        OtpExpiresAt
    FROM dbo.Users
    WHERE Username = @Username;
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.GetUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UserId,
        Username,
        Email,
        PasswordHash,
        Role,
        CreatedAt,
        IsEmailVerified,
        OtpCode,
        OtpExpiresAt
    FROM dbo.Users
    WHERE Email = @Email;
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.SetUserOtp
    @UserId INT,
    @OtpCode NVARCHAR(10),
    @OtpExpiresAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET
        OtpCode = @OtpCode,
        OtpExpiresAt = @OtpExpiresAt
    WHERE UserId = @UserId;

    SELECT
        UserId,
        Username,
        Email,
        PasswordHash,
        Role,
        CreatedAt,
        IsEmailVerified,
        OtpCode,
        OtpExpiresAt
    FROM dbo.Users
    WHERE UserId = @UserId;
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.VerifyUserEmail
    @UserId INT,
    @OtpCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET
        IsEmailVerified = 1,
        OtpCode = NULL,
        OtpExpiresAt = NULL
    WHERE UserId = @UserId
        AND OtpCode = @OtpCode
        AND OtpExpiresAt IS NOT NULL
        AND OtpExpiresAt > SYSUTCDATETIME();

    DECLARE @RowsAffected INT = @@ROWCOUNT;

    SELECT
        UserId,
        Username,
        Email,
        PasswordHash,
        Role,
        CreatedAt,
        IsEmailVerified,
        OtpCode,
        OtpExpiresAt
    FROM dbo.Users
    WHERE UserId = @UserId
        AND @RowsAffected > 0;
END
');

EXEC(N'
CREATE OR ALTER PROCEDURE dbo.DeleteUnverifiedUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Users
    WHERE UserId = @UserId
        AND IsEmailVerified = 0;
END
');
