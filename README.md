# E-Commerce Application

Full-stack e-commerce app with ASP.NET Core Web API (clean architecture) + Angular + PostgreSQL.

## Structure

- `backend/` - ASP.NET Core Web API (.NET 10) clean architecture (Domain, Application, Infrastructure, API)
- `frontend/` - Angular 21 + Tailwind CSS

## Prerequisites

- .NET 10 SDK
- Node.js 22+
- PostgreSQL 18 (database `ecommerce`, user `ecommerce` / password `ecommerce`)

## Backend

```bash
cd backend
dotnet restore
dotnet ef database update --project src/ECommerce.Infrastructure --startup-project src/ECommerce.API
dotnet run --project src/ECommerce.API
```

API: http://localhost:5062  (Swagger not enabled; use /api/*)
Admin seed: admin@ecommerce.com / Admin@123

## Frontend

```bash
cd frontend
npm install
npm start
```

App: http://localhost:4200

## Features

- Auth (JWT + refresh), roles (Admin/Customer)
- Products, Categories, Brands with search/filter/pagination
- Cart, Checkout, Orders with status tracking
- Reviews, Ratings, Wishlist
- Addresses, Coupons
- Admin dashboard & CRUD management
- Image upload
- Tailwind CSS UI