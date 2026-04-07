CREATE TABLE [dbo].[Booking]
(
	BookingId BIGINT IDENTITY NOT NULL,

    CustomerName NVARCHAR(150) NOT NULL,
    ContactNumber NVARCHAR(50) NOT NULL,
    Email NVARCHAR(150) NULL,

    VehicleType NVARCHAR(100) NOT NULL,
    ServiceType NVARCHAR(150) NOT NULL,

    BookingDate DATE NOT NULL,

    Notes NVARCHAR(MAX) NULL,

    GoogleEventId NVARCHAR(255) NULL,

    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',

    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,

    CONSTRAINT [Booking_BookingId_PK] PRIMARY KEY (BookingId)
)
