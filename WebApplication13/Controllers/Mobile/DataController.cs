using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using WebApplication13.Models;
using WebApplication13.Models.Mobile;
using System.Web.Http.Cors;
using WebApplication13.DAL;
using Newtonsoft.Json;

namespace WebApplication13.Controllers.Mobile
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DataController : ApiController
    {
        #region mobile app
        [HttpPost]
        public async Task<IHttpActionResult> GetCommissionTier(ResponseData token)
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    if (token != null && !string.IsNullOrEmpty(token.Data))
                    {
                        MobileUserLoginToken loginToken = db.MobileUserLoginTokens.FirstOrDefault(c => c.Token == token.Data);
                        if (loginToken != null)
                        {
                            var comTier = db.MobileComTiers.OrderBy(o => o.ComBahtFrom).ToList();
                            if (comTier.Count > 0)
                            {
                                return Ok(comTier);
                            }
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetMobileOptionSetting(string code)
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    string jsonString = DataDAL.GetMobileSetting(code);
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        List<string> stringList = JsonConvert.DeserializeObject<List<string>>(jsonString);
                        return Ok(stringList);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetMobileOption(string code)
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    string query = DataDAL.GetMobileSetting(code);
                    if (!string.IsNullOrEmpty(query))
                    {
                        return Ok(query);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }
        #endregion

        [HttpPost]
        public async Task<IHttpActionResult> GetCommissionTierSetting(FilterParams filter)
        {
            try
            {
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        filter.status = DataDAL.GetActiveFlag(filter.status);
                        var comTiers = db.MobileComTiers.Where(c =>
                            !string.IsNullOrEmpty(filter.tierName) ? c.TierName.ToLower().Contains(filter.tierName) : true
                            && !string.IsNullOrEmpty(filter.status) ? c.Active == filter.status : true
                        ).OrderBy(o => o.ComBahtFrom).ToList();
                        if (comTiers.Count > 0)
                        {
                            return Ok(comTiers);
                        }
                    }
                }

            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateTier(MobileComTier data)
        {
            try
            {
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DataDAL.GetDateTimeNow();
                        MobileComTier newTier = new MobileComTier();
                        if (!string.IsNullOrEmpty(data.TierName)) newTier.TierName = data.TierName;
                        if (!string.IsNullOrEmpty(data.TierColor)) newTier.TierColor = data.TierColor.Replace("#", "ff").ToUpper();
                        if (data.ComPercentage != null) newTier.ComPercentage = data.ComPercentage;
                        if (data.ComBahtFrom!= null) newTier.ComBahtFrom = data.ComBahtFrom;
                        if (data.ComBahtTo != null) newTier.ComBahtTo = data.ComBahtTo;
                        if (!string.IsNullOrEmpty(data.Active)) newTier.Active = data.Active;
                        newTier.Updated = now;
                        newTier.UpdatedBy = userAuth;
                        newTier.Created = now;
                        newTier.CreatedBy = userAuth;

                        db.MobileComTiers.Add(newTier);

                        db.SaveChanges();

                        return Ok(newTier);
                    }
                }
            }
            catch (Exception ex) { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateTierCommission(MobileComTier data)
        {
            try
            {
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DataDAL.GetDateTimeNow();
                        MobileComTier tier = db.MobileComTiers.FirstOrDefault(c => c.Id == data.Id); 
                        if (tier != null)
                        {
                            if (!string.IsNullOrEmpty(data.TierName)) tier.TierName = data.TierName;
                            if (!string.IsNullOrEmpty(data.TierName)) tier.TierColor = data.TierColor.Replace("#", "ff").ToUpper();
                            if (!string.IsNullOrEmpty(data.TierName)) tier.ComPercentage = data.ComPercentage;
                            if (!string.IsNullOrEmpty(data.TierName)) tier.ComBahtFrom = data.ComBahtFrom;
                            if (!string.IsNullOrEmpty(data.TierName)) tier.ComBahtTo = data.ComBahtTo;
                            if (!string.IsNullOrEmpty(data.Active)) tier.Active = data.Active;
                            tier.Updated = now;
                            tier.UpdatedBy = userAuth;

                            db.SaveChanges();

                            return Ok(tier);
                        }
                    }
                }
            }
            catch(Exception ex) { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteTier(MobileComTier data)
        {
            try
            {

                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        MobileComTier tier = db.MobileComTiers.FirstOrDefault(c => c.Id == data.Id);
                        if (tier != null)
                        {
                            db.MobileComTiers.Remove(tier);
                            db.SaveChanges();

                            return Ok("SUCCESS !!");
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetSetting(string code)
        {
            try
            {
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        return Ok(DataDAL.GetMobileSetting(code));
                    }
                }
            }
            catch (Exception ex) { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetDropdown(string code)
        {
            try
            {
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        return Ok(JsonConvert.DeserializeObject<List<string>>(DataDAL.GetMobileSetting(code)));
                    }
                }
            }
            catch (Exception ex) { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetTierList()
        {
            try
            {
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        List<string> result = new List<string>();
                        List<string> query = db.MobileComTiers.Where(c => c.Active == "Y").OrderBy(o => o.ComBahtFrom).Select(s => s.TierName).ToList();
                        if (query.Count > 0)
                        {
                            result.Add("All");
                            result.AddRange(query);
                            return Ok(result);
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetBranchList()
        {
            List<Branch> query = DataDAL.GetBranchList();
            if (query.Count > 0)
            {
                List<string> result = new List<string>();
                result.Add("All");
                result.AddRange(query.Select(s => s.Name).ToList());
                return Ok(result);
            }

            return Content(HttpStatusCode.NoContent, "No content.");
        }
    }
}
