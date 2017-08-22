using System;

namespace AWSLambda1
{
    public class GetAccountDetailsResponse 
    {
        public string AccountId { get; set; }
        public string CollectionClass { get; set; }
        public string CustomerClass { get; set; }
        public string PaymentClass { get; set; }
        public bool ClbFlag { get; set; }
        public bool ApsFlag { get; set; }
        public bool HasRebates { get; set; }
        public bool HasPayPlan { get; set; }
        public bool IsLandlord { get; set; }
        public bool HasDisconnectedService { get; set; }
        public string PaymentBlockingCode { get; set; }
        public decimal DepositAmountReceived { get; set; }
        public decimal DepositBalanceDue { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal ActualAccountBalance { get; set; }
        public decimal DelinquentAmount { get; set; }
        public DateTime LastBillingDate { get; set; }
        public DateTime LastBillDueDate { get; set; }
        public decimal PaymentAmountDue { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public string CreditRatingCode { get; set; }
        public bool ActiveCollectionsFlag { get; set; }
        public bool ActiveSeveranceFlag { get; set; }
        public bool EbillFlag { get; set; }
        public bool EbillDueFlag { get; set; }
        public DateTime FutureRunDate { get; set; }
        public bool ProjectShareFlag { get; set; }
        public decimal ProjectShareAmount { get; set; }
        public bool WindPowerFlag { get; set; }
        public decimal WindPowerAmount { get; set; }
        public bool ImExistsFlag { get; set; }
        public string Name { get; set; }
    }
}