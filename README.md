# Rick and Morty App

A .NET 8 application that fetches data from the Rick and Morty API and provides a web interface to browse characters, episodes, and locations using ASP.NET Core Razor Pages.

## Project Structure
- **Web App**: ASP.NET Core Razor Pages web application providing the user interface
- **Console App**: Data fetching utility that populates the database from the Rick and Morty API
- **RickAndMortyApp.Test**: Unit test project with service layer tests

## Prerequisites
- .NET 8.0 or later
- SQL Server (Express or full version)
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Create Empty MSSQL Database
Create a new empty database in SQL Server Management Studio or using command line:
```sql
CREATE DATABASE RickAndMortyDB;
```

### 2. Configure Connection Strings
Update the connection strings in both applications to point to your SQL Server instance:

#### Web App - appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RickAndMortyDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### Console App - appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RickAndMortyDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Run Entity Framework Migrations
Navigate to the Web App directory and run the database update command:

```bash
cd WebApp
dotnet ef database update
```

This will create the necessary tables and schema in your database.

### 4. Populate Database
Run the Console App to fetch data from the Rick and Morty API and populate your database:

```bash
cd ConsoleApp
dotnet run
```

The console application will:
- Fetch characters from the Rick and Morty API
- Fetch episodes and locations
- Store the data in your SQL Server database

### 5. Run the Web Application
Start the web application to browse the data:

```bash
cd WebApp
dotnet run
```


## Testing

### Unit Tests
The project includes comprehensive unit tests for the service layer using xUnit and in-memory Entity Framework databases.

**Run all unit tests:**
```bash
dotnet test
```

**Run tests from the test project directory:**
```bash
cd RickAndMortyApp.Test
dotnet test
```



### Test Helpers
The test project uses helper classes for:
- **DbContextHelper** - Provides in-memory database contexts for testing
- **HttpClientHelper** - Creates mock HTTP clients for API testing

## Features
- Browse Rick and Morty characters
- View character details including episodes they appear in
- Search and filter functionality
- Web interface
- Unit tested service layer




## Technologies Used
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Rick and Morty API (https://rickandmortyapi.com/)
- xUnit (Testing Framework)
- Entity Framework InMemory (For Unit Testing)
- Microsoft.Extensions.Caching.Memory (Caching)
