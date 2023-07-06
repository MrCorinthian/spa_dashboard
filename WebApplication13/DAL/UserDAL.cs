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
        public static MobileUser GetUserByToken(string token)
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
                            MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == loginToken.MobileUserId && c.Active == "Y");
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
                    DateTime now = DataDAL.GetDateTimeNow();

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
                        mobileUserInfo.CompanyTypeOfUsage = user.CompanyTypeOfUsage;
                        mobileUserInfo.CompanyName = user.CompanyName;
                        mobileUserInfo.CompanyTaxId = user.CompanyTaxId;
                        mobileUserInfo.BankAccount = user.BankAccount;
                        mobileUserInfo.BankAccountNumber = user.BankAccountNumber;
                        if (!string.IsNullOrEmpty(user.ProfilePath)) mobileUserInfo.ProfilePath = $"File/ProfileImageWeb/{user.Id}";

                        List<MobileComTier> comTiers = db.MobileComTiers.OrderBy(o => o.ComBahtFrom).ToList();
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

        public static MobileComTier GetUserTier(int id)
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    DateTime now = DataDAL.GetDateTimeNow();
                    List<MobileComTransaction> comTrans = db.MobileComTransactions.Where(c =>
                                c.MobileUserId == id
                                && c.Created.Year == now.Year
                                ).ToList();

                    double sumCom = comTrans.Sum(s => s.TotalBaht);
                    MobileComTier tier = db.MobileComTiers.Where(c => sumCom >= c.ComBahtFrom && c.ComBahtTo != null ? sumCom <= c.ComBahtTo : true).OrderBy(o => o.ComBahtFrom).FirstOrDefault();

                    return tier;
                }

            }
            catch { }

            return null;
        }

        public static OtpData GenerateOTP(MobileUser user)
        {
            try
            {
                if (user != null)
                {
                    int length = 6;
                    DateTime now = DataDAL.GetDateTimeNow();
                    OtpData otpData = new OtpData();
                    Random random = new Random();

                    const string numbers = "0123456789";
                    string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                    char[] otpRefChars = new char[length];
                    char[] otpChars = new char[length];
                    for (int i = 0; i < length; i++)
                    {
                        otpRefChars[i] = alphabets[random.Next(alphabets.Length)];
                        otpChars[i] = numbers[random.Next(numbers.Length)];
                    }
                    otpData.Ref = new string(otpRefChars);
                    otpData.Otp = new string(otpChars);
                    using (var db = new spasystemdbEntities())
                    {
                        List<MobileOtp> oldOtps = db.MobileOtps.Where(c => c.MobileUserId == user.Id && c.Used == "N" && c.Active == "Y").ToList();
                        foreach (MobileOtp item in oldOtps)
                        {
                            item.Active = "N";
                        }

                        MobileOtp otp = new MobileOtp();
                        otp.MobileUserId = user.Id;
                        otp.Module = 1; //forgot password
                        otp.Ref = otpData.Ref;
                        otp.Otp = otpData.Otp;
                        otp.Used = "N";
                        otp.Active = "Y";
                        otp.Created = now;
                        otp.CreatedBy = DataDAL.GetUserName(user.Id);
                        otp.Updated = now;
                        otp.UpdatedBy = DataDAL.GetUserName(user.Id);

                        db.MobileOtps.Add(otp);
                        db.SaveChanges();
                    }

                    return otpData;
                }
            }
            catch { }

            return null;
        }

        public static OtpData VerifyOtp(int module, string phoneNumber, string _ref, string _otp)
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    DateTime now = DataDAL.GetDateTimeNow();
                    if (!string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(_ref) && !string.IsNullOrEmpty(_otp))
                    {
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.PhoneNumber == phoneNumber);
                        if (user != null)
                        {
                            MobileOtp otp = db.MobileOtps.FirstOrDefault(c => c.MobileUserId == user.Id && c.Module == module && c.Ref == _ref && c.Otp == _otp && c.Used == "N" && c.Active == "Y");
                            if (otp != null)
                            {
                                DateTime otpExpired = otp.Created.AddMinutes(5);
                                if (otpExpired >= now)
                                {
                                    OtpData otpData = new OtpData();
                                    otpData.Ref = otp.Ref;
                                    otpData.Otp = otp.Otp;

                                    return otpData;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return null;
        }
    }
}