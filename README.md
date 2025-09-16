# Rick and Morty App

A **.NET 8** application that uses the [Rick and Morty API](https://rickandmortyapi.com/) to browse characters, episodes, and locations.
It includes a console tool to fetch data into a SQL Server database and a web app to explore it.

---

## What Each App Does

* **RickAndMortyApp.Console**

  * Connects to the public [Rick and Morty API](https://rickandmortyapi.com/)
  * Downloads characters, episodes, and locations
  * Saves that data into your SQL Server database
  * Run anytime to refresh your local data

* **RickAndMorty.WebApp**

  * ASP.NET Core Razor Pages web app for browsing the database
  * Features:

    * **Origin-based browsing:** navigate to

      ```
      ~/characters/from/{locationName}
      ```

      to see all characters whose origin is `{locationName}`
    * **Swagger UI** at `/swagger` for exploring and testing API endpoints

---

## Project Structure

```
RickAndMortyApp/
├── RickAndMorty.WebApp/       # ASP.NET Core Razor Pages web app
├── RickAndMortyApp.Console/   # Console app to download and populate data
├── RickAndMortyApp.Data/      # Entity Framework Core data layer
├── RickAndMorty.Services/     # Business logic services
├── RickAndMortyApp.Test/      # Unit tests
└── RickAndMortyApp.sln        # Solution file
```

---

## Quick Start

1. **Clone & Restore**

   ```bash
   git clone https://github.com/ShakeerAkmal/RickAndMortyApp.git
   cd RickAndMortyApp
   dotnet restore
   ```

2. **Create the Database**
   Create an empty SQL Server database (e.g., `RickAndMortyDB`).

3. **Set Connection String**
   Edit `RickAndMorty.WebApp/appsettings.json` and `RickAndMortyApp.Console/appsettings.json` with your SQL Server connection.

4. **Apply Migrations**

   ```bash
   cd RickAndMorty.WebApp
   dotnet ef database update
   ```

5. **Load Data**

   ```bash
   cd ../RickAndMortyApp.Console
   dotnet run
   ```

6. **Run the Web App**

   ```bash
   cd ../RickAndMorty.WebApp
   dotnet run
   ```



## Technologies

* .NET 8
* ASP.NET Core Razor Pages
* Entity Framework Core 9
* SQL Server
* xUnit & Moq (unit testing)
* EF Core InMemory (unit tests)
* Microsoft.Extensions.Caching.Memory
* Swashbuckle.AspNetCore (Swagger)
* Rick and Morty API

---


  ```
