# OrderShop

A small but realistic **ASP.NET Core (.NET 8) + EF Core backend** for an online shop.
It has a **relational data model** with real relationships (foreign keys + navigation
properties, configured with the EF Core Fluent API) and several **business flows** —
checkout, fulfillment, and refund — layered as `Controllers → Services → Repositories → DbContext`.

## Data model & relationships

Eight entities (`Domain/Entities`) wired together in `Data/ShopDbContext.cs`:

| Relationship | Cardinality |
|--------------|-------------|
| `Customer` → `Order` | one-to-many |
| `Customer` → `Address` | one-to-many |
| `Category` → `Product` | one-to-many |
| `Order` → `OrderItem` | one-to-many |
| `Product` → `OrderItem` | one-to-many (`Order` ↔ `Product` many-to-many via `OrderItem`) |
| `Order` → `Payment` | one-to-one |
| `Order` → `Shipment` | one-to-one |
| `Order` → `Address` (shipping) | many-to-one |

```
Customer ──< Order ──< OrderItem >── Product >── Category
   │           │
   │           ├── 1:1 Payment
   │           ├── 1:1 Shipment
   └──< Address ┘ (shipping address)
```

## Business flows

| Flow | Orchestrator | Steps |
|------|--------------|-------|
| **Checkout** | `CheckoutService` | validate stock → price (subtotal + tax) → charge payment → reserve stock → persist order → notify |
| **Fulfillment** | `FulfillmentService` | create shipment for a paid order → mark shipped → mark delivered → notify |
| **Refund** | `RefundService` | refund payment → restock items → mark order refunded → notify |

Each flow returns an explicit result and coordinates the supporting services
(`InventoryService`, `PricingService`, `PaymentService`, `NotificationService`) over the
repository data layer.

## Project structure

```
OrderShop/
├─ OrderShop.csproj
├─ Program.cs                     Host + DI wiring (DbContext, repositories, services)
├─ appsettings.json
├─ Directory.Build.props          Repo-wide build + analyzer settings
├─ .editorconfig                  Formatting & naming rules
├─ Domain/
│  ├─ Enums.cs                    OrderStatus, PaymentMethod, PaymentStatus, ShipmentStatus
│  └─ Entities/                   Customer, Address, Category, Product, Order,
│                                 OrderItem, Payment, Shipment
├─ Data/
│  ├─ ShopDbContext.cs            DbSets + Fluent API relationship configuration
│  └─ Repositories/               Order / Product / Customer repositories (interfaces + impl)
├─ Services/                      Pricing, Payment, Inventory, Notification,
│                                 Checkout, Fulfillment, Refund
├─ Controllers/                   Checkout, Orders, Products (Web API)
└─ docs/
   └─ CODING-STANDARDS.md
```

## API surface

| Method & route | Flow |
|----------------|------|
| `POST /api/checkout/{customerId}?method=Card` | Checkout |
| `POST /api/orders/{id}/ship?carrier=DHL` | Fulfillment — ship |
| `POST /api/orders/{id}/refund` | Refund |
| `GET  /api/orders/{id}` | Read an order with all relations |
| `GET  /api/products/{id}` | Read a product with its category |
| `GET  /api/products/category/{categoryId}` | List products in a category |

## Build & run

Requires the **.NET 8 SDK**.

```bash
dotnet restore
dotnet run
```

The API listens on the default Kestrel ports and creates a SQLite database
(`shop.db`) from the connection string in `appsettings.json`.

## Coding standards

Conventions are documented in [`docs/CODING-STANDARDS.md`](docs/CODING-STANDARDS.md)
and enforced by `.editorconfig` + `Directory.Build.props` at build time.

## License

MIT
