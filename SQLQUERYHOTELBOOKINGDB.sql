USE HotelBookingDB


SELECT * FROM Rooms
SELECT * FROM Guests
SELECT * FROM Invoices
SELECT * FROM Payments
SELECT * FROM Bookings

SELECT 
    i.InvoiceId,
    i.TotalAmount,
    i.IsPaid,
    i.PaymentDeadline,
    p.PaymentId,
    p.PaymentDate,
    p.AmountPaid
FROM Invoices i
LEFT JOIN Payments p ON i.InvoiceId = p.InvoiceId
ORDER BY i.InvoiceId;


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
    b.BookingStatus
FROM Guests g
LEFT JOIN Bookings b ON g.GuestId = b.GuestId
ORDER BY g.GuestId;


SELECT * FROM Guests
SELECT * FROM Invoices
SELECT * FROM Payments
SELECT * FROM Bookings

UPDATE Rooms SET IsAvailable =1; -- All room available
DELETE FROM Bookings; --Remove all current bookings
DBCC CHECKIDENT ('Bookings', RESEED, 0); -- resetest the ID seeding 

-- G

-- SELECT
SELECT 
    B.BookingId, 
    G.FirstName + ' ' + G.LastName AS GuestName, 
    R.RoomId, 
    R.Type AS RoomType, 
    B.CheckInDate, 
    B.CheckOutDate
FROM 
    Bookings AS B
INNER JOIN 
    Guests AS G ON B.GuestId = G.GuestId
INNER JOIN 
    Rooms AS R ON B.RoomId = R.RoomId
WHERE 
    G.LastName = 'Lastname2' 
    AND B.IsCheckedOut = 0;

-- SORT

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

--where
SELECT 
    B.BookingId, 
    G.FirstName + ' ' + G.LastName AS GuestName, 
    R.RoomId, 
    B.CheckInDate, 
    B.CheckOutDate
FROM 
    Bookings AS B
INNER JOIN 
    Guests AS G ON B.GuestId = G.GuestId
INNER JOIN 
    Rooms AS R ON B.RoomId = R.RoomId
WHERE 
    B.CheckInDate BETWEEN '2024-01-01' AND '2024-01-31'
ORDER BY 
    B.CheckInDate ASC;


--WHERE
SELECT 
    GuestId, 
    FirstName, 
    LastName, 
    Email, 
    PhoneNumber
FROM 
    Guests
WHERE 
    LastName LIKE 'J%'
ORDER BY 
    LastName ASC;


-- SELECT & WHERE
SELECT
    RoomId, 
    Type AS RoomType, 
    PricePerNight
FROM 
    Rooms
WHERE 
    Type IN ('Single', 'Double')
ORDER BY 
    PricePerNight DESC;


-- SELECT & WHERE

SELECT 
    BookingId, 
    RoomId, 
    GuestId, 
    CheckInDate, 
    CheckOutDate
FROM 
    Bookings
WHERE 
    CheckOutDate IS NULL
ORDER BY 
    CheckInDate DESC;




-- JOINS
-- information om gäster, deras bokningar och vilka rum de har bokat.

SELECT 
    G.GuestId, 
    G.FirstName + ' ' + G.LastName AS GuestName,
    B.BookingId,
    R.RoomId,
    R.Type AS RoomType,
    R.PricePerNight,
    B.CheckInDate,
    B.CheckOutDate
FROM 
    Guests AS G
INNER JOIN 
    Bookings AS B ON G.GuestId = B.GuestId
INNER JOIN 
    Rooms AS R ON B.RoomId = R.RoomId
WHERE 
    B.IsCheckedOut = 0 -- Endast aktiva bokningar
ORDER BY 
    G.GuestId;


-- GROUP BY 
-- totala antalet bokningar och det totala intäktsvärdet per rumstyp.
SELECT 
    R.Type AS RoomType,
    COUNT(B.BookingId) AS TotalBookings,
    SUM(R.PricePerNight) AS TotalRevenue
FROM 
    Bookings AS B
INNER JOIN 
    Rooms AS R ON B.RoomId = R.RoomId
GROUP BY 
    R.Type
ORDER BY 
    TotalRevenue DESC;


-- Unpaid bookings
-- hämta alla obetalda bokningar med fakturor som är äldre än 10 dagar
SELECT 
    B.BookingId,
    G.FirstName + ' ' + G.LastName AS GuestName,
    R.RoomId,
    I.TotalAmount,
    I.PaymentDeadline
FROM 
    Bookings AS B
INNER JOIN 
    Guests AS G ON B.GuestId = G.GuestId
INNER JOIN 
    Rooms AS R ON B.RoomId = R.RoomId
INNER JOIN 
    Invoices AS I ON B.BookingId = I.BookingId
WHERE 
    I.IsPaid = 0 
    AND I.PaymentDeadline < DATEADD(DAY, -10, GETDATE()) 
    AND B.BookingId IN (
        SELECT BookingId 
        FROM Invoices 
        WHERE IsPaid = 0
    )
ORDER BY 
    I.PaymentDeadline ASC;
