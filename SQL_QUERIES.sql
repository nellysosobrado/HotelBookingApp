USE HotelBookingDB;
GO

-- Invoices/Payments
SELECT * FROM Rooms
SELECT * FROM Guests
SELECT * FROm Bookings
SELECT * FROM Invoices;
SELECT * FROM Payments;

SELECT 
    g.GuestId,
    g.FirstName + ' ' + g.LastName AS GuestName,
    b.BookingId,
    r.RoomId,
    r.Type AS RoomType,
    i.TotalAmount,
    CASE 
        WHEN i.IsPaid = 1 THEN 'Paid'
        ELSE 'Unpaid'
    END AS PaymentStatus
FROM Guests g
JOIN Bookings b ON g.GuestId = b.GuestId
JOIN Rooms r ON b.RoomId = r.RoomId
LEFT JOIN Invoices i ON b.BookingId = i.BookingId
ORDER BY g.GuestId;



SELECT i.InvoiceId
FROM Invoices i
LEFT JOIN Payments p ON i.InvoiceId = p.InvoiceId
WHERE p.InvoiceId IS NULL;


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

-- ========================
-- 2. ÖVERSIKT AV GÄSTER, BOKNINGAR, BETALNINGSSTATUS
-- ========================
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
    b.BookingStatus,
    i.TotalAmount AS InvoiceAmount,
    CASE 
        WHEN i.IsPaid = 1 THEN 'Paid'
        WHEN i.IsPaid = 0 THEN 'Unpaid'
        ELSE 'No Invoice'
    END AS PaymentStatus
FROM 
    Guests g
LEFT JOIN 
    Bookings b ON g.GuestId = b.GuestId
LEFT JOIN 
    Invoices i ON b.BookingId = i.BookingId
ORDER BY 
    g.GuestId;

-- ========================
-- 3. ALLMÄN GÄST- OCH BOKNINGSINFORMATION
-- ========================
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
FROM 
    Guests g
LEFT JOIN 
    Bookings b ON g.GuestId = b.GuestId
ORDER BY 
    g.GuestId;

-- ========================
-- 4. STANDARD SELECT-STATEMENTS
-- ========================
SELECT * FROM Guests;
SELECT * FROM Invoices;
SELECT * FROM Payments;
SELECT * FROM Bookings;
SELECT * FROM Rooms;

-- ========================
-- 5. ADMINISTRATIVA UPPGIFTER
-- ========================
UPDATE Rooms SET IsAvailable = 1; -- Gör alla rum tillgängliga
DELETE FROM Bookings; -- Tar bort alla aktuella bokningar
DBCC CHECKIDENT ('Bookings', RESEED, 0); -- Återställ ID-sekvens för tabellen 'Bookings'

-- ========================
-- 6. GÄST- OCH RUMSINFORMATION MED CHECKINS
-- ========================
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

-- ========================
-- 7. SORTERA TILLGÄNGLIGA RUM EFTER PRIS
-- ========================
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

-- ========================
-- 8. BOKNINGAR INOM EN SPECIFIK PERIOD
-- ========================
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

-- ========================
-- 9. GÄSTER MED EFTERNAMN SOM BÖRJAR MED 'J'
-- ========================
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

-- ========================
-- 10. BOKNINGAR MED NULL CHECKOUT-DATUM
-- ========================
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

-- ========================
-- 11. JOINS: AKTIVA BOKNINGAR MED RUMSINFORMATION
-- ========================
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

-- ========================
-- 12. GROUP BY: BOKNINGAR OCH TOTAL INTÄKT PER RUMSTYP
-- ========================
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

-- ========================
-- 13. OBEGLIGNA BOKNINGAR ÄLDRE ÄN 10 DAGAR
-- ========================
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
