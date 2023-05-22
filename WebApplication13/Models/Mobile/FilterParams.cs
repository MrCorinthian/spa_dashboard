using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication13.DAL;

namespace WebApplication13.Models.Mobile
{
    public class FilterParams
    {
        public int page { get; set; }
        public int userId { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string tierName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string status { get; set; }
    }
}