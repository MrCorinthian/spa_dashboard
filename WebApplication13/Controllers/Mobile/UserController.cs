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

namespace WebApplication13.Controllers.Mobile
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Login(UserLogin data)
        {
            ResponseData response = new ResponseData();
            string token = "";
            try
            {
                string enPassword = EncryptionDAL.EncryptString(data.Password);
                using (var db = new spasystemdbEntities())
                {
                    MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Username == data.Username && c.Password == enPassword);
                    if (user != null)
                    {
                        token = UserDAL.CreateLoginToken(user.Id);

                        response.Success = true;
                        response.Data = token;
                        return Ok(response);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NotFound, "Not found.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> Logout(ResponseData token)
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
                            List<MobileUserLoginToken> allToken = db.MobileUserLoginTokens.Where(c => c.MobileUserId == loginToken.MobileUserId && c.Active == "Y").ToList();
                            foreach(MobileUserLoginToken _t in allToken)
                            {
                                _t.Active = "N";
                            }
                            db.SaveChanges();

                            return Content(HttpStatusCode.OK, "Ok.");
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetMoblieUserInfo(ResponseData token)
        {
            MobileUserInfo mobileUserInfo = new MobileUserInfo();
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    if (token != null && !string.IsNullOrEmpty(token.Data))
                    {
                        MobileUserLoginToken loginToken = db.MobileUserLoginTokens.FirstOrDefault(c => c.Token == token.Data);
                        if (loginToken != null)
                        {
                            var user = db.MobileUsers.FirstOrDefault(c => c.Id == loginToken.MobileUserId);
                            if (user != null)
                            {
                                MobileUserInfo uInfo = UserDAL.GetMoblieUserInfo(user.Id);

                                if (!string.IsNullOrEmpty(uInfo.Username)) return Ok(mobileUserInfo);
                            }
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetMoblieUser(ReportParams parms)
        {
            try
            {
                var noms = System.Runtime.Caching.MemoryCache.Default["names"];
                if (noms != null)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        List<MobileUserInfo> result = new List<MobileUserInfo>();
                        var users = db.MobileUsers.ToList();
                        foreach(MobileUser user in users)
                        {
                            MobileUserInfo uInfo = UserDAL.GetMoblieUserInfo(user.Id);
                            if (!string.IsNullOrEmpty(uInfo.Username))
                            {
                                result.Add(uInfo);
                            }
                        }
                        return Ok(result);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> Username(ResponseData username)
        {
            ResponseData response = new ResponseData();
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    if (username != null && !string.IsNullOrEmpty(username.Data))
                    {
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Username == username.Data);
                        if (user != null)
                        {
                            response.Success = true;
                            response.Data = user.Username;
                        }
                        else
                        {
                            response.Success = false;
                            response.Data = "";
                        }

                        return Ok(response);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }
    }
}
