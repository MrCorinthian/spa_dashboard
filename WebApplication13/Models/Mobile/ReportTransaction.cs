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

    public class ReportBranch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public double TotalBaht { get; set; }
        public double TotalPercentage { get; set; }
        public List<MobileComTransaction> Commission { get; set; }
    }
}