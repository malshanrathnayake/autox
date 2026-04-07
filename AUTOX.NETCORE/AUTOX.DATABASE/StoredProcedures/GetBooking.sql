CREATE PROCEDURE [dbo].[GetBooking]
    @BookingId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF(@BookingId != 0)
    BEGIN
        SELECT * FROM [Booking] WHERE BookingId = @BookingId FOR JSON PATH;
    END
    ELSE
    BEGIN
        SELECT * FROM [Booking] FOR JSON PATH;
    END


END