﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class spasystemdbEntities : DbContext
    {
        public spasystemdbEntities()
            : base("name=spasystemdbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<MassagePlan> MassagePlans { get; set; }
        public virtual DbSet<MassageSet> MassageSets { get; set; }
        public virtual DbSet<MassageTopic> MassageTopics { get; set; }
        public virtual DbSet<OrderRecord> OrderRecords { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Version> Versions { get; set; }
        public virtual DbSet<OtherSale> OtherSales { get; set; }
        public virtual DbSet<OtherSaleRecord> OtherSaleRecords { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
        public virtual DbSet<DiscountMaster> DiscountMasters { get; set; }
        public virtual DbSet<DiscountMasterDetail> DiscountMasterDetails { get; set; }
        public virtual DbSet<DiscountRecord> DiscountRecords { get; set; }
        public virtual DbSet<OrderRecordWithDiscount> OrderRecordWithDiscounts { get; set; }
    }
}
