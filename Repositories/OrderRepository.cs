using System.Collections.Generic;
using OrderShop.Models;

namespace OrderShop.Repositories;

/// <summary>
/// In-memory persistence for placed orders. Stands in for a database so the
/// rest of the app can depend on a data-access layer without any real store.
/// </summary>
public sealed class OrderRepository
{
    private readonly List<Order> _orders = new();

    /// <summary>Persist a placed order.</summary>
    public void Save(Order order) => _orders.Add(order);

    /// <summary>Number of orders currently stored.</summary>
    public int Count => _orders.Count;

    /// <summary>All stored orders, in insertion order.</summary>
    public IReadOnlyList<Order> All() => _orders;
}
