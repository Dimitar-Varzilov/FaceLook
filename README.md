# FaceLook

FaceLook is a social networking web application built with ASP.NET Core MVC. It lets registered users send and receive messages in real time, as well as upload and manage pictures stored in Azure File Share. The project was built as a learning exercise covering identity, SignalR, Azure storage, and a clean layered architecture.

## Features

- User registration and login using ASP.NET Core Identity
- Send, edit, and delete messages between users
- Real-time message delivery using SignalR
- Upload pictures to Azure File Share and view them via time-limited SAS URLs
- Email notifications sent through Gmail SMTP
- Global exception handling middleware with typed exceptions (ResourceNotFoundException, ValidationException)

## Solution Structure

The solution is split into several projects to keep things organized:

- **FaceLook.Web** - the main MVC web application with controllers and views
- **FaceLook.Services** - business logic, service interfaces, SignalR hub, middleware, and extensions
- **FaceLook.Data** - Entity Framework Core DbContext and migrations
- **FaceLook.Entities** - domain entities (User, Message, Picture)
- **FaceLook.Web.ViewModels** - view models and request/response models
- **FaceLook.Common** - shared constants, enums, and validation attributes

## Tech Stack

- ASP.NET Core MVC (.NET 9)
- Entity Framework Core with SQL Server
- ASP.NET Core Identity
- SignalR for real-time chat
- AutoMapper
- Azure File Share (for picture storage)
- SMTP email sending

## Setup

### Prerequisites

- .NET 9 SDK
- SQL Server LocalDB (ships with Visual Studio)
- An Azure Storage account with a File Share created
- A Gmail account with an app password for SMTP

### Configuration

Create or update `appsettings.Development.json` in the `FaceLook.Web` project with the following:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-FaceLook-f2808d80-0439-42c2-9070-24167481675a;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "MailServerOptions": {
    "Host": "smtp.gmail.com",
    "Port": "465",
    "UseSsl": "true",
    "UserName": "mitaka911@gmail.com",
    "Password": "ztkk jlko rwwb vkok",
    "SenderName": "Facelook",
    "SenderEmail": "mail@facelook.com",
    "RecepientName": "Facelook user"
  },
  "FileShareOptions": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=dimitarsstorage;AccountKey=kWsEzBm1OydUUJZAnU31nZEaiiJ6+2uZHrPugRNI4wSn8PgqYitiMv9eF25Th/wgIQXfDfXvjv91+AStuGkUHQ==;EndpointSuffix=core.windows.net",
    "FileShare": "dimitarsfileshares",
    "SasExpiryInHours": "1"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

### Running the app

1. Clone the repository
2. Add the `appsettings.Development.json` file as shown above
3. Apply the database migrations:

```bash
dotnet ef database update --project FaceLook.Data/FaceLook.Data.csproj --startup-project FaceLook/FaceLook.Web.csproj
```

4. Run the project:

```bash
dotnet run --project FaceLook.Web
```

5. Open your browser at `https://localhost:{port}`, register an account and you are good to go.

## Notes

- Pictures are stored in Azure File Share. SAS URLs are generated on the fly with a configurable expiry (default 1 hour).
- Real-time messaging is handled through a SignalR hub. Users are grouped by email address, so messages are pushed directly to the sender and receiver.
- The app uses a custom `ExceptionHandlingMiddleware` that maps typed service exceptions to appropriate HTTP responses.
