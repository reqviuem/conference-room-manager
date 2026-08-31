# RoomManager

RoomManager is a .NET-based Web API designed for efficient management and booking of conference rooms or event halls. It allows users to manage room details, search for available rooms based on capacity and time, and handle bookings with optional additional services (e.g., Wi-Fi, Projectors).

## Features

- **Room Management**: Create, update, and delete room records including capacity and hourly pricing.
- **Service Integration**: Manage additional services that can be attached to rooms and bookings.
- **Availability Search**: Find rooms available for specific time slots and minimum capacity requirements.
- **Booking System**: Book rooms for specific intervals, including automatic price calculation based on room rate and selected services.
- **API Documentation**: Integrated Swagger/OpenAPI support for easy testing and integration.
- **Auto-Migrations**: Automatically applies database migrations on startup.

## Tech Stack

- **Language**: C#
- **Framework**: .NET 10.0 (ASP.NET Core Web API)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **API Docs**: Swashbuckle (Swagger UI) / Microsoft.AspNetCore.OpenApi

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/)

## Configuration

1.  Clone the repository.
2.  Update the connection string in `RoomManager/appsettings.Development.json` (or `appsettings.json`):
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=RoomManager;Username=your_username;Password=your_password"
    }
    ```

## Getting Started

1.  **Restore dependencies**:
    ```bash
    dotnet restore
    ```
2.  **Run the application**:
    ```bash
    dotnet run --project RoomManager
    ```
    The application will automatically apply migrations to your PostgreSQL database on the first run.

3.  **Access API Documentation**:
    Once the app is running, navigate to:
    - Swagger UI: `http://localhost:<port>/swagger`
    - OpenAPI JSON: `http://localhost:<port>/api-docs/v1.json`
    (Check your terminal output or `Properties/launchSettings.json` for the exact port).

## API Endpoints

The API is accessible at the following routes:

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/create` | Create a new room |
| `PATCH` | `/update` | Update existing room details |
| `DELETE` | `/delete` | Delete a room |
| `GET` | `/rooms` | Find available rooms by capacity and time |
| `POST` | `/book` | Create a room booking |

## Data Models

- **Room**: Represents a space available for booking (Name, Capacity, PricePerHour).
- **Service**: Additional equipment or features (e.g., Projector, Wi-Fi, Sound).
- **Booking**: Stores information about reserved slots, linked rooms, and selected services.

### Default Seed Data
The project includes predefined data for quick testing:
- **Rooms**: Hall A (50 cap, 2000/hr), Hall B (100 cap, 3500/hr), Hall C (30 cap, 1500/hr).
- **Services**: Projector (500), Wi-Fi (300), Sound (700).
