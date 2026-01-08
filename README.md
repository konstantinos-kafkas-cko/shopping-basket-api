# Shopping Basket API

A RESTful API for managing shopping baskets with support for discounts, shipping costs, and VAT calculations.

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Setup and Running](#setup-and-running)
  - [Running from IDE](#running-from-ide)
  - [Running with Docker](#running-with-docker)
- [Authentication](#authentication)
- [Configuration](#configuration)
- [Basket Management](#basket-management)
- [TODO / Future Improvements](#todo--future-improvements)
- [Usage Examples](#usage-examples)

## Overview

The Shopping Basket API provides a complete solution for managing user shopping baskets with the following features:

- **User Authentication**: JWT-based authentication
- **Basket Management**: Add, remove, and view items in shopping baskets
- **Discount Codes**: Apply configurable discount codes
- **Shipping**: Configure shipping costs per country/region
- **VAT Calculation**: Configurable VAT rates with optional inclusion in totals
- **Swagger UI**: Interactive API documentation and testing interface

## Prerequisites

- .NET 10.0 SDK
- Docker and Docker Compose (for Docker deployment)
- IDE (Visual Studio, Rider, or VS Code) for direct execution

## Setup and Running

### Running from IDE

1. Open the solution file `shopping-basket.sln` in your IDE
2. Set `ShoppingBasket.Api` as the startup project
3. Run the application (F5 or Run button)
4. The API will start and Swagger UI will be available at:
   - `http://localhost:5127/swagger/index.html` (HTTP)

### Running with Docker

1. Navigate to the project root directory
2. Run the following command:

```bash
docker-compose -f docker-compose.yml up --build
```

3. The API will be available at:
   - `http://localhost:8080/swagger/index.html`

## Authentication

**Important**: All basket endpoints require authentication. You must authenticate first before accessing any basket operations.

### Step 1: Get JWT Token

1. Open Swagger UI at the URL mentioned above
2. Find the `POST /auth/login` endpoint (1st endpoint)
3. Click "Try it out"
4. Enter your credentials (any username/password is accepted for demo purposes):

```json
{
  "username": "admin",
  "password": "password"
}
```

5. Click "Execute"
6. Copy the **token value only** from the response (without the "Bearer" prefix)
7. Important: Make sure you copy only the token value, without quotation marks.
   Paste it into a text editor first to verify that no extra characters or quotes are included before using it.

Example response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJzdWIiOiJhZG1pbiIsImp0aSI6IjkwYTJjZDcwLWI2NzctNGQ4OS04NDJlLWZiNzk0NjljMzgxNyIsImV4cCI6MTc2NzkwNjE3MiwiaXNzIjoiU2hvcHBpbmdCYXNrZXQiLCJhdWQiOiJTaG9wcGluZ0Jhc2tldCJ9.XULe3bjukNXRfbhm7rhcXk6A1Ze1F3CQkceOoULwxBo"
}
```

**Important!!** Copy only this part: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJzdWIiOiJhZG1pbiIsImp0aSI6IjkwYTJjZDcwLWI2NzctNGQ4OS04NDJlLWZiNzk0NjljMzgxNyIsImV4cCI6MTc2NzkwNjE3MiwiaXNzIjoiU2hvcHBpbmdCYXNrZXQiLCJhdWQiOiJTaG9wcGluZ0Jhc2tldCJ9.XULe3bjukNXRfbhm7rhcXk6A1Ze1F3CQkceOoULwxBo`

### Step 2: Authorize in Swagger

1. Click the **"Authorize"** button at the top right of the Swagger UI
2. In the "Value" field, paste **only the token value** (without "Bearer" prefix)
3. Click "Authorize"
4. Click "Close"

![img.png](img.png)

Now you can execute all protected endpoints. The token will be automatically included in all requests.

---

## Configuration

All configuration is done through `appsettings.json`. The following sections are configurable:

### Discounts

Configure discount codes and their percentage values (as decimals):

```json
"Discounts": {
  "SUMMER10": 0.10,
  "WELCOME5": 0.50
}
```

- `SUMMER10`: 10% discount (0.10)
- `WELCOME5`: 50% discount (0.50)

**Note**: Discount codes can only be applied once per user.

### Shipping

Configure shipping costs per country/region:

```json
"Shipping": {
  "UK": 5.00,
  "US": 15.00
}
```

- `UK`: £5.00 shipping cost
- `US`: $15.00 shipping cost

### VAT

Configure the VAT rate:

```json
"Vat": {
  "Vat": 0.20
}
```

- `0.20`: 20% VAT rate

---

## Basket Management

All basket endpoints require authentication. The user is automatically identified from the JWT token.

#### `POST /api/basket/items`
Add items to the shopping basket. This was added for me to keep track on the basket updates (Not part of the requirements)

**Request Body:**
```json
[
  {
    "productId": "p1",
    "price": 10.00,
    "quantity": 2,
    "isDiscounted": false
  },
  {
    "productId": "p2",
    "price": 5.50,
    "quantity": 1,
    "isDiscounted": false
  }
]
```

**Response:** `200 OK`

**Validation:**
- `productId`: Required, 1-100 characters
- `price`: Required, between 0.01 and 999999
- `quantity`: Optional, defaults to 1, between 1 and 1000
- `isDiscounted`: Optional, defaults to false

---

#### `GET /api/basket/items`
Get all items in the current user's basket.

**Response:** `200 OK`
```json
[
  {
    "username": "admin",
    "productId": "p1",
    "price": 10.00,
    "quantity": 2,
    "isDiscounted": false,
    "totalPrice": 20.00
  }
]
```

**Response:** `404 Not Found` (if basket is empty)

---

#### `DELETE /api/basket/items/{productId}`
Remove an item from the basket.

**Parameters:**
- `productId` (path): The product ID to remove

**Response:** `200 OK` (item removed)
**Response:** `404 Not Found` (item not found)

---

#### `GET /api/basket/total`
Get the total price of the basket.

**Query Parameters:**
- `includeVat` (optional): Boolean, defaults to `true`. If `true`, VAT is included in the total.

**Response:** `200 OK`
```json
114.00
```

**Calculation:**
- Subtotal: Sum of all item prices × quantities
- Discount: Applied discount codes (percentage-based)
- Shipping: Based on configured shipping region
- VAT: Applied to (subtotal - discount + shipping) if `includeVat=true`

---

#### `POST /api/basket/discount-code`
Apply a discount code to the basket.

**Query Parameters:**
- `code` (required): The discount code to apply

**Response:** `200 OK` (discount applied)
**Response:** `400 Bad Request` (invalid or expired code, or code already applied)

**Important Notes:**
- Each discount code can only be applied **once per user**
- Discount codes must be configured in `appsettings.json`
- If a code is already applied, subsequent attempts will return `400 Bad Request`

**Example:**
```
POST /api/basket/discount-code?code=SUMMER10
```

---

#### `POST /api/basket/shipping`
Set the shipping country/region for the basket.

**Query Parameters:**
- `country` (required): The country/region code (e.g., "UK", "US")

**Response:** `200 OK` (shipping set)
**Response:** `400 Bad Request` (country not supported)

**Example:**
```
POST /api/basket/shipping?country=UK
```

---

## Usage Examples

### Complete Workflow

1. **Authenticate:**
   ```
   POST /auth/login
   Body: { "username": "admin", "password": "password" }
   ```
   Copy the token and authorize in Swagger.

2. **Add Items:**
   ```
   POST /api/basket/items
   Body: [
     { "productId": "p1", "price": 100.00, "quantity": 2 },
     { "productId": "p2", "price": 50.00, "quantity": 2 }
   ]
   ```

3. **Set Shipping:**
   ```
   POST /api/basket/shipping?country=UK
   ```

4. **Apply Discount:**
   ```
   POST /api/basket/discount-code?code=SUMMER10
   ```

5. **Get Total (with VAT):**
   ```
   GET /api/basket/total?includeVat=true
   ```
   Returns: `330` (100 * 2 + 50 * 2 = 300 - 10% discount = 270 + 5 shipping = 275 + 20% VAT = 330)

6. **Get Total (without VAT):**
   ```
   GET /api/basket/total?includeVat=false
   ```
   Returns: `275` (100 * 2 + 50 * 2 = 300 - 10% discount = 270 + 5 shipping = 275)

7. **View Basket:**
   ```
   GET /api/basket/items
   ```

8. **Remove Item:**
   ```
   DELETE /api/basket/items/p1
   ```

## TODO / Future Improvements

- **Persistent Data Storage**
   - Introduce database-backed storage for products and baskets
   - Store product prices in one place and validate `productId` when adding items
   - Use an ORM (e.g. Entity Framework Core) for data access and migrations

- **Real Authentication & Authorization**
   - Store users in a database with encrypted and hashed passwords
   - Introduce role-based access control (Admin / User)
   - Admin users can manage products, discounts, shipping, and VAT

- **Dynamic Configuration**
   - Move discounts, shipping costs, and VAT configuration from `appsettings.json` into the database
   - Allow runtime updates without redeploying the application

- **Integration Testing**
   - Add end-to-end integration tests covering all API endpoints

## Important Notes

- **Authentication**: All basket endpoints require a valid JWT token. Always authenticate first using `/auth/login`.
- **Discount Codes**: Each discount code can only be applied **once per user**. Attempting to apply the same code twice will result in a `400 Bad Request` error.
- **User Isolation**: Each user has their own basket. The user is identified from the JWT token claims.
- **Swagger UI**: The easiest way to test the API is through Swagger UI, which provides an interactive interface for all endpoints.
- **Configuration**: All discounts, shipping costs, and VAT rates are configurable through `appsettings.json`.

## Project Structure

```
ShoppingBasket.Api/
├── Controllers/          # API controllers (Auth, Basket)
├── Services/            # Business logic services
├── Repositories/        # Data access layer
├── Models/              # Domain models
├── Options/             # Configuration options
└── Program.cs           # Application entry point
```


