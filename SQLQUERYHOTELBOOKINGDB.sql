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
