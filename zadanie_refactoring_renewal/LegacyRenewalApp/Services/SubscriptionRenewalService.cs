using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly CustomerRepository _customerRepo;
        private readonly SubscriptionPlanRepository _planRepo;
        private readonly IBillingService _billingService;
        private readonly RenewalValidator _validator;
        private readonly RenewalPriceCalculator _calculator;
        private readonly InvoiceFactory _invoiceFactory;
        private readonly TaxAndFeeProvider _taxAndFeeProvider;

        
        public SubscriptionRenewalService() : this(
            new CustomerRepository(), 
            new SubscriptionPlanRepository(), 
            new LegacyBillingService(),
            new RenewalValidator(),
            new TaxAndFeeProvider(),
            new InvoiceFactory()) { }

        
        public SubscriptionRenewalService(
            CustomerRepository customerRepo, 
            SubscriptionPlanRepository planRepo, 
            IBillingService billingService,
            RenewalValidator validator,
            TaxAndFeeProvider taxAndFeeProvider,
            InvoiceFactory invoiceFactory)
        {
            _customerRepo = customerRepo;
            _planRepo = planRepo;
            _billingService = billingService;
            _validator = validator;
            _taxAndFeeProvider = taxAndFeeProvider;
            _invoiceFactory = invoiceFactory;

            
            _calculator = new RenewalPriceCalculator(new List<IDiscountStrategy> 
            { 
                new CustomerSegmentDiscount(), 
                new LoyaltyDiscount(), 
                new TeamSizeDiscount() 
            });
        }

        public RenewalInvoice CreateRenewalInvoice(int customerId, string planCode, int seatCount, string paymentMethod, bool includePremiumSupport, bool useLoyaltyPoints)
        {
            
            _validator.Validate(customerId, planCode, seatCount, paymentMethod);
            string normPlan = planCode.Trim().ToUpperInvariant();
            string normMethod = paymentMethod.Trim().ToUpperInvariant();

            
            var customer = _customerRepo.GetById(customerId);
            var plan = _planRepo.GetByCode(normPlan);
            if (!customer.IsActive) throw new InvalidOperationException("Inactive customers cannot renew");

            
            var (baseAmount, discountAmount, notes) = _calculator.CalculateTotalDiscounts(customer, plan, seatCount, useLoyaltyPoints);

            decimal subtotal = Math.Max(baseAmount - discountAmount, 300m);
            if (baseAmount - discountAmount < 300m) notes += "minimum discounted subtotal applied; ";

            
            decimal supportFee = CalculateSupportFee(normPlan, includePremiumSupport, ref notes);
            decimal payFeeRate = _taxAndFeeProvider.GetPaymentFeeRate(normMethod, out string methodNote);
            decimal payFee = (subtotal + supportFee) * payFeeRate;
            notes += methodNote;

            decimal taxRate = _taxAndFeeProvider.GetTaxRate(customer.Country);
            decimal taxAmount = (subtotal + supportFee + payFee) * taxRate;
            decimal finalAmount = Math.Max(subtotal + supportFee + payFee + taxAmount, 500m);
            if (subtotal + supportFee + payFee + taxAmount < 500m) notes += "minimum invoice amount applied; ";

            
            var invoice = _invoiceFactory.Create(customer, normPlan, normMethod, seatCount, baseAmount, discountAmount, supportFee, payFee, taxAmount, finalAmount, notes);

            
            _billingService.SaveInvoice(invoice);
            NotifyCustomer(customer, invoice, normPlan);

            return invoice;
        }

        private decimal CalculateSupportFee(string planCode, bool include, ref string notes)
        {
            if (!include) return 0m;
            notes += "premium support included; ";
            return planCode switch { "START" => 250m, "PRO" => 400m, "ENTERPRISE" => 700m, _ => 0m };
        }

        private void NotifyCustomer(Customer customer, RenewalInvoice invoice, string planCode)
        {
            if (string.IsNullOrWhiteSpace(customer.Email)) return;
            _billingService.SendEmail(customer.Email, "Subscription renewal invoice", 
                $"Hello {customer.FullName}, your renewal for {planCode} is ready. Total: {invoice.FinalAmount:F2}.");
        }
    }
}