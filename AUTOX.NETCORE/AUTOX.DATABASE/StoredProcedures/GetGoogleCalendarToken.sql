CREATE PROCEDURE [dbo].[GetGoogleCalendarToken]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM [dbo].[GoogleCalendarToken]
    FOR JSON PATH;
END