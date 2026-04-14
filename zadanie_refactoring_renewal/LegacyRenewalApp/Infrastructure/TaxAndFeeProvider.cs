using System;

namespace LegacyRenewalApp
{
    public class TaxAndFeeProvider
    {
        public decimal GetTaxRate(string country) => country switch
        {
            "Poland" => 0.23m,
            "Germany" => 0.19m,
            "Czech Republic" => 0.21m,
            "Norway" => 0.25m,
            _ => 0.20m
        };

        public decimal GetPaymentFeeRate(string method, out string note)
        {
            note = method switch
            {
                "CARD" => "card payment fee; ",
                "BANK_TRANSFER" => "bank transfer fee; ",
                "PAYPAL" => "paypal fee; ",
                "INVOICE" => "invoice payment; ",
                _ => throw new ArgumentException("Unsupported payment method")
            };

            return method switch
            {
                "CARD" => 0.02m,
                "BANK_TRANSFER" => 0.01m,
                "PAYPAL" => 0.035m,
                _ => 0m
            };
        }
    }
}