# OrderShop

A small but realistic **ASP.NET Core (.NET 8) + EF Core backend** for an online shop. It has a
**relational data model** with real relationships (foreign keys + navigation properties,
configured with the EF Core Fluent API) and **eight business flows** exposed as HTTP endpoints,
layered as `Controllers → Services → Repositories → DbContext`.

> Full design detail lives in [`docs/KNOWLEDGE-BASE.md`](docs/KNOWLEDGE-BASE.md).

## Business flows

Each flow is triggered by an HTTP endpoint and orchestrated by a single service:

| # | Flow | Endpoint | Orchestrator |
|---|------|----------|--------------|
| 1 | Checkout | `POST /api/checkout/{customerId}?method=Card` | `CheckoutService` |
| 2 | Register customer | `POST /api/customers` | `CustomerService` |
| 3 | Ship order | `POST /api/orders/{id}/ship?carrier=DHL` | `FulfillmentService` |
| 4 | Deliver order | `POST /api/orders/{id}/deliver` | `FulfillmentService` |
| 5 | Cancel order | `POST /api/orders/{id}/cancel` | `OrderCancellationService` |
| 6 | Refund order | `POST /api/orders/{id}/refund` | `RefundService` |
| 7 | Submit review | `POST /api/products/{id}/reviews` | `ReviewService` |
| 8 | Restock product | `POST /api/products/{id}/restock?quantity=10` | `InventoryService` |

The **Checkout** flow, for example, runs: validate stock → price (subtotal + tax) → charge
payment → reserve stock → persist order → notify. See the KB doc for the full step list of
every flow.

## Data model & relationships

Nine entities (`Domain/Entities`) wired together in `Data/ShopDbContext.cs`:

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
| `Product` → `Review` | one-to-many |
| `Customer` → `Review` | one-to-many |

```
Customer ──< Order ──< OrderItem >── Product >── Category
   │  │        │                        │
   │  │        ├── 1:1 Payment          └──< Review
   │  │        └── 1:1 Shipment              │
   │  └──< Address ┘ (shipping address)      │
   └──────────────< Review ──────────────────┘
```

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
│                                 OrderItem, Payment, Shipment, Review
├─ Data/
│  ├─ ShopDbContext.cs            DbSets + Fluent API relationship configuration
│  └─ Repositories/               Order / Product / Customer / Review repositories
├─ Services/                      Pricing, Payment, Inventory, Notification, Checkout,
│                                 Fulfillment, Refund, OrderCancellation, Review, Customer
├─ Controllers/                   Checkout, Orders, Products, Customers, Reviews
└─ docs/
   ├─ KNOWLEDGE-BASE.md
   └─ CODING-STANDARDS.md
```

## Build & run

Requires the **.NET 8 SDK**.

```bash
dotnet restore
dotnet run
```

The API listens on the default Kestrel ports and creates a SQLite database (`shop.db`) from the
connection string in `appsettings.json`.

## Coding standards

Conventions are documented in [`docs/CODING-STANDARDS.md`](docs/CODING-STANDARDS.md) and enforced
by `.editorconfig` + `Directory.Build.props` at build time.

## License

MIT
