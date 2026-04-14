using System;

namespace LegacyRenewalApp
{
    public class InvoiceFactory
    {
        public RenewalInvoice Create(Customer customer, string plan, string method, int seats, 
            decimal baseAmt, decimal disc, decimal supp, decimal pay, decimal tax, decimal total, string notes)
        {
            return new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customer.Id}-{plan}",
                CustomerName = customer.FullName,
                PlanCode = plan,
                PaymentMethod = method,
                SeatCount = seats,
                BaseAmount = Math.Round(baseAmt, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(disc, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supp, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(pay, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(tax, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(total, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}