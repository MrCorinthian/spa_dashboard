using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class MemberPriviledgeItem
    {
        public string Id { get; set; }
        public string ShowName { get; set; }
        public string PriviledgeTypeId { get; set; }
        public string PriviledgeTypeName { get; set; }
        public string Value { get; set; }
        public string StartDate { get; set; }
        public string ExpireDate { get; set; }
        public string Status { get; set; }
    }
}