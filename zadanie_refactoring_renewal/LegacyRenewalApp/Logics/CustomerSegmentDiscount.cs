namespace LegacyRenewalApp
{
    public class CustomerSegmentDiscount : IDiscountStrategy
    {
        public (decimal Amount, string Note) Calculate(Customer customer, SubscriptionPlan plan, int seatCount, decimal baseAmount)
        {
            if (customer.Segment == "Silver") return (baseAmount * 0.05m, "silver discount; ");
            if (customer.Segment == "Gold") return (baseAmount * 0.10m, "gold discount; ");
            if (customer.Segment == "Platinum") return (baseAmount * 0.15m, "platinum discount; ");
            if (customer.Segment == "Education" && plan.IsEducationEligible) return (baseAmount * 0.20m, "education discount; ");
            return (0, string.Empty);
        }
    }
}