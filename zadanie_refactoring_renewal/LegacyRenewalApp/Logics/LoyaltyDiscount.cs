namespace LegacyRenewalApp;

public class LoyaltyDiscount : IDiscountStrategy
{
    public (decimal Amount, string Note) Calculate(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount)
    {
        if (customer.YearsWithCompany >= 5) return (baseAmount * 0.07m, "long-term loyalty discount; ");
        if (customer.YearsWithCompany >= 2) return (baseAmount * 0.03m, "basic loyalty discount; ");
        return (0, string.Empty);
    }
}