﻿using System;
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
    }
}