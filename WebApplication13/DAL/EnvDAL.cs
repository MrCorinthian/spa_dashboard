using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace WebApplication13.DAL
{
    public class EnvDAL
    {
        public static string GetEnvironmentVariable(string variableName)
        {
            string[] lines = File.ReadAllLines($"{HostingEnvironment.ApplicationPhysicalPath}\\.env");

            foreach (string line in lines)
            {
                string[] parts = line.Split('=');

                if (parts.Length == 2 && parts[0].Trim() == variableName)
                {
                    return parts[1].Trim();
                }
            }

            return null;
        }
    }
}