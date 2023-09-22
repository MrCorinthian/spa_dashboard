using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models.Mobile
{
    public class CommissionReportData
    {
        public int Id { get; set; }
        public int MobileUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Tier { get; set; }
        public double Commission { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    }

    public class CommissionReportDataIndex
    {
        public int Index { get; set; }
        public List<int> Indices = new List<int>();
        public int RowPerPage = new int();
        public List<CommissionReportData> Data = new List<CommissionReportData>();
    }
}