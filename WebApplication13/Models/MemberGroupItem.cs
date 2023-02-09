using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class MemberGroupItem
    {
        public string MemberGroupId { get; set; }
        public string MemberPriviledgeId { get; set; }
        public string MemberGroupPriviledgeId { get; set; }
        public string MemberGroupName { get; set; }
        public string MemberGroupShowName { get; set; }
        public string MemberPriviledgeName { get; set; }
        public string Status { get; set; }
    }
}