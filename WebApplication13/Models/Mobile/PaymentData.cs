using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models.Mobile
{
    public class PaymentData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePath { get; set; }
        public string PhoneNumber { get; set; }
        public string Tier { get; set; }
        public double Commission { get; set; }
        public string CompanyName { get; set; }
        public string CompanyTaxId { get; set; }
        public Nullable<int> Bank { get; set; }
        public string BankAccountNumber { get; set; }
        public bool Payment { get; set; }
        public string PaymentStatus { get; set; }

        public List<MobileComTransaction> ComTrans = new List<MobileComTransaction>();
    }

    public class PaymentDataIndex
    {
        public int Index { get; set; }
        public List<int> Indices = new List<int>();
        public List<PaymentData> Data = new List<PaymentData>();
    }
}