using System;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

public sealed class LoyaltyService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILoyaltyPointsHistoryRepository _loyaltyPointsHistoryRepository;

    public LoyaltyService(
        ICustomerRepository customerRepository,
        ILoyaltyPointsHistoryRepository loyaltyPointsHistoryRepository)
    {
        _customerRepository = customerRepository;
        _loyaltyPointsHistoryRepository = loyaltyPointsHistoryRepository;
    }

    public void AccumulatePoints(int customerId, int points)
    {
        if (points < 0)
        {
            throw new ArgumentException("Points cannot be negative.", nameof(points));
        }

        Customer? customer = _customerRepository.GetById(customerId);
        if (customer is null)
        {
            throw new InvalidOperationException("Customer not found.");
        }

        customer.LoyaltyPoints += points;
        _customerRepository.Save();

        _loyaltyPointsHistoryRepository.Add(new LoyaltyPointsHistory
        {
            CustomerId = customerId,
            Points = points,
            Action = "Earned",
            Date = DateTime.UtcNow,
        });
    }

    public void RedeemPoints(int customerId, int points)
    {
        if (points < 0)
        {
            throw new ArgumentException("Points cannot be negative.", nameof(points));
        }

        Customer? customer = _customerRepository.GetById(customerId);
        if (customer is null)
        {
            throw new InvalidOperationException("Customer not found.");
        }

        if (customer.LoyaltyPoints < points)
        {
            throw new InvalidOperationException("Insufficient points for redemption.");
        }

        customer.LoyaltyPoints -= points;
        _customerRepository.Save();

        _loyaltyPointsHistoryRepository.Add(new LoyaltyPointsHistory
        {
            CustomerId = customerId,
            Points = -points,
            Action = "Redeemed",
            Date = DateTime.UtcNow,
        });
    }

    public int GetPointsBalance(int customerId)
    {
        Customer? customer = _customerRepository.GetById(customerId);
        return customer!.LoyaltyPoints;
    }
}
