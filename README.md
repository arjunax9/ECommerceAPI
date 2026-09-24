# Full-Stack E-Commerce Web Application

A full-stack, enterprise-grade E-Commerce web application built using **ASP.NET Core Web API (.NET 10)** and **Angular (TypeScript)**. The application implements an N-tier layered architecture following modern software engineering design patterns, complete with role-based authentication, catalog management, inventory tracking, Excel bulk product imports, order lifecycle management, and real-time updates via SignalR.

---

## 🏛 System Architecture

The solution follows a clean separation of concerns across multiple layers:

```
ECommerceAPI/
├── ECommerceAPI/          # Presentation Layer (REST API Controllers, SignalR Hubs, Middleware, DI Configuration)
├── ECommerce.Business/    # Business Logic Layer (Services, Interfaces, Business Rules, Security)
├── ECommerce.Data/        # Data Access Layer (Entity Framework Core DbContext, Repositories, Migrations)
├── ECommerce.Models/      # Domain Layer (Entities, Data Transfer Objects / DTOs)
└── frontend/              # Client Application (Angular SPA, Components, Services, Guards)
```

---

## 🚀 Key Features

- **Authentication & Authorization**:
  - Secure PBKDF2 with SHA-256 password hashing and salt generation.
  - JWT (JSON Web Token) bearer authentication with role-based access control (`Admin`, `Customer`).
  - Route guards and HTTP interceptors on the Angular client.

- **Product & Inventory Management**:
  - Complete CRUD operations for products and real-time stock quantity synchronization.
  - Bulk import capability supporting both `.xlsx` (Excel) and `.csv` file formats via ClosedXML.

- **Cart & Order Processing**:
  - Client-side shopping cart state management with persistent session support.
  - End-to-end checkout pipeline with inventory validation.
  - Order state machine (`Pending` $\rightarrow$ `CheckedOut` $\rightarrow$ `Paid`).
  - Dedicated order approval workflow for customer and administrator portals.

- **Real-Time Communications**:
  - ASP.NET Core SignalR hubs broadcasting live order updates and inventory notifications.
  - Hosted background service (`OrderBackgroundService`) for background order processing.

---

## 🛠 Technology Stack

### Backend
- **Framework**: .NET 10 (C# 13)
- **API Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 10
- **Database**: Microsoft SQL Server / LocalDB
- **Security**: JWT Bearer Tokens, System.Security.Cryptography PBKDF2
- **Real-time Engine**: ASP.NET Core SignalR
- **Excel Processing**: ClosedXML
- **API Documentation**: OpenAPI / Swagger UI

### Frontend
- **Framework**: Angular 16+
- **Language**: TypeScript & HTML5 / CSS3
- **State & Async**: RxJS Observables
- **Networking**: Angular HttpClient with Authorization Interceptors

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js (v18 or newer) & npm](https://nodejs.org/)
- SQL Server (LocalDB or SQL Server Express/Standard)

---

### Step 1: Running the Backend (.NET API)

1. Open a terminal in the root directory:
   ```powershell
   dotnet restore
   dotnet build
   ```

2. Start the API server:
   ```powershell
   dotnet run --project .\ECommerceAPI\ECommerceAPI.csproj
   ```

3. The API will start and seed default sample data automatically:
   - **Base URL**: `http://localhost:5067`
   - **Swagger UI**: `http://localhost:5067/swagger`

---

### Step 2: Running the Frontend (Angular)

1. Open a new terminal in the `frontend` directory:
   ```powershell
   cd frontend
   npm install
   npm start
   ```

2. Open your browser and navigate to:
   ```
   http://localhost:4200
   ```

---

## 🔑 Default Demo Accounts

Upon initial startup, the database automatically seeds the following user credentials:

| Role | Email | Password | Access Level |
| :--- | :--- | :--- | :--- |
| **Administrator** | `admin@local` | `Admin123!` | Full admin panel, inventory management, product CRUD, order approval |
| **Customer** | `demo@local` | `Password123` | Storefront browsing, cart, checkout, order history |

---

## 📚 API Endpoints Summary

### Authentication (`/api/auth`)
- `POST /api/auth/register` — Register a new customer account.
- `POST /api/auth/login` — Authenticate and receive a JWT Bearer token.

### Products & Inventory (`/api/products`)
- `GET /api/products` — Retrieve all active products with inventory stock.
- `GET /api/products/{id}` — Retrieve a single product by ID.
- `POST /api/products` — Add a new product *(Admin only)*.
- `PUT /api/products/{id}` — Update product details and inventory quantity *(Admin only)*.
- `DELETE /api/products/{id}` — Delete a product *(Admin only)*.
- `POST /api/products/import` — Bulk import products from Excel (`.xlsx`) or CSV.

### Orders (`/api/orders`)
- `GET /api/orders` — List all orders across the system *(Admin only)*.
- `GET /api/orders/user/{userId}` — List orders placed by a specific user.
- `GET /api/orders/{id}` — Get order details by ID.
- `POST /api/orders` — Create a new order (`Pending`).
- `POST /api/orders/{id}/checkout` — Progress order to `CheckedOut`.
- `POST /api/orders/{id}/payment` — Process payment and mark as `Paid`.
- `POST /api/orders/{id}/approve` — Approve and complete pending orders.
