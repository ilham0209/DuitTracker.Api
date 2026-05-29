# DuitTracker API

ASP.NET Core Web API for DuitTracker — a personal finance tracker application.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL 16
- MediatR (CQRS pattern)
- FluentValidation
- MailKit (Email service)
- JWT Authentication
- Scalar (API documentation)

## Project Structure

    DuitTracker.Api/
    ├── Controllers/
    │   ├── Auth/
    │   │   └── AuthController.cs
    │   ├── Budgets/
    │   │   └── BudgetsController.cs
    │   ├── Categories/
    │   │   └── CategoriesController.cs
    │   ├── Dashboard/
    │   │   └── DashboardController.cs
    │   ├── PaymentMethods/
    │   │   └── PaymentMethodsController.cs
    │   └── Transactions/
    │       └── TransactionsController.cs
    ├── Features/
    │   ├── Auth/
    │   │   ├── RegisterUser.cs
    │   │   ├── LoginUser.cs
    │   │   ├── ForgotPassword.cs
    │   │   ├── ResetPassword.cs
    │   │   └── ChangePassword.cs
    │   ├── Budgets/
    │   │   ├── CreateBudget.cs
    │   │   ├── EditBudget.cs
    │   │   ├── DeleteBudget.cs
    │   │   ├── GetAllBudget.cs
    │   │   └── GetByIdBudget.cs
    │   ├── Categories/
    │   │   ├── CreateCategory.cs
    │   │   ├── EditCategory.cs
    │   │   ├── DeleteCategory.cs
    │   │   ├── GetAllCategory.cs
    │   │   └── GetByIdCategory.cs
    │   ├── Dashboard/
    │   │   └── GetDashboardSummary.cs
    │   ├── PaymentMethods/
    │   │   ├── CreatePaymentMethod.cs
    │   │   ├── EditPaymentMethod.cs
    │   │   ├── DeletePaymentMethod.cs
    │   │   ├── GetAllPaymentMethod.cs
    │   │   └── GetByIdPaymentMethod.cs
    │   └── Transactions/
    │       ├── CreateTransaction.cs
    │       ├── EditTransaction.cs
    │       ├── DeleteTransaction.cs
    │       ├── GetAllTransaction.cs
    │       └── GetByIdTransaction.cs
    └── Shared/
        ├── Behaviours/
        │   └── ValidationPipelineBehavior.cs
        ├── Configuration/
        │   └── EmailSettings.cs
        ├── Domain/
        │   ├── BaseClass.cs
        │   ├── Budget.cs
        │   ├── Category.cs
        │   ├── PasswordResetToken.cs
        │   ├── PaymentMethod.cs
        │   ├── Transaction.cs
        │   └── User.cs
        ├── Infrastructure/
        │   ├── Migrations/
        │   └── Persistence/
        │       └── DuitDbContext.cs
        ├── Middleware/
        │   └── GlobalExceptionHandlingMiddleware.cs
        └── Services/
            ├── CurrentUserService.cs
            ├── EmailService.cs
            └── JwtService.cs

## API Endpoints

### Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | /api/auth/register | ❌ | Register new account |
| POST | /api/auth/login | ❌ | Login and get JWT token |
| POST | /api/auth/forgot-password | ❌ | Request password reset email |
| POST | /api/auth/reset-password | ❌ | Reset password using token |
| POST | /api/auth/change-password | ✅ | Change current password |

### Categories
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | /api/categories | ✅ | Create category |
| GET | /api/categories | ✅ | Get all categories |
| GET | /api/categories/{id} | ✅ | Get category by ID |
| PUT | /api/categories/{id} | ✅ | Edit category |
| DELETE | /api/categories/{id} | ✅ | Soft delete category |

### Transactions
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | /api/transactions | ✅ | Create transaction |
| GET | /api/transactions | ✅ | Get all transactions |
| GET | /api/transactions/{id} | ✅ | Get transaction by ID |
| PUT | /api/transactions/{id} | ✅ | Edit transaction |
| DELETE | /api/transactions/{id} | ✅ | Soft delete transaction |

### Payment Methods
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | /api/paymentmethods | ✅ | Create payment method |
| GET | /api/paymentmethods | ✅ | Get all payment methods |
| GET | /api/paymentmethods/{id} | ✅ | Get payment method by ID |
| PUT | /api/paymentmethods/{id} | ✅ | Edit payment method |
| DELETE | /api/paymentmethods/{id} | ✅ | Soft delete payment method |

### Budgets
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | /api/budgets | ✅ | Create budget |
| GET | /api/budgets | ✅ | Get all budgets |
| GET | /api/budgets/{id} | ✅ | Get budget by ID |
| PUT | /api/budgets/{id} | ✅ | Edit budget |
| DELETE | /api/budgets/{id} | ✅ | Soft delete budget |

### Dashboard
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | /api/dashboard | ✅ | Get summary (optional ?year=2026) |

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop
- Gmail account with App Password enabled

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

3. Update `appsettings.json` with your configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=DuitTrackerDb;Username=sa;Password=yourpassword"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-minimum-32-characters",
    "Issuer": "DuitTracker",
    "Audience": "DuitTracker",
    "ExpiryInMinutes": 60
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-gmail@gmail.com",
    "SenderName": "DuitTracker",
    "Password": "your-gmail-app-password"
  }
}
```

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
```
http://localhost:{port}/scalar
```

## Architecture

- **Clean Architecture** — clear separation of concerns across layers
- **CQRS** — Commands and Queries are separated via MediatR
- **Vertical Slice** — each feature is fully self-contained in one file
- **Soft Delete** — records are never permanently deleted, uses `IsDeleted` flag
- **Audit Trail** — all entities track `SysUserCreated`, `SysDateCreated`, `SysUserModified`, `SysDateModified`
- **Global Exception Handling** — consistent error responses across all endpoints
- **JWT Authentication** — stateless auth with Bearer token
- **EF Core Global Query Filter** — auto-filters `IsDeleted == false` on all queries
- **FluentValidation Pipeline** — validation runs automatically before every handler