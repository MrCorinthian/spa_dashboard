//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication13.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DiscountRecord
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int AccountId { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Time { get; set; }
        public int DiscountMasterId { get; set; }
        public int DiscountMasterDetailId { get; set; }
        public string Value { get; set; }
        public string IsCreditCard { get; set; }
        public string CancelStatus { get; set; }
        public Nullable<System.DateTime> CreateDateTime { get; set; }
        public Nullable<System.DateTime> UpdateDateTime { get; set; }
        public Nullable<int> OrderReceiptId { get; set; }
    }
}
