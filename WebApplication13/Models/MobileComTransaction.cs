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
    
    public partial class MobileComTransaction
    {
        public int Id { get; set; }
        public int MobileUserId { get; set; }
        public int BranchId { get; set; }
        public double TotalBaht { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    }
}
