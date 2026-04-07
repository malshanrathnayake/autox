CREATE PROCEDURE [dbo].[UpdateBooking]
    @jsonString NVARCHAR(MAX),
    @executionStatus BIT OUT,
    @primaryKey BIGINT OUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE
            @BookingId BIGINT,
            @CustomerName NVARCHAR(150),
            @ContactNumber NVARCHAR(50),
            @Email NVARCHAR(150),
            @VehicleType NVARCHAR(100),
            @ServiceType NVARCHAR(150),
            @BookingDate DATE,
            @Notes NVARCHAR(MAX),
            @GoogleEventId NVARCHAR(255),
            @Status NVARCHAR(50);

        SELECT
            @BookingId = ISNULL(BookingId, 0),
            @CustomerName = CustomerName,
            @ContactNumber = ContactNumber,
            @Email = Email,
            @VehicleType = VehicleType,
            @ServiceType = ServiceType,
            @BookingDate = BookingDate,
            @Notes = Notes,
            @GoogleEventId = GoogleEventId,
            @Status = ISNULL(Status, 'Pending')
        FROM OPENJSON(@jsonString)
        WITH
        (
            BookingId BIGINT,
            CustomerName NVARCHAR(150),
            ContactNumber NVARCHAR(50),
            Email NVARCHAR(150),
            VehicleType NVARCHAR(100),
            ServiceType NVARCHAR(150),
            BookingDate DATE,
            Notes NVARCHAR(MAX),
            GoogleEventId NVARCHAR(255),
            Status NVARCHAR(50)
        );

        IF (@BookingId = 0)
        BEGIN
            INSERT INTO [dbo].[Booking]
            (
                CustomerName,
                ContactNumber,
                Email,
                VehicleType,
                ServiceType,
                BookingDate,
                Notes,
                GoogleEventId,
                Status,
                CreatedAt
            )
            VALUES
            (
                @CustomerName,
                @ContactNumber,
                @Email,
                @VehicleType,
                @ServiceType,
                @BookingDate,
                @Notes,
                @GoogleEventId,
                @Status,
                GETUTCDATE()
            );

            SET @primaryKey = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            UPDATE [dbo].[Booking]
            SET
                CustomerName = @CustomerName,
                ContactNumber = @ContactNumber,
                Email = @Email,
                VehicleType = @VehicleType,
                ServiceType = @ServiceType,
                BookingDate = @BookingDate,
                Notes = @Notes,
                GoogleEventId = @GoogleEventId,
                Status = @Status,
                UpdatedAt = GETUTCDATE()
            WHERE BookingId = @BookingId;

            SET @primaryKey = @BookingId;
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