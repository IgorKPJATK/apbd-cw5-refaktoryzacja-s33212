namespace LegacyRenewalApp;

public interface IDiscountStrategy
{
        decimal CalculateDiscount(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount);
}