using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class RenewalPriceCalculator
    {
        private readonly List<IDiscountStrategy> _strategies;

        public RenewalPriceCalculator(List<IDiscountStrategy> strategies)
        {
            _strategies = strategies;
        }

        public (decimal BaseAmount, decimal DiscountAmount, string Notes) CalculateTotalDiscounts(
            Customer customer, SubscriptionPlan plan, int seatCount, bool useLoyaltyPoints)
        {
            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
            decimal discountAmount = 0;
            string notes = "";

            foreach (var strategy in _strategies)
            {
                var (amount, note) = strategy.Calculate(customer, plan, seatCount, baseAmount);
                discountAmount += amount;
                notes += note;
            }

            if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
            {
                int points = Math.Min(customer.LoyaltyPoints, 200);
                discountAmount += points;
                notes += $"loyalty points used: {points}; ";
            }

            return (baseAmount, discountAmount, notes);
        }
    }
}