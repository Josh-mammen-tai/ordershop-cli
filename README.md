# OrderShop CLI

A minimal **C# (.NET 8) console application** that models the core of a small online shop's
order pipeline: a customer places an order for one or more catalog items, stock is checked,
the price and total are calculated, and a receipt is printed to the console.

It is intentionally small and dependency‑free — a single console project with a clean
**Models / Services** split — so the flow of calls between files is easy to read and reason about.

## What it does

- Defines a **customer** and a basket of **order items** (`Models/`)
- Checks **stock availability** for every item (`InventoryService`)
- Calculates the order **subtotal and total** (`PricingService`)
- Orchestrates the end‑to‑end **place‑order** flow (`OrderService`)
- Prints a **receipt** to the console (`Program`)

## Domain model

| Type | Responsibility |
|------|----------------|
| `Customer` | Who is placing the order (id, name, email) |
| `Order` / `OrderItem` | The basket and its line items |
| `InventoryService` | Is each item in stock? |
| `PricingService` | Subtotal and grand total for an order |
| `OrderService` | Validates stock, prices the order, returns an `OrderResult` |
| `Program` | Wires everything together and prints the receipt |

## Project structure

```
OrderShop/
├─ OrderShop.csproj
├─ Program.cs                 Entry point — builds a sample order and prints the receipt
├─ Models/
│  ├─ Customer.cs             Customer
│  └─ Order.cs                Order + OrderItem
└─ Services/
   ├─ InventoryService.cs     Stock lookup
   ├─ PricingService.cs       Subtotal / total calculation
   └─ OrderService.cs         Places an order (uses the models + the two services above)
```

## Build & run

Requires the **.NET 8 SDK**.

```bash
dotnet run
```

Expected output — a formatted receipt for the built‑in sample order:

```
Customer : Ada Lovelace <ada@example.com>
Items    : 2
Total    : $38.00
Status   : Order placed.
```

## License

MIT
