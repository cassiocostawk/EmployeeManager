# Employee Manager API

This repository contains a solution structured with **Clean Architecture** principles using **.NET 8**, promoting clear separation of concerns, testability, and scalability.
It is intended as a **technical portfolio project**, focused on backend architecture and good practices.

---

## Project Structure

* **Api**
  ASP.NET Core Web API layer. Handles HTTP requests, authentication, and middleware configuration.

* **Domain**
  Contains core domain entities, enums, and business rules. This layer has no external dependencies.

* **Application**
  Implements business logic and use cases using CQRS (Commands & Queries). Depends only on the Domain layer.

* **Infrastructure**
  Provides implementations for persistence, authentication, and external services.

* **Tests**
  Automated unit tests using **xUnit**, focused on Application use cases.

---

## Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

---

## Getting Started

1. Clone the repository
2. Restore dependencies:

   ```bash
   dotnet restore
   ```
3. Build the solution:

   ```bash
   dotnet build
   ```
4. Run the API project:

   ```bash
   dotnet run --project Api
   ```

---

## Notes

* Follow **Clean Architecture** and **CQRS** guidelines when adding new features.
* Secrets and sensitive configuration values are **for development/testing purposes only**.
* This project intentionally avoids unnecessary frameworks or overengineering to keep the focus on clarity and architecture.
