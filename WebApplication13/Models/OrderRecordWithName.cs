using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class OrderRecordWithName
    {
        public string Date { get; set; }
        public System.TimeSpan Time { get; set; }
        public string MassageTopicName { get; set; }
        public string MassagePlanName { get; set; }
        public string Price { get; set; }
        public string Commission { get; set; }
        public string CancelStatus { get; set; }
    }
}