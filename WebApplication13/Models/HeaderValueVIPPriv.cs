using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication13.Models
{
    public class HeaderValueVIPPriv
    {
        public List<MemberPriviledgeItem> MemberPriviledgeList { get; set; }
        public string strLoginName { get; set; }
    }
}