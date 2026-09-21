# Inventory Management API
A RESTful Web API built with ASP.NET Core (.NET 10), Entity Framework Core, and Microsoft SQL Server for managing Categories and Products, with pagination, search, and centralized the error handling.

---

## Overview
Manages a simple product catalog — CRUD for Categories and Products, where each Product belongs to a Category.

---

## Features
- Category Management
  - Create, Read, Update, Delete
  - Duplicate name prevention
  - Blocks deletion of categories that still have products assigned
- Product Management
  - Create, Read, Update, Delete
  - Duplicate SKU prevention
  - Validates that the assigned Category exists
- Pagination
  - Page number and page size support on list endpoints
- Search & Filtering
  - Search Categories by name
  - Search Products by name and filter by CategoryId
- Consistent API Response Wrapper
  - Every response returns `success`, `message`, and `data` in the same shape
- Centralized Exception Handling
  - Custom exceptions (`NotFoundException`, `ConflictException`, `BadRequestException`) mapped automatically to the correct HTTP status codes via middleware
- Centralized Messages
  - User-facing response text managed in one place (`Resources/Messages.cs`)
- Entity-DTO Mapping Extensions
  - Reusable `ToDto()`, `ToEntity()`, and `ApplyUpdate()` methods to eliminate repeated mapping code
- Swagger / OpenAPI Documentation
  - Interactive API documentation and testing via Swagger UI

---

## Tech Stack
- Language: C#
- Framework: ASP.NET Core Web API (.NET 10)
- Database: Microsoft SQL Server
- Data Access: Entity Framework Core
- API Documentation: Swashbuckle (Swagger UI)
- IDE: Visual Studio

---

## Project Structure
```text
InventoryManagementAPI/
├── Controllers/
├── Data/
├── Database/
│   └── InventoryManagementSystem.sql
├── Models/
│   ├── Entities/
│   └── DTOs/
│       ├── Category/
│       ├── Product/
│       └── Common/
├── Repositories/
│   └── Interfaces/
├── Resources/
│   └── Messages.cs
├── Shared/
│   ├── Exceptions/
│   ├── Middleware/
│   └── Mappings/
├── appsettings.json
└── Program.cs
```

---

## Getting Started

### Prerequisites
- Visual Studio 2022 (or later)
- .NET 10 SDK
- Microsoft SQL Server (LocalDB, Express, or a full instance)
- SQL Server Management Studio (SSMS) *(optional, for manual database inspection)*

---

## Installation
Clone the repository:
```bash
git clone https://github.com/kingdavidperalta/InventoryManagementAPI.git
```
Open the solution in **Visual Studio**.

---

## Database Setup

### 1. Configure the Connection String
Open **appsettings.json** and set your connection string under `ConnectionStrings`:
```json
{
  "ConnectionStrings": {
    "db_imsAPI": "Server=YOUR_SERVER;Database=db_imsAPI;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
Replace `YOUR_SERVER` with your SQL Server instance.

Example:
```text
DESKTOP-ABC123\SQLEXPRESS
```
or
```text
(localdb)\mssqllocaldb
```

> For sensitive/production connection strings, use **User Secrets** instead of committing real credentials to `appsettings.json`:
> ```bash
> dotnet user-secrets init
> dotnet user-secrets set "ConnectionStrings:db_imsAPI" "your-real-connection-string"
> ```

---

### 2. Restore NuGet Packages
In Visual Studio:
```
Tools
└── NuGet Package Manager
    └── Manage NuGet Packages for Solution
```
Or via terminal:
```bash
dotnet restore
```

---

### 3. Create and Apply Migrations
The database schema is generated from the project's EF Core models — no manual SQL is required to create it. A `.sql` reference script is also included under `Database/InventoryManagementSystem.sql` for review purposes.

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If the `dotnet-ef` tool isn't installed:
```bash
dotnet tool install --global dotnet-ef
```

---

### 4. Run the Application
1. Build the solution.
2. Press **F5** to run the application.
3. Navigate to `/swagger` to view and test the API endpoints.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Categories` | Get paginated list of categories (supports `search`) |
| GET | `/api/Categories/{id}` | Get a single category |
| POST | `/api/Categories` | Create a category |
| PUT | `/api/Categories/{id}` | Update a category |
| DELETE | `/api/Categories/{id}` | Delete a category |
| GET | `/api/Products` | Get paginated list of products (supports `search`, `categoryId`) |
| GET | `/api/Products/{id}` | Get a single product |
| POST | `/api/Products` | Create a product |
| PUT | `/api/Products/{id}` | Update a product |
| DELETE | `/api/Products/{id}` | Delete a product |

---

## Developer
**King David B. Peralta**
Bachelor of Science in Information Technology

### Technologies Used
- C#
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core
- Microsoft SQL Server
- Swagger / Swashbuckle
