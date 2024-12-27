### README for HotelBooking Application

# HotelBooking Application

This project is my **final project** for the course **"Database course"**, showcasing a simple console application for managing hotel bookings. The application allows users to book rooms, manage guests, generate invoices, and track payments. The system is designed to ensure data integrity by adhering to database normalization principles.

## Features

- **Guest Management:** Add, update, and view guest information.
- **Room Booking:** Book rooms, check availability, and manage reservations.
- **Invoices:** Generate and view invoices linked to bookings.
- **Payments:** Track payments and their statuses.
- **Database Structure:** Tables normalized to 3NF for better data quality and reduced redundancy.

## Technologies

- **Language:** C#
- **Database:** SQL (with normalization principles)
- **IDE:** Visual Studio

## NuGet Packages Used

The following NuGet packages are used in the project:
- **Entity Framework Core** – For database interaction and ORM capabilities.
- **Microsoft.Extensions.DependencyInjection** – For managing dependency injection.
- **Newtonsoft.Json** – For handling JSON serialization and deserialization.
- **FluentValidation** – For validating input data.
- **Dapper** (optional) – For lightweight database queries.
- **NUnit** – For unit testing.

## Installation

1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/<your-username>/hotelbooking-application.git
   ```
2. Open the project in Visual Studio.
3. Build the project to install dependencies.
4. Run the application.

## Database Design

The application uses the following tables:
- **Guests:** Stores guest details (name, email, phone number).
- **Rooms:** Stores room details (type, price, size).
- **Bookings:** Stores reservations linked to guests and rooms.
- **Invoices:** Stores invoice details with payment statuses.
- **Payments:** Tracks payments linked to invoices.
