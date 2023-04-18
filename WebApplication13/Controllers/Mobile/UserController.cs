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
                using (var db = new spasystemdbEntities())
                {
                    MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Username == data.Username && c.Password == data.Password);
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
                                mobileUserInfo.Token = token.Data;
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

                                return Ok(mobileUserInfo);
                            }
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> CheckUsername(ResponseData username)
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
