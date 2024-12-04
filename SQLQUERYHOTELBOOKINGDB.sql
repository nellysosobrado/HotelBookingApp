USE HotelBookingDB

SELECT * FROM Bookings
SELECT * FROM Rooms
SELECT * FROM Guests
SELECT * FROM Invoices
SELECT * FROM Payments

UPDATE Rooms SET IsAvailable =1; -- All room available
DELETE FROM Bookings; --Remove all current bookings
DBCC CHECKIDENT ('Bookings', RESEED, 0); -- resetest the ID seeding 
