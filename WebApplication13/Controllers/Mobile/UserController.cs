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
        #region mobile app
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
                    MobileUser user = db.MobileUsers.FirstOrDefault(c => c.PhoneNumber == data.Phone && c.Password == enPassword);
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
                            foreach (MobileUserLoginToken _t in allToken)
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

                                if (!string.IsNullOrEmpty(uInfo.PhoneNumber)) return Ok(uInfo);
                            }
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserInfo(RegisterData data)
        {
            try
            {
                MobileUser userAuth = UserDAL.GetUserByToken(data.Token);
                if (userAuth != null)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DataDAL.GetDateTimeNow();
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == userAuth.Id);
                        if (user != null)
                        {
                            if (!string.IsNullOrEmpty(data.FirstName)) user.FirstName = data.FirstName;
                            if (!string.IsNullOrEmpty(data.LastName)) user.LastName = data.LastName;
                            if (!string.IsNullOrEmpty(data.IdCardNumber)) user.IdCardNumber = data.IdCardNumber;
                            if (!string.IsNullOrEmpty(data.Nationality)) user.Nationality = data.Nationality;
                            if (!string.IsNullOrEmpty(data.Address)) user.Address = data.Address;
                            if (!string.IsNullOrEmpty(data.Province)) user.Province = data.Province;
                            if (!string.IsNullOrEmpty(data.Occupation)) user.Occupation = data.Occupation;
                            if (!string.IsNullOrEmpty(data.PhoneNumber)) user.PhoneNumber = data.PhoneNumber;
                            if (!string.IsNullOrEmpty(data.Email)) user.Email = data.Email;
                            if (!string.IsNullOrEmpty(data.LineId)) user.LineId = data.LineId;
                            if (!string.IsNullOrEmpty(data.WhatsAppId)) user.WhatsAppId = data.WhatsAppId;
                            if (!string.IsNullOrEmpty(data.CompanyName)) user.CompanyName = data.CompanyName;
                            if (!string.IsNullOrEmpty(data.CompanyTexId)) user.CompanyTexId = data.CompanyTexId;
                            if (!string.IsNullOrEmpty(data.BankAccount)) user.BankAccount = data.BankAccount;
                            if (!string.IsNullOrEmpty(data.BankAccountNumber)) user.BankAccountNumber = data.BankAccountNumber;
                            if (!string.IsNullOrEmpty(data.ProfilePath)) user.ProfilePath = $"UPLOAD\\MOBILE_USER_PROFILE_IMAGES\\{data.ProfilePath}";
                            //if (!string.IsNullOrEmpty(data.Active)) user.Active = data.Active;
                            user.Updated = now;
                            user.UpdatedBy = userAuth.Username;

                            db.SaveChanges();

                            user.ProfilePath = $"File/ProfileImageWebUpload?fileName={data.ProfilePath}";
                            user.Password = null;
                            return Ok(user);
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }


        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(ChanegePasswordParams data)
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    ResponseData response = new ResponseData();
                    MobileUser user = UserDAL.GetUserByToken(data.Token);
                    string enPassword = EncryptionDAL.EncryptString(data.Password);
                    MobileUser userPassword = db.MobileUsers.FirstOrDefault(c => c.Id == user.Id && c.Password == enPassword);
                    if (user != null && userPassword != null && data.NewPassword == data.ConfirmNewPassword)
                    {
                        userPassword.Password = EncryptionDAL.EncryptString(data.NewPassword);
                        db.SaveChanges();

                        string token = UserDAL.CreateLoginToken(userPassword.Id);

                        response.Success = true;
                        response.Data = token;
                        return Ok(response);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        #endregion

        [HttpPost]
        public async Task<IHttpActionResult> WebLogout()
        {
            try
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
                return Ok();
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetMoblieUserIndex(FilterParams filter)
        {
            try
            {
                var noms = System.Runtime.Caching.MemoryCache.Default["names"];
                if (noms != null)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        filter.status = DataDAL.GetActiveFlag(filter.status);
                        List<int> indexTable = new List<int>();
                        decimal tableMaxRow = int.Parse(DataDAL.GetMobileSetting("TABLE_MAX_ROW"));
                        decimal rowCount = db.MobileUsers.Where(c =>
                        !string.IsNullOrEmpty(filter.firstName) ? c.FirstName.ToLower().Contains(filter.firstName) : true
                        && !string.IsNullOrEmpty(filter.lastName) ? c.LastName.ToLower().Contains(filter.lastName) : true
                        && !string.IsNullOrEmpty(filter.phone) ? c.PhoneNumber.ToLower().Contains(filter.phone) : true
                        && !string.IsNullOrEmpty(filter.status) ? c.Active == filter.status : true
                        ).Count();
                        decimal rowPerPage = rowCount / tableMaxRow;
                        if (rowPerPage > 0)
                        {
                            for(int i = 0; i < rowPerPage; i++)
                            {
                                indexTable.Add(i+1);
                            }
                            return Ok(indexTable);
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetMoblieUser(FilterParams filter)
        {
            try
            {
                var noms = System.Runtime.Caching.MemoryCache.Default["names"];
                if (noms != null)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        filter.page = filter.page > 0? filter.page - 1 : 0;
                        filter.status = DataDAL.GetActiveFlag(filter.status);
                        List<MobileUser> mUsers = new List<MobileUser>();
                        int tableMaxRow = int.Parse(DataDAL.GetMobileSetting("TABLE_MAX_ROW"));
                        var users = db.MobileUsers.Where(c => 
                        !string.IsNullOrEmpty(filter.firstName) ? c.FirstName.ToLower().Contains(filter.firstName) : true
                        && !string.IsNullOrEmpty(filter.lastName) ? c.LastName.ToLower().Contains(filter.lastName) : true
                        && !string.IsNullOrEmpty(filter.phone) ? c.PhoneNumber.ToLower().Contains(filter.phone) : true
                        && !string.IsNullOrEmpty(filter.status) ? c.Active == filter.status : true
                        ).OrderBy(o => o.Id).Skip(tableMaxRow * filter.page).Take(tableMaxRow).ToList();
                        foreach (MobileUser user in users)
                        {
                            if (!string.IsNullOrEmpty(user.ProfilePath))
                            {
                                List<string> file = new List<string>();
                                if(!string.IsNullOrEmpty(user.ProfilePath)) file = user.ProfilePath.Split(new[] { "\\" }, StringSplitOptions.None).ToList();
                                if(file.Count > 0) user.ProfilePath = $"File/ProfileImageWebUpload?fileName={file.LastOrDefault()}";
                            }
                            mUsers.Add(user);
                        }

                        var result = (from s in mUsers
                                      select new { 
                                          Id = s.Id,
                                          Username = s.Username,
                                          TitleName = s.TitleName,
                                          FirstName = s.FirstName,
                                          LastName = s.LastName,
                                          IdCardNumber = s.IdCardNumber,
                                          Nationality = s.Nationality,
                                          Birthday = s.Birthday,
                                          Address = s.Address,
                                          Province = s.Province,
                                          Occupation = s.Occupation,
                                          PhoneNumber = s.PhoneNumber,
                                          Email = s.Email,
                                          LineId = s.LineId,
                                          WhatsAppId = s.WhatsAppId,
                                          CompanyName = s.CompanyName,
                                          CompanyTexId = s.CompanyTexId,
                                          BankAccount = s.BankAccount,
                                          BankAccountNumber = s.BankAccountNumber,
                                          ProfilePath = s.ProfilePath,
                                          Created = s.Created,
                                          Updated = s.Updated,
                                          Active = s.Active,
                                      });
                        return Ok(result);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetMoblieUserOverview(int page)
        {
            try
            {
                var noms = System.Runtime.Caching.MemoryCache.Default["names"];
                if (noms != null)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        page--;
                        List<MobileUserInfo> result = new List<MobileUserInfo>();
                        int tableMaxRow = int.Parse(DataDAL.GetMobileSetting("TABLE_MAX_ROW"));
                        var users = db.MobileUsers.OrderBy(o => o.Id).Skip(tableMaxRow*page).Take(tableMaxRow).ToList();
                        foreach(MobileUser user in users)
                        {
                            MobileUserInfo uInfo = UserDAL.GetMoblieUserInfo(user.Id);
                            result.Add(uInfo);
                        }
                        return Ok(result);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateUser(MobileUser data)
        {
            try
            {
                string userAuth = UserDAL.UserLoginAuth();
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DataDAL.GetDateTimeNow();
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Username == data.Username);
                        if (user == null)
                        {
                            MobileUser newUser = new MobileUser();
                            if (!string.IsNullOrEmpty(data.Username)) newUser.Username = data.Username;
                            if (!string.IsNullOrEmpty(data.Password)) newUser.Password = EncryptionDAL.EncryptString(data.Password);
                            if (!string.IsNullOrEmpty(data.FirstName)) newUser.FirstName = data.FirstName;
                            if (!string.IsNullOrEmpty(data.LastName)) newUser.LastName = data.LastName;
                            if (!string.IsNullOrEmpty(data.IdCardNumber)) newUser.IdCardNumber = data.IdCardNumber;
                            if (!string.IsNullOrEmpty(data.Nationality)) newUser.Nationality = data.Nationality;
                            if (!string.IsNullOrEmpty(data.Address)) newUser.Address = data.Address;
                            if (!string.IsNullOrEmpty(data.Province)) newUser.Province = data.Province;
                            if (!string.IsNullOrEmpty(data.Occupation)) newUser.Occupation = data.Occupation;
                            if (!string.IsNullOrEmpty(data.PhoneNumber)) newUser.PhoneNumber = data.PhoneNumber;
                            if (!string.IsNullOrEmpty(data.Email)) newUser.Email = data.Email;
                            if (!string.IsNullOrEmpty(data.LineId)) newUser.LineId = data.LineId;
                            if (!string.IsNullOrEmpty(data.WhatsAppId)) newUser.WhatsAppId = data.WhatsAppId;
                            if (!string.IsNullOrEmpty(data.CompanyName)) newUser.CompanyName = data.CompanyName;
                            if (!string.IsNullOrEmpty(data.CompanyTexId)) newUser.CompanyTexId = data.CompanyTexId;
                            if (!string.IsNullOrEmpty(data.BankAccount)) newUser.BankAccount = data.BankAccount;
                            if (!string.IsNullOrEmpty(data.BankAccountNumber)) newUser.BankAccountNumber = data.BankAccountNumber;
                            if (!string.IsNullOrEmpty(data.ProfilePath)) newUser.ProfilePath = $"UPLOAD\\MOBILE_USER_PROFILE_IMAGES\\{data.ProfilePath}";
                            if (!string.IsNullOrEmpty(data.Active)) newUser.Active = data.Active;
                            newUser.Created = now;
                            newUser.CreatedBy = userAuth;
                            newUser.Updated = now;
                            newUser.UpdatedBy = userAuth;

                            db.MobileUsers.Add(newUser);

                            db.SaveChanges();

                            newUser.ProfilePath = $"File/ProfileImageWebUpload?fileName={data.ProfilePath}";

                            return Ok(newUser);
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserInformation(MobileUser data)
        {
            try
            {
                string userAuth = UserDAL.UserLoginAuth();
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DataDAL.GetDateTimeNow();
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == data.Id);
                        if (user != null)
                        {
                            if (!string.IsNullOrEmpty(data.FirstName)) user.FirstName = data.FirstName;
                            if (!string.IsNullOrEmpty(data.LastName)) user.LastName = data.LastName;
                            if (!string.IsNullOrEmpty(data.IdCardNumber)) user.IdCardNumber = data.IdCardNumber;
                            if (!string.IsNullOrEmpty(data.Nationality)) user.Nationality = data.Nationality;
                            if (!string.IsNullOrEmpty(data.Address)) user.Address = data.Address;
                            if (!string.IsNullOrEmpty(data.Province)) user.Province = data.Province;
                            if (!string.IsNullOrEmpty(data.Occupation)) user.Occupation = data.Occupation;
                            if (!string.IsNullOrEmpty(data.PhoneNumber)) user.PhoneNumber = data.PhoneNumber;
                            if (!string.IsNullOrEmpty(data.Email)) user.Email = data.Email;
                            if (!string.IsNullOrEmpty(data.LineId)) user.LineId = data.LineId;
                            if (!string.IsNullOrEmpty(data.WhatsAppId)) user.WhatsAppId = data.WhatsAppId;
                            if (!string.IsNullOrEmpty(data.CompanyName)) user.CompanyName = data.CompanyName;
                            if (!string.IsNullOrEmpty(data.CompanyTexId)) user.CompanyTexId = data.CompanyTexId;
                            if (!string.IsNullOrEmpty(data.BankAccount)) user.BankAccount = data.BankAccount;
                            if (!string.IsNullOrEmpty(data.BankAccountNumber)) user.BankAccountNumber = data.BankAccountNumber;
                            if (!string.IsNullOrEmpty(data.ProfilePath)) user.ProfilePath = $"UPLOAD\\MOBILE_USER_PROFILE_IMAGES\\{data.ProfilePath}";
                            if (!string.IsNullOrEmpty(data.Active)) user.Active = data.Active;
                            user.Updated = now;
                            user.UpdatedBy = userAuth;

                            db.SaveChanges();

                            user.ProfilePath = $"File/ProfileImageWebUpload?fileName={data.ProfilePath}";

                            return Ok(user);
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteUser(MobileUser data)
        {
            try
            {

                string userAuth = UserDAL.UserLoginAuth();
                if (!string.IsNullOrEmpty(userAuth))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == data.Id);
                        if (user != null)
                        {
                            db.MobileUsers.Remove(user);
                            db.SaveChanges();

                            return Ok("SUCCESS !!");
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> Username(ResponseData phone)
        {
            ResponseData response = new ResponseData();
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    if (phone != null && !string.IsNullOrEmpty(phone.Data))
                    {
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.PhoneNumber == phone.Data);
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
