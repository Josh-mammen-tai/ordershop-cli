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
        Customer? customer = _customerRepository.GetById(customerId);
        if (customer is null)
        {
            return;
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
        Customer? customer = _customerRepository.GetById(customerId);
        if (customer is null || customer.LoyaltyPoints < points)
        {
            return;
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
        return customer?.LoyaltyPoints ?? 0;
    }
}
