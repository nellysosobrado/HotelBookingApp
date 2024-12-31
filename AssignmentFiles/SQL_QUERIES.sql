USE HotelBookingDB;
GO


--2: (G) Skapa MS-SQL syntaxer som kan utföra ’Select’, ’Where’, ’Order By’ uppgifter mot din databas.


-- Select
SELECT * FROM Rooms
SELECT * FROM Guests
SELECT * FROm Bookings
SELECT * FROM Invoices;
SELECT * FROM Payments;
SELECT * FROM CanceledBookingsHistory

-- Where

-- Visar alla betalda invoices
SELECT * 
FROM Invoices
WHERE isPaid = 1

-- -- Visar alla obetalda invoices
SELECT * 
FROM Invoices
WHERE isPaid = 1


--Order By
-- sorterar utifrån invoiceid's
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


--4: (VG) Skapa MS-SQL syntaxer som kan utföra ’Joins’, ’Group By’ och minst en ’Subquery’ mot din databas

-- Joins

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


--Group By

-- räknar antal bokningar per gäst har
SELECT g.GuestId, COUNT(b.BookingId) AS BookingCount
FROM Guests g
LEFT JOIN Bookings b ON g.GuestId = b.GuestId
GROUP BY g.GuestId;



-- SubQuery
-- hämtar guest information i guest tabell, men samtidigt från bookings
SELECT *
FROM Guests g
WHERE g.GuestId IN (
    SELECT b.GuestId
    FROM Bookings b
);

-- hämtar obetalda fakturar, bara om det inte finns hos payments tabellen
SELECT i.InvoiceId, i.TotalAmount, i.IsPaid, i.PaymentDeadline
FROM Invoices i
WHERE i.InvoiceId NOT IN (
    SELECT DISTINCT p.InvoiceId
    FROM Payments p
);


