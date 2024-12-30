### README for HotelBooking Application

# HotelBooking Application

This project is my **final project** for the course **"Database course"**, showcasing a simple console application for managing hotel bookings. The application allows users to book rooms, manage guests, generate invoices, and track payments. The system is designed to ensure data integrity by adhering to database normalization principles.

## Features

- **Guest Management:** Add, update, remove, and view guest details.
- **Room Booking:** Manage room reservations and check availability.
- **Invoices:** Generate, view, and manage booking-related invoices.
- **Payments:** Track payment statuses and handle unpaid bookings.
- **Database Design:** Optimized structure in 3NF for minimal redundancy.
- **Cancellation History:** Log and view details of canceled bookings

## Technologies

- **Language:** C#
- **Database:** SQL 
- **IDE:** Visual Studio

## NuGet Packages Used

The following NuGet packages are used in the project:
- **Entity Framework Core**
- **Microsoft.Extensions.DependencyInjection** 
- **Newtonsoft.Json** 
- **FluentValidation** 

## Installation

1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/<your-username>/hotelbooking-application.git
   ```
2. Open the project in Visual Studio.
3. Build the project to install dependencies.
4. Run the application.

## Database Design

- **Guests:** Contains guest information, including name, email, and phone number.
- **Rooms:** Stores room details such as type, price per night, size, and availability.
- **Bookings:** Tracks reservations linked to guests and rooms, including check-in/out dates and booking status.
- **Invoices:** Records invoice details with total amounts, payment deadlines, and statuses (paid/unpaid).
- **Payments:** Logs payment transactions linked to invoices, including amounts and payment dates.
- **CanceledBookingHistory:** Maintains a log of canceled bookings, including cancellation reasons and dates.
