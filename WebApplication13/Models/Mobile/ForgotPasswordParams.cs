﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models.Mobile
{
    public class ForgotPasswordParams
    {
        public string Ref { get; set; }
        public string Otp { get; set; }
        public string PhoneNumber { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}