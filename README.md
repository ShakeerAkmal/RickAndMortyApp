# Rick and Morty App

A .NET application that fetches data from the Rick and Morty API and provides a web interface to browse characters, episodes, and locations.

## Project Structure
- **Web App**: ASP.NET Core web application providing the user interface
- **Console App**: Data fetching utility that populates the database from the Rick and Morty API
- **RickAndMortyApp.Test**: Unit test project with service layer tests

## Prerequisites
- .NET 6.0 or later
- SQL Server (Express or full version)
- Visual Studio or VS Code

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

**Note**: Replace the connection string with your actual SQL Server details:
- For SQL Server Express: `Server=.\\SQLEXPRESS;Database=RickAndMortyDB;Trusted_Connection=true;MultipleActiveResultSets=true`
- For full SQL Server: `Server=localhost;Database=RickAndMortyDB;Trusted_Connection=true;MultipleActiveResultSets=true`

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

The web app will be available at `https://localhost:5001` or `http://localhost:5000`

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

**Run tests with detailed output:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

**Current unit test coverage includes:**

1. **CharacterServiceTests**
   - `CreateCharacter_ValidData_ReturnsCharacterId` - Tests character creation with valid data
   - `CreateCharacter_InvalidOriginLocationId_ThrowsArgumentException` - Tests validation for invalid origin location
   - `CreateCharacter_InvalidCurrentLocationId_ThrowsArgumentException` - Tests validation for invalid current location

2. **EpisodeServiceTests**
   - `FetchAndSaveEpisodesAsync_SavesEpisodesToDb` - Tests episode data fetching and saving

3. **LocationServiceTests**
   - `FetchAndSaveLocationsAsync_SavesLocationsToDb` - Tests location data fetching and saving

### Test Coverage Report
Generate test coverage reports:

```bash
# Install coverage tools (if not already installed)
dotnet tool install --global coverlet.console

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Helpers
The test project uses helper classes for:
- **DbContextHelper** - Provides in-memory database contexts for testing
- **HttpClientHelper** - Creates mock HTTP clients for API testing

## Features
- Browse Rick and Morty characters
- View character details including episodes they appear in
- Search and filter functionality
- Responsive web interface
- Unit tested service layer

## API Endpoints
The web application provides RESTful API endpoints for:
- Characters: `/api/characters`
- Episodes: `/api/episodes`
- Locations: `/api/locations`

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server is running
- Verify the connection string matches your SQL Server configuration
- Check that the database exists and is accessible

### Migration Issues
- Ensure you have Entity Framework CLI tools installed: `dotnet tool install --global dotnet-ef`
- Make sure you're in the correct project directory when running migrations

### Console App Issues
- Check your internet connection (required to fetch data from Rick and Morty API)
- Verify the database connection string in the console app configuration

### Test Issues
- Make sure you're running tests from the solution root or test project directory
- Ensure all test dependencies are restored: `dotnet restore`

## Technologies Used
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Rick and Morty API (https://rickandmortyapi.com/)
- xUnit (Testing Framework)
- Entity Framework InMemory (For Unit Testing)
- Microsoft.Extensions.Caching.Memory (Caching)

## Getting Help
If you encounter any issues during setup, please check:
1. All prerequisites are installed
2. Connection strings are correctly configured
3. SQL Server is running and accessible
4. Entity Framework migrations completed successfully
5. Test dependencies are properly restored