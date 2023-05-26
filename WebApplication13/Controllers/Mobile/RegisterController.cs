using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using WebApplication13.Models;
using WebApplication13.Models.Mobile;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using WebApplication13.DAL;

namespace WebApplication13.Controllers.Mobile
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RegisterController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterData data)
        {
            ResponseData response = new ResponseData();
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    DateTime now = DataDAL.GetDateTimeNow();
                    MobileUser user = new MobileUser();
                    string subPath = "UPLOAD\\MOBILE_USER_PROFILE_IMAGES\\";
                    var findUsername = db.MobileUsers.FirstOrDefault(c => c.Username.ToUpper() == user.Username.ToUpper());
                    if (findUsername == null)
                    {
                        user.Username = data.Username;
                        user.Password = EncryptionDAL.EncryptString(data.Password);
                        //user.TitleName = data.TitleName;
                        user.FirstName = data.FirstName;
                        user.LastName = data.LastName;
                        user.IdCardNumber = data.IdCardNumber;
                        user.Province = data.Province;
                        user.PhoneNumber = data.PhoneNumber;
                        user.Occupation = data.Occupation;
                        user.BankAccount = data.BankAccount;
                        user.BankAccountNumber = data.BankAccountNumber;

                        if (!string.IsNullOrEmpty(data.ProfilePath)) user.ProfilePath = $"{subPath}{data.ProfilePath}";
                        if (!string.IsNullOrEmpty(data.Nationality)) user.Nationality = data.Nationality;
                        if (!string.IsNullOrEmpty(data.Address)) user.Address = data.Address;
                        if (!string.IsNullOrEmpty(data.Email)) user.Email = data.Email;
                        if (!string.IsNullOrEmpty(data.LineId)) user.LineId = data.LineId;
                        if (!string.IsNullOrEmpty(data.WhatsAppId)) user.WhatsAppId = data.WhatsAppId;
                        if (!string.IsNullOrEmpty(data.CompanyName)) user.CompanyName = data.CompanyName;
                        if (!string.IsNullOrEmpty(data.CompanyTexId)) user.CompanyTexId = data.CompanyTexId;

                        user.Active = "Y";
                        user.CreatedBy = "system";
                        user.Created = now;
                        user.UpdatedBy = "system";
                        user.Updated = now;

                        db.MobileUsers.Add(user);
                        db.SaveChanges();

                        string token = UserDAL.CreateLoginToken(user.Id);

                        response.Success = true;
                        response.Data = token;
                        return Ok(response);
                    }
                }
            }
            catch(Exception ex) 
            { 

            }

            return Content(HttpStatusCode.NotFound, "Not found.");
        }
    }
}
