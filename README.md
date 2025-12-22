# Employee Manager API

## 1. Overview

A RESTful API for employee management, featuring JWT authentication, role-based permissions, and robust business rules.  
This project was developed as a portfolio piece, demonstrating Clean Architecture and modern .NET backend practices.

---

## 2. Stack / Technologies

- .NET 8
- ASP.NET Core Web API
- Clean Architecture
- CQRS with MediatR
- Entity Framework Core
- JWT Authentication
- FluentValidation
- HealthChecks
- ILogger (built-in logging, used especially in exception middleware)
- xUnit (unit testing)
- Docker (optional)
- Swagger (OpenAPI)

---

## 3. Related Repositories

The frontend WebApp for this project is available at:

- [https://github.com/cassiocostawk/EmployeeManager.WebApp](https://github.com/cassiocostawk/EmployeeManager.WebApp)

---

## 4. Architecture

The solution follows Clean Architecture principles, ensuring clear separation of concerns and testability:

- **Domain**: Core entities, enums, and business rules (no external dependencies)
- **Application**: Business logic and use cases (CQRS: Commands & Queries)
- **Infrastructure**: Persistence, authentication, and external services
- **API**: HTTP endpoints, authentication, and middleware
  - **Application.Tests**: Automated unit tests using xUnit, focused on Application use cases

CQRS is used to separate read and write operations. All business rules are enforced in the backend.  

---

## 5. Business Rules

- Underage employees cannot be registered
- Duplicate document numbers and email are not allowed
- An employee cannot create another with a higher permission level
  - Leaders can create Employees
  - Directors can create Leaders and Employees

---

## 6. Authentication & Authorization

- JWT Bearer authentication
- Token includes UserId and Role
- `[Authorize]` attribute on controllers (except login)
- `[AllowAnonymous]` only on login endpoint

---

## 7. Main Endpoints

- Login
  - `POST /api/auth/login`
- Employee
  - `GET api/employees`
  - `GET /api/employees/{id}`
  - `POST /api/employees/Create`
  - `PUT /api/employees/Update/{id}`
  - `DELETE /api/employees/Delete/{id}`
  
Full API documentation is available via Swagger.

---

## 8. Validation

- Frontend: basic input validation
- Backend:  
  - FluentValidation for input models  
  - Business rules enforced in CommandHandlers

---

## 9. Error Handling

- Exceptions are thrown in domain/application layers
- Global middleware captures and converts errors to:
  - 400 Bad Request
  - 401 Unauthorized
  - 403 Forbidden
  - 404 Not Found

---

## 10. Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- Docker (optional)

### Steps

#### Local Setup

1. Clone the repository
2. Restore dependencies:

    ```bash
    dotnet restore
    ```

3. Apply database migrations:

    ```bash
    dotnet ef database update
    ```

4. Run the API:

    ```bash
    dotnet run --project Api
    ```

#### Docker Setup

1. Start the containers (API and SQL Server) using Docker Compose:

    ```bash
    docker-compose up -d --build
    ```

2. The API will be available at http://localhost:5000
3. To stop the containers:

    ```bash
    docker-compose down
    ```
  
---

## 11. Initial Credentials

For initial access, use the following seeded credentials (created via database seed):

- **Director**
  - Email: admin@email.com
  - Password: Admin@123

You can use this account to authenticate and test the API and WebApp.

---

## 12. Notes

- JWT secrets and sensitive configs are for development/testing only
- The project is focused on architectural clarity and best practices
- Not intended for production use
