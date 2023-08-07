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
        public static DateTime GetDateTimeNow()
        {
            // Get the current UTC time
            DateTime utcNow = DateTime.UtcNow;

            // Specify the time zone for Bangkok
            TimeZoneInfo bangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert the UTC time to Bangkok's time zone
            DateTime bangkokTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, bangkokTimeZone);

            return bangkokTime;
        }

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

        public static string GetActiveFlag(string value)
        {
            try
            {

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.ToLower() == "enable" || value.ToLower() == "completed")
                    {
                        value = "Y";
                    }
                    else if (value.ToLower() == "disable" || value.ToLower() == "not completed")
                    {
                        value = "N";
                    }
                    else
                    {
                        value = null;
                    }
                }
            }
            catch { }

            return value;
        }

        public static string GetProfilePath(string path)
        {
            string result = null;
            List<string> file = new List<string>();
            if (!string.IsNullOrEmpty(path)) file = path.Split(new[] { "\\" }, StringSplitOptions.None).ToList();
            if (file.Count > 0) result = $"File/ProfileImageWebUpload?fileName={file.LastOrDefault()}";
            return result;
        }

        public static string GetUserName(int id)
        {
            using (var db = new spasystemdbEntities())
            {
                string name = null;
                MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == id);
                if (user != null && !string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                {
                    name = $"{user.LastName.ElementAt(0)}.{user.FirstName}".ToLower();
                }
                return name;
            }
        }
        public static string GetUserNameByName(string firstName, string lastName)
        {
            using (var db = new spasystemdbEntities())
            {
                string name = null;
                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                {
                    name = $"{lastName.ElementAt(0)}.{firstName}".ToLower();
                }
                return name;
            }
        }

        public static void ErrorLog(string exception, string message, string userName = "")
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    DateTime now = GetDateTimeNow();
                    MobileErrorLog error = new MobileErrorLog();
                    if (!string.IsNullOrEmpty(exception) && !string.IsNullOrEmpty(message))
                    {
                        error.Exception = exception;
                        error.Message = message;
                        error.Created = now;
                        if(!string.IsNullOrEmpty(userName)) error.CreatedBy = userName;
                        db.MobileErrorLogs.Add(error);
                        db.SaveChanges();
                    }
                }
            }
            catch { }
        }

        public static List<Branch> GetBranchList()
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    List<Branch> result = new List<Branch>();

                    List<int> ignoreBranch = new List<int>();
                    ignoreBranch.Add(1);
                    ignoreBranch.Add(2);
                    ignoreBranch.Add(3);
                    ignoreBranch.Add(9);
                    //ignoreBranch.Add(99);

                    List<Branch> query = db.Branches.Where(c => !ignoreBranch.Contains(c.Id)).OrderBy(o => o.Id).ToList();
                    if (query.Count > 0)
                    {
                        result.AddRange(query);
                        return result;
                    }
                }
            }
            catch { }

            return new List<Branch>();
        }
    }
}
