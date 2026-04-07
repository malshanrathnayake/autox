CREATE PROCEDURE [dbo].[CompleteBooking]	
	@bookingId BIGINT,
	@executionStatus BIT OUTPUT
AS
BEGIN

	SET NOCOUNT ON

	BEGIN TRY

		UPDATE Booking SET Status = 'Completed' WHERE BookingId = @bookingId

		SET @executionStatus = 1

	END TRY
	BEGIN CATCH

		SET @executionStatus = 0

	END CATCH

END