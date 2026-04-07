CREATE PROCEDURE [dbo].[UpdateGoogleCalendarToken]
@jsonString NVARCHAR(MAX),
    @executionStatus BIT OUT,
    @primaryKey BIGINT OUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE
            @GoogleCalendarTokenId INT,
            @GoogleEmail NVARCHAR(150),
            @AccessToken NVARCHAR(MAX),
            @RefreshToken NVARCHAR(MAX),
            @TokenExpiryUtc DATETIME;

        SELECT
            @GoogleCalendarTokenId = ISNULL(GoogleCalendarTokenId, 0),
            @GoogleEmail = GoogleEmail,
            @AccessToken = AccessToken,
            @RefreshToken = RefreshToken,
            @TokenExpiryUtc = TokenExpiryUtc
        FROM OPENJSON(@jsonString)
        WITH
        (
            GoogleCalendarTokenId INT,
            GoogleEmail NVARCHAR(150),
            AccessToken NVARCHAR(MAX),
            RefreshToken NVARCHAR(MAX),
            TokenExpiryUtc DATETIMEOFFSET
        );

        IF (@GoogleCalendarTokenId = 0)
        BEGIN
            INSERT INTO [dbo].[GoogleCalendarToken]
            (
                GoogleEmail,
                AccessToken,
                RefreshToken,
                TokenExpiryUtc,
                CreatedAt
            )
            VALUES
            (
                @GoogleEmail,
                @AccessToken,
                @RefreshToken,
                @TokenExpiryUtc,
                GETUTCDATE()
            );

            SET @primaryKey = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE [dbo].[GoogleCalendarToken]
            SET
                GoogleEmail = @GoogleEmail,
                AccessToken = @AccessToken,
                RefreshToken = @RefreshToken,
                TokenExpiryUtc = @TokenExpiryUtc,
                UpdatedAt = GETUTCDATE()
            WHERE GoogleCalendarTokenId = @GoogleCalendarTokenId;

            SET @primaryKey = @GoogleCalendarTokenId;
        END

        COMMIT TRANSACTION;
        SET @executionStatus = 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @executionStatus = 0;
        SET @primaryKey = 0;
        THROW;
    END CATCH
END