using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication13.Models;

namespace WebApplication13.DAL
{
    public class UserDAL
    {
        public static string CreateLoginToken(int mobileUserId)
        {
            string token = "";
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    DateTime now = DateTime.Now;

                    List<MobileUserLoginToken> allToken = db.MobileUserLoginTokens.Where(c => c.MobileUserId == mobileUserId && c.Active == "Y").ToList();
                    foreach (MobileUserLoginToken _t in allToken)
                    {
                        _t.Active = "N";
                    }

                    int loginSection = 3;
                    MobileUserLoginToken loginToken = new MobileUserLoginToken();
                    token = Guid.NewGuid().ToString();
                    loginToken.Token = token;
                    loginToken.MobileUserId = mobileUserId;
                    loginToken.Created = now;
                    loginToken.Expired = now.AddMonths(loginSection);
                    loginToken.Active = "Y";
                    db.MobileUserLoginTokens.Add(loginToken);
                    db.SaveChanges();
                    return token;
                }
            }
            catch { }

            return token;
        }
    }
}