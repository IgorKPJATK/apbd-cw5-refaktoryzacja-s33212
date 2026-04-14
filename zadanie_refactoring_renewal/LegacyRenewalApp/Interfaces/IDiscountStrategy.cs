namespace LegacyRenewalApp;

public interface IDiscountStrategy
{
    (decimal Amount, string Note) Calculate(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount);
}