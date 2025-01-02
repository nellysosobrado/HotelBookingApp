### README for HotelBooking Application

# HotelBooking Application
This is my **final project** for the **"Database Course"**. It's a simple console application designed for receptionists to manage hotel bookings. The app allows them to book rooms, manage guest details, create invoices, and track payments efficiently. The database is built using SQL with a **Code-First approach** for storing and retrieving booking and guest information.

## Database Design
![ER_NylegnaKir_Sosobrado_HotelBookignApp drawio (1)](https://github.com/user-attachments/assets/bc8f4f4a-6c43-42bd-b5f5-76d21f48b77b)

## Features

- **Guest Management:** Add, update, remove, and view guest details.
- **Room Booking:** Manage room reservations, add, delete, uppdate and check availability.
- **Invoices:** manage booking-related invoices
- **Payments:** Track payment statuses and handle unpaid bookings.
- **Cancellation History:** view details of canceled bookings

## Technologies
- **Language:** C#
- **Database:** SQL 
- **IDE:** Visual Studio

## NuGet Packages Used
The following NuGet packages are used in the project:
- Spectre.Console
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Configuration.Json
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore
- FluentValidation
- Bogus
- Autofac.Extensions.DependencyInjection

## Installation
1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/<your-username>/hotelbooking-application.git
   ```
2. Open the project in Visual Studio.
3. F5, Run the application.

