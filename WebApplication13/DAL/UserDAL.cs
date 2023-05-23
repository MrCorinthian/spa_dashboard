using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WebApplication13.Models;
using WebApplication13.Models.Mobile;

namespace WebApplication13.DAL
{
    public class UserDAL
    {
        public static User GetUserByToken(string token)
        {
            try
            {
                if (token != null && !string.IsNullOrEmpty(token))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        MobileUserLoginToken loginToken = db.MobileUserLoginTokens.FirstOrDefault(c => c.Token == token);
                        if (loginToken != null)
                        {
                            User user = db.Users.FirstOrDefault(c => c.Id == loginToken.MobileUserId);
                            if (user != null)
                            {
                                return user;
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }

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

        public static MobileUserInfo GetMoblieUserInfo(int id)
        {
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";
            MobileUserInfo mobileUserInfo = new MobileUserInfo();
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    var user = db.MobileUsers.FirstOrDefault(c => c.Id == id);
                    if (user != null)
                    {
                        mobileUserInfo.Username = user.Username;
                        mobileUserInfo.TitleName = user.TitleName;
                        mobileUserInfo.FirstName = user.FirstName;
                        mobileUserInfo.LastName = user.LastName;
                        mobileUserInfo.IdCardNumber = user.IdCardNumber;
                        mobileUserInfo.Nationality = user.Nationality;
                        mobileUserInfo.Birthday = user.Birthday;
                        mobileUserInfo.Address = user.Address;
                        mobileUserInfo.Province = user.Province;
                        mobileUserInfo.Occupation = user.Occupation;
                        mobileUserInfo.PhoneNumber = user.PhoneNumber;
                        mobileUserInfo.Email = user.Email;
                        mobileUserInfo.LineId = user.LineId;
                        mobileUserInfo.WhatsAppId = user.WhatsAppId;
                        mobileUserInfo.CompanyName = user.CompanyName;
                        mobileUserInfo.CompanyTexId = user.CompanyTexId;
                        mobileUserInfo.BankAccount = user.BankAccount;
                        mobileUserInfo.BankAccountNumber = user.BankAccountNumber;
                        if (!string.IsNullOrEmpty(user.ProfilePath)) mobileUserInfo.ProfilePath = $"File/ProfileImageWeb/{user.Id}";

                        List<MobileComTier> comTiers = db.MobileComTiers.ToList();
                        List<double> userComs = db.MobileComTransactions.Where(c => c.MobileUserId == user.Id).Select(s => s.TotalBaht).ToList();
                        double comTotal = userComs.Sum();
                        foreach (MobileComTier comTier in comTiers)
                        {
                            if ((comTotal >= comTier.ComBahtFrom && comTotal < comTier.ComBahtTo) || comTier.ComBahtTo == null)
                            {
                                comTotal -= Math.Round(comTier.ComBahtFrom, 0);
                                mobileUserInfo.TierName = comTier.TierName;
                                mobileUserInfo.TierColor = comTier.TierColor;
                                mobileUserInfo.TotalBaht = comTier.ComBahtTo == null ? Math.Round(comTier.ComBahtFrom, 0) : comTotal;
                                mobileUserInfo.MaxBaht = comTier.ComBahtTo ?? comTier.ComBahtFrom;
                                mobileUserInfo.MaxBaht = comTier.ComBahtTo == null ? Math.Round(comTier.ComBahtFrom, 0) : Math.Round(mobileUserInfo.MaxBaht, 0) - Math.Round(comTier.ComBahtFrom, 0);
                                break;
                            }
                        }
                    }
                }
            }
            catch { }

            return mobileUserInfo;
        }

        public static string UserLoginAuth()
        {
            string userAuth = null;
            try
            {
                var noms = System.Runtime.Caching.MemoryCache.Default["names"];
                if (noms != null)
                {
                    userAuth = noms.ToString();
                }

            }
            catch { }

            return userAuth;
        }

        public static string GetUserTier(int id)
        {
            string tierName = null;
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    DateTime now = DateTime.Now;
                    List<MobileComTransaction> comTrans = db.MobileComTransactions.Where(c =>
                                c.MobileUserId == id
                                && c.Created.Year == now.Year
                                ).ToList();

                    double sumCom = comTrans.Sum(s => s.TotalBaht);
                    MobileComTier tier = db.MobileComTiers.FirstOrDefault(c => sumCom >= c.ComBahtFrom && c.ComBahtTo != null ? sumCom <= c.ComBahtTo : true);
                    tierName = tier != null ? tier.TierName : null;
                }

            }
            catch { }

            return tierName;
        }
    }
}