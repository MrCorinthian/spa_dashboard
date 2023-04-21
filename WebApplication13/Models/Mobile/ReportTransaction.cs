using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models.Mobile
{
    public class ReportTransaction
    {
        public double? TotalBaht { get; set; }
        public DateTime? Created { get; set; }
        public string PaymentStatus { get; set; }
    }
}