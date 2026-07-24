# Coding Standards

These standards are enforced by [`.editorconfig`](../.editorconfig) and
[`Directory.Build.props`](../Directory.Build.props) — most are checked at build time
(`EnforceCodeStyleInBuild=true`, `TreatWarningsAsErrors=true`).

## Language & compiler

- **Target framework:** .NET 8, `LangVersion=latest`.
- **Nullable reference types** are enabled everywhere. No `!`-silencing except for
  EF navigation properties initialised by the ORM (`= null!;`).
- **Warnings are errors.** Code must build clean.
- **XML doc comments** on public types and members (the build generates the doc file).

## Layout

- **File-scoped namespaces**, one type per file (a type and its small result/DTO
  companion may share a file).
- `using` directives go **outside** the namespace, `System.*` sorted first.
- **4-space** indentation for C#, **2-space** for project/JSON files.
- Lines wrap at **120** columns.

## Naming

| Symbol | Convention | Example |
|--------|-----------|---------|
| Types, public members, methods | `PascalCase` | `CheckoutService`, `PlaceOrder` |
| Interfaces | `I` + `PascalCase` | `IOrderRepository` |
| Private fields | `_camelCase` | `_orders` |
| Locals & parameters | `camelCase` | `orderId` |
| Constants | `PascalCase` | `TaxRate` |

## Style

- Prefer **explicit types** over `var` for readability.
- Use **language keywords** (`int`, `string`) over framework types (`Int32`).
- **Always brace** blocks, including single-line `if`s.
- Prefer **pattern matching**, null-propagation (`?.`), and coalescing (`??`).
- Mark fields **`readonly`** where possible; prefer immutable results.
- No `this.` qualification.

## Architecture conventions

- **Layering:** `Controllers → Services → Repositories → DbContext`. A layer may
  only call the layer directly beneath it. Controllers never touch `ShopDbContext`.
- **Entities** live in `Domain/Entities` and hold no business logic beyond simple
  computed properties (e.g. `OrderItem.LineTotal`).
- **Relationships** are configured centrally via the Fluent API in
  `ShopDbContext.OnModelCreating` — not scattered across `[ForeignKey]` attributes.
- **Business flows** are orchestrated in `Services` (`CheckoutService`,
  `FulfillmentService`, `RefundService`); each returns an explicit result type
  rather than throwing for expected failures.
- Repositories are the **only** place LINQ-to-Entities queries are written.
