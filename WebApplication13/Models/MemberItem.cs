using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class MemberItem
    {
        public string Id { get; set; }
        public string MemberNo { get; set; }
        public string VipType { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string Birth { get; set; }
        public string BirthDay { get; set; }
        public string BirthMonth { get; set; }
        public string BirthYear { get; set; }
        public string AddressInTH { get; set; }
        public string City { get; set; }
        public string TelephoneNo { get; set; }
        public string WhatsAppId { get; set; }
        public string LineId { get; set; }
        public string CreateDate { get; set; }
        public string VipStart { get; set; }
        public string VipStartDay { get; set; }
        public string VipStartMonth { get; set; }
        public string VipStartYear { get; set; }
        public string VipExpire { get; set; }
        public string VipExpireDay { get; set; }
        public string VipExpireMonth { get; set; }
        public string VipExpireYear { get; set; }
        public string Status { get; set; }
        public string MemberGroupId { get; set; }
        public List<MemberGroup> MemberGroupForSelect { get; set; }
        public string PageMode { get; set; }
    }
}