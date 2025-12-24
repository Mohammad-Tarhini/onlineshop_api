Online Shop Owner API – ASP.NET Web API

This project is a multi-role e-commerce backend built with ASP.NET Web API, implementing a complete system for:

     Authentication & Authorization (JWT + Role-based)
     Shop Management
     Product Management
     Delivery System (delivery agents / persons / shop delivery)
     Order & Payment workflow
     Caching with Redis
     Admin functionality (categories, system control)

The system follows layered architecture with:

   Controllers
   Application Services
   Domain Entities
   Repositories

External Services (Redis, Cloud Storage, etc.)
✔ Features Overview

    1. Authentication System
        Supports registration and login for:
                  Client
                  Shop Owner
                  Admin
                  Delivery Agents / Delivery Persons

        All authentication uses:
           JWT Tokens
           Role-based authorization attributes

  2. Admin Module

      Admin can:
        Add categories
       Manage shop types
       Run system checks

  3. Shop Management

     Shop owners can:
        Open a new shop
        Update shop profile (including image upload)
        View shop types
        Manage products

  4. Product Management

        Shop owners can:
       Add products
      Upload product images (supports form-data binder)
     Retrieve paginated product lists with filters:
          Search by name
          Search by category
          Search by shop type

   5. Delivery System

      Three delivery roles supported:
      Delivery Agent
      Delivery Person
      Shop Delivery
     Features:
       Add delivery staff
       Delivery staff login
       Client can get delivery agents based on geolocation
       Delivery routes prepared for assignment

  6. Order & Payment Workflow
       Client can:
         Check product availability before checkout
         Place orders with payment
         Retrieve order details
     
       Shop Owners can:
          View incoming orders
         Transfer orders to delivery

       Delivery can:
        View assigned orders
        Deliver orders to clients

   7. Redis Cache Integration
        Simple test endpoint: /api/cache/test
        Used for caching and performance optimization


Technologies used:

    ASP.NET Web API 4.8
    SQL Server / EF
    Redis Cache
    Custom Model Binders for complex form-data
    JWT Authentication

🔒 Security

   JWT authentication
   Request validation via DTOs
   ModelState validation



Use IIS Express or Visual Studio debugger.
