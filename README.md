# DuitTracker API

ASP.NET Core Web API for DuitTracker personal finance tracker.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- MediatR (CQRS pattern)
- FluentValidation
- Scalar (API documentation)

## Project Structure

    DuitTracker.Api/
    ├── Controllers/
    │   ├── Categories/
    │   │   └── CategoriesController.cs
    │   └── Transactions/
    │       └── TransactionsController.cs
    ├── Features/
    │   ├── Categories/
    │   │   ├── CreateCategory.cs
    │   │   ├── UpdateCategory.cs
    │   │   ├── DeleteCategory.cs
    │   │   ├── GetAllCategories.cs
    │   │   └── GetCategoryById.cs
    │   └── Transactions/
    │       ├── CreateTransaction.cs
    │       ├── UpdateTransaction.cs
    │       ├── DeleteTransaction.cs
    │       ├── GetAllTransactions.cs
    │       └── GetTransactionById.cs
    └── Shared/
        ├── Domain/
        │   ├── BaseClass.cs
        │   ├── Category.cs
        │   └── Transaction.cs
        ├── Infrastructure/
        │   ├── Migrations/
        │   └── Persistence/
        │       └── DuitDbContext.cs
        └── Middleware/
            └── GlobalExceptionHandlingMiddleware.cs

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop

### Setup

1. Clone the repository
```bash
   git clone https://github.com/ilham0209/DuitTracker.Api.git
   cd DuitTracker.Api
```

2. Copy appsettings example
```bash
   cp appsettings.example.json appsettings.json
```

3. Update connection string in `appsettings.json`

4. Start PostgreSQL
```bash
   docker compose up -d
```

5. Run migrations
```bash
   dotnet ef database update
```

6. Run the API
```bash
   dotnet run
```

### API Documentation

Access Scalar UI at:

https://localhost:{port}/scalar

## Architecture

- **CQRS** — Commands and Queries are separated
- **Vertical Slice** — each feature is self-contained
- **Soft Delete** — records are never permanently deleted
- **Global Exception Handling** — consistent error responses
