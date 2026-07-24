# OrderShop — Knowledge Base

A single source of truth for the OrderShop backend: what it is, how it's layered, the
relational data model, and every business flow it exposes.

## 1. Overview

OrderShop is an **ASP.NET Core (.NET 8) Web API** backend for a small online shop. It uses
**Entity Framework Core** (SQLite) for persistence. The codebase is deliberately layered so
the flow of each business process is easy to trace end-to-end:

```
HTTP request → Controller → Service (business flow) → Repository → DbContext → database
```

## 2. Technology stack

| Concern | Choice |
|---------|--------|
| Runtime | .NET 8 |
| Web | ASP.NET Core Web API (attribute-routed controllers) |
| ORM | EF Core 8 (SQLite provider) |
| Persistence | SQLite (`shop.db`) |
| DI | Built-in `Microsoft.Extensions.DependencyInjection` |

## 3. Architecture & layering

- **Controllers** (`Controllers/`) — HTTP entry points. Each action delegates to exactly one
  service; controllers never touch `ShopDbContext`.
- **Services** (`Services/`) — one class per business flow (checkout, fulfillment, refund,
  cancellation, review, registration) plus supporting services (pricing, payment, inventory,
  notification). Each flow returns an explicit result and coordinates lower layers.
- **Repositories** (`Data/Repositories/`) — the only place LINQ-to-Entities queries live.
- **DbContext** (`Data/ShopDbContext.cs`) — schema + relationship configuration (Fluent API).
- **Domain** (`Domain/`) — entities and enums, no business logic beyond computed properties.

## 4. Data model & relationships

Nine entities. Every relationship is configured centrally in `ShopDbContext.OnModelCreating`.

| From | To | Cardinality | Foreign key |
|------|-----|-------------|-------------|
| Customer | Order | one-to-many | `Order.CustomerId` |
| Customer | Address | one-to-many | `Address.CustomerId` |
| Category | Product | one-to-many | `Product.CategoryId` |
| Order | OrderItem | one-to-many | `OrderItem.OrderId` |
| Product | OrderItem | one-to-many | `OrderItem.ProductId` |
| Order | Payment | one-to-one | `Payment.OrderId` |
| Order | Shipment | one-to-one | `Shipment.OrderId` |
| Order | Address (shipping) | many-to-one | `Order.ShippingAddressId` |
| Product | Review | one-to-many | `Review.ProductId` |
| Customer | Review | one-to-many | `Review.CustomerId` |

`Order` ↔ `Product` is an effective many-to-many resolved through the `OrderItem` join entity.

```
Customer ──< Order ──< OrderItem >── Product >── Category
   │  │        │                        │
   │  │        ├── 1:1 Payment          └──< Review
   │  │        └── 1:1 Shipment              │
   │  └──< Address ┘ (shipping address)      │
   └──────────────< Review ──────────────────┘
```

## 5. Business flows

Every flow is triggered by an HTTP endpoint and orchestrated by a single service.

| # | Flow | Trigger | Orchestrator |
|---|------|---------|--------------|
| 1 | **Checkout** | `POST /api/checkout/{customerId}` | `CheckoutService` |
| 2 | **Register customer** | `POST /api/customers` | `CustomerService` |
| 3 | **Ship order** | `POST /api/orders/{id}/ship` | `FulfillmentService` |
| 4 | **Deliver order** | `POST /api/orders/{id}/deliver` | `FulfillmentService` |
| 5 | **Cancel order** | `POST /api/orders/{id}/cancel` | `OrderCancellationService` |
| 6 | **Refund order** | `POST /api/orders/{id}/refund` | `RefundService` |
| 7 | **Submit review** | `POST /api/products/{id}/reviews` | `ReviewService` |
| 8 | **Restock product** | `POST /api/products/{id}/restock` | `InventoryService` |

### 5.1 Checkout
`CheckoutController.Checkout` → `CheckoutService.Checkout`:
1. `InventoryService.IsAvailable` — validate stock for every line.
2. `PricingService.Total` — subtotal + tax.
3. `PaymentService.Charge` — take payment for the total.
4. `InventoryService.Reserve` — decrement stock.
5. `OrderRepository.Add` / `Save` — persist the paid order.
6. `NotificationService.OrderConfirmed` — notify the customer.

### 5.2 Register customer
`CustomersController.Register` → `CustomerService.Register`: create the customer with a default
address (`CustomerRepository.Add`/`Save`) → `NotificationService.Welcome`.

### 5.3 Ship order
`OrdersController.Ship` → `FulfillmentService.Ship`: load the paid order
(`OrderRepository.GetWithDetails`) → create a `Shipment` → set status `Shipped` →
`NotificationService.OrderShipped`.

### 5.4 Deliver order
`OrdersController.Deliver` → `FulfillmentService.MarkDelivered`: set the shipment status to
`Delivered` and the order status to `Delivered`.

### 5.5 Cancel order
`OrdersController.Cancel` → `OrderCancellationService.Cancel`: if paid, `PaymentService.Refund`
+ `InventoryService.Restock` each line → set status `Cancelled` → `NotificationService.OrderCancelled`.

### 5.6 Refund order
`OrdersController.Refund` → `RefundService.Refund`: `PaymentService.Refund` →
`InventoryService.Restock` each line → set status `Refunded` → `NotificationService.OrderRefunded`.

### 5.7 Submit review
`ReviewsController.Submit` → `ReviewService.Submit`: validate the product and rating
(`ProductRepository.Get`) → create a `Review` (`ReviewRepository.Add`/`Save`) → recompute the
product's average rating (`ReviewRepository.AverageRatingForProduct`).

### 5.8 Restock product
`ProductsController.Restock` → `InventoryService.Restock` → `ProductRepository.AdjustStock`/`Save`.

## 6. Order lifecycle (status)

```
Pending ──checkout──▶ Paid ──ship──▶ Shipped ──deliver──▶ Delivered
   │                   │                                     │
   └──────cancel───────┴─────────cancel──────────────▶ Cancelled
                                     Delivered ──refund──▶ Refunded
```
