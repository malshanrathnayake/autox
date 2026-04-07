CREATE TABLE [dbo].[GoogleCalendarToken]
(
	GoogleCalendarTokenId INT IDENTITY NOT NULL,
    GoogleEmail NVARCHAR(150) NOT NULL,
    AccessToken NVARCHAR(MAX) NULL,
    RefreshToken NVARCHAR(MAX) NOT NULL,
    TokenExpiryUtc DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT [GoogleCalendarToken_GoogleCalendarTokenId] PRIMARY KEY (GoogleCalendarTokenId)
)
