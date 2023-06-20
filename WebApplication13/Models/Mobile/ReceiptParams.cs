using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models.Mobile
{
    public class ReceiptParams
    {
        public string Token { get; set; }
        public string ReceiptCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}