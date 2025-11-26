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

Use Redis for caching

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

📌 API Endpoints Summary
Admin
Method	Endpoint	Description
POST	/api/Addcategory	Add categories
GET	/api/cache/test	Test Redis connection
Authentication
Role	Register Endpoint	Login Endpoint
Client	/api/registeration/client	/api/Login/Client
Shop Owner	/api/registeration/shopowner	/api/Login/shopowner
Admin	/api/registeration/admin	/api/Login/admin
Delivery	/api/delivery/deliverylogin	Login for delivery agents
Delivery Module
Method	Endpoint	Description
POST	/api/delivery/adddeliveryagent	Add delivery agent
POST	/api/delivery/adddeliveryperson	Add delivery person
POST	/api/delivery/adddeliveryshop	Add delivery shop
GET	/api/delivary/getdeliveryonloction	Get delivery staff based on location
Product Module
Method	Endpoint	Description
POST	/api/product/addproduct	Add a new product
POST	/api/product/addimageforproduct	Upload product image
GET	/api/product/getproducts	Get products with filtering & pagination
Shop Module
Method	Endpoint	Description
POST	/api/shop/opennewshop	Create a shop
POST	/api/shop/updateprofile	Update shop profile
GET	/api/shop/getshoptypes	Get shop categories
GET	/api/shop/Getshops	(Assumed continuation) List shops
Order & Payment Module
Method	Endpoint	Description
POST	/api/order/checkavailability	Check cart availability
POST	/api/order/pay	Process payment & register order
GET	/api/order/getordersforshop	Shop: view new orders
GET	/api/order/getitemsoforder	Get items of an order
GET	/api/order/getordersfordelivery	Delivery: orders assigned
POST	/api/order/giveorderfromshoptodelivery	Shop hands order to delivery
POST	/api/order/giveorderfromdeliverytoclient	Delivery hands order to client
🏛 Architecture
/Controllers
/Application
    /Services
    /Dtos
/Domain
    /Entities
    /Interfaces
/Infrastructure
    /Repositories
    /ExternalServices (Redis, Storage, etc.)


Technologies used:

ASP.NET Web API 4.8

SQL Server / EF

Redis Cache

Custom Model Binders for complex form-data

JWT Authentication

🔒 Security

JWT authentication

Role-based authorization:
[JwtAuthorize(Roles = "admin")]

Request validation via DTOs

ModelState validation

📦 Installation & Run
1. Restore NuGet packages
nuget restore

2. Setup Database

Run migrations or SQL scripts

Update connection string in Web.config

3. Run the API

Use IIS Express or Visual Studio debugger.
