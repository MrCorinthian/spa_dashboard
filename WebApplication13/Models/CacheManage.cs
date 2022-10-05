using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class CacheManage
    {
        public CacheManage()
        {

        }

        public void GetNames(string username)
        {
            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                noms = username;
                System.Runtime.Caching.MemoryCache.Default["names"] = noms;
            }
            
        }

        public bool checkCache()
        {
            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}