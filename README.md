# OrderShop CLI

A small **C# (.NET 8) console application** that models the core of an online shop's
**checkout flow**: a customer places an order, stock is checked, the order is priced,
payment is taken, the order is stored, and the customer is notified.

It is intentionally dependency‑free and organised into clear layers — **Models**,
**Services**, and a **Repositories** data layer — so the flow of calls between files is
easy to read and reason about.

## The checkout business flow

`CheckoutService.Checkout(order, method)` orchestrates the end‑to‑end flow:

1. **Validate & price** — `OrderService.PlaceOrder` checks stock via `InventoryService`
   and prices the order via `PricingService`.
2. **Take payment** — `PaymentService.Charge` approves or declines the payment for the total.
3. **Persist** — the order is saved to the `OrderRepository` (data layer).
4. **Notify** — `NotificationService` confirms the order to the customer.

## Layers & responsibilities

| Layer | Type | Responsibility |
|-------|------|----------------|
| Model | `Customer` | Who is placing the order |
| Model | `Order` / `OrderItem` | The basket and its line items |
| Model | `Payment` / `PaymentMethod` / `PaymentResult` | A payment attempt and its outcome |
| Service | `InventoryService` | Is each item in stock? |
| Service | `PricingService` | Subtotal and total for an order |
| Service | `PaymentService` | Charge a payment for the order total |
| Service | `NotificationService` | Send the order confirmation |
| Service | `OrderService` | Validate stock, price the order, return an `OrderResult` |
| Service | `CheckoutService` | The end‑to‑end checkout flow (uses every layer) |
| Data | `OrderRepository` | In‑memory persistence for placed orders |
| Entry | `Program` | Wires everything together and runs a sample checkout |

## Project structure

```
OrderShop/
├─ OrderShop.csproj
├─ Program.cs                 Entry point — runs the checkout flow for a sample order
├─ Models/
│  ├─ Customer.cs             Customer
│  ├─ Order.cs                Order + OrderItem
│  └─ Payment.cs              Payment + PaymentMethod + PaymentResult
├─ Services/
│  ├─ InventoryService.cs     Stock lookup
│  ├─ PricingService.cs       Subtotal / total calculation
│  ├─ PaymentService.cs       Charges a payment for the total
│  ├─ NotificationService.cs  Sends order confirmations
│  ├─ OrderService.cs         Validates stock + prices the order
│  └─ CheckoutService.cs      Orchestrates the full checkout business flow
└─ Repositories/
   └─ OrderRepository.cs      In-memory order persistence (data layer)
```

## Build & run

Requires the **.NET 8 SDK**.

```bash
dotnet run
```

Expected output — the checkout flow for the built‑in sample order:

```
[notify] ada@example.com: your order is confirmed (PAY-Card-38.00).
Customer : Ada Lovelace <ada@example.com>
Items    : 2
Status   : Checkout complete. PAY-Card-38.00
Stored   : 1 order(s)
```

## License

MIT
