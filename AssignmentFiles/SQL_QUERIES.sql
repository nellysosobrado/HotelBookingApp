USE HotelBookingDB;
GO

-- Select ========================================================
SELECT * FROM Rooms
SELECT * FROM Guests
SELECT * FROm Bookings
SELECT * FROM Invoices;
SELECT * FROM Payments;
SELECT * FROM CanceledBookingsHistory

-- Where ========================================================

SELECT * 
FROM Invoices
WHERE isPaid = 1

SELECT * 
FROM Invoices
WHERE isPaid = 1


--Order By ========================================================

SELECT 
    i.InvoiceId,
    i.TotalAmount,
    i.IsPaid,
    i.PaymentDeadline,
    p.PaymentId,
    p.PaymentDate,
    p.AmountPaid
FROM 
    Invoices i
LEFT JOIN 
    Payments p ON i.InvoiceId = p.InvoiceId
ORDER BY 
    i.InvoiceId;

-- Joins ========================================================

SELECT 
    RoomId, 
    Type AS RoomType, 
    PricePerNight, 
    SizeInSquareMeters, 
    ExtraBeds
FROM 
    Rooms
WHERE 
    IsAvailable = 1
ORDER BY 
    PricePerNight ASC;

	
SELECT 
    g.GuestId,
    g.FirstName,
    g.LastName,
    g.Email,
    g.PhoneNumber,
    b.BookingId,
    b.RoomId,
    b.CheckInDate,
    b.CheckOutDate,
    b.IsCheckedIn,
    b.IsCheckedOut,
    b.BookingCompleted
FROM 
    Guests g
LEFT JOIN 
    Bookings b ON g.GuestId = b.GuestId
ORDER BY 
    g.GuestId;


--Group By========================================================

SELECT g.GuestId, COUNT(b.BookingId) AS BookingCount
FROM Guests g
LEFT JOIN Bookings b ON g.GuestId = b.GuestId
GROUP BY g.GuestId;


-- SubQuery ========================================================

SELECT *
FROM Guests g
WHERE g.GuestId IN (
    SELECT b.GuestId
    FROM Bookings b
);


SELECT i.InvoiceId, i.TotalAmount, i.IsPaid, i.PaymentDeadline
FROM Invoices i
WHERE i.InvoiceId NOT IN (
    SELECT DISTINCT p.InvoiceId
    FROM Payments p
);


