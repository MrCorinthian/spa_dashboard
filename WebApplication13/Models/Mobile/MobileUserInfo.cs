﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models.Mobile {
    public class MobileUserInfo
{
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TitleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdCardNumber { get; set; }
        public string Nationality { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Address { get; set; }
        public Nullable<int> Province { get; set; }
        public Nullable<int> Occupation { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LineId { get; set; }
        public string WhatsAppId { get; set; }
        public Nullable<int> CompanyTypeOfUsage { get; set; }
        public string CompanyName { get; set; }
        public string CompanyTaxId { get; set; }
        public string CompanyAddress { get; set; }
        public Nullable<int> Bank { get; set; }
        public string BankAccountNumber { get; set; }
        public string ProfilePath { get; set; }
        public string Active { get; set; }
        public string TierName { get; set; }
        public string TierColor { get; set; }
        public double TotalBaht { get; set; }
        public double MaxBaht { get; set; }
    }
}