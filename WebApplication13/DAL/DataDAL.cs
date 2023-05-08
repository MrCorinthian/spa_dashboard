using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication13.Models;
using WebApplication13.Models.Mobile;

namespace WebApplication13.DAL
{
    public class DataDAL
    {
        public static string GetMobileSetting(string code)
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    MobileSetting query = db.MobileSettings.FirstOrDefault(c => c.Code == code);
                    if (query != null) return query.Value;
                }
            }
            catch { }
            return null;
        }
    }
}