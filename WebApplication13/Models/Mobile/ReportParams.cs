using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models.Mobile
{
    public class ReportParams
    {
        public string Token { get; set; }
        public int MobileUserId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
    }
}