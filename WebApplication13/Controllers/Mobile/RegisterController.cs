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
using System.Globalization;

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
                    string profileSubPath = "UPLOAD\\MOBILE_USER_PROFILE_IMAGES\\";
                    var findPhoneNumber = db.MobileUsers.FirstOrDefault(c => c.PhoneNumber == data.PhoneNumber);
                    if (findPhoneNumber == null)
                    {
                        user.ProfilePath = $"{profileSubPath}{data.ProfilePath}";
                        user.Password = EncryptionDAL.EncryptString(data.Password);
                        user.FirstName = data.FirstName;
                        user.LastName = data.LastName;
                        user.IdCardNumber = data.IdCardNumber;
                        user.Province = data.Province;
                        user.PhoneNumber = data.PhoneNumber;
                        user.Occupation = data.Occupation;
                        user.BankAccount = data.BankAccount;
                        user.BankAccountNumber = data.BankAccountNumber;

                        if (!string.IsNullOrEmpty(data.Birthday)) user.Birthday = DateTime.ParseExact($"{data.Birthday}", "dd MMMM yyyy", CultureInfo.InvariantCulture);
                        else user.Birthday = null;
                        if (!string.IsNullOrEmpty(data.ProfilePath)) 
                        if (!string.IsNullOrEmpty(data.Nationality)) user.Nationality = data.Nationality;
                        else user.Nationality = null;
                        if (!string.IsNullOrEmpty(data.Address)) user.Address = data.Address;
                        else user.Address = null;
                        if (!string.IsNullOrEmpty(data.Email)) user.Email = data.Email;
                        else user.Email = null;
                        if (!string.IsNullOrEmpty(data.LineId)) user.LineId = data.LineId;
                        else user.LineId = null;
                        if (!string.IsNullOrEmpty(data.WhatsAppId)) user.WhatsAppId = data.WhatsAppId;
                        else user.WhatsAppId = null;
                        if (!string.IsNullOrEmpty(data.CompanyTypeOfUsage)) user.CompanyTypeOfUsage = data.CompanyTypeOfUsage;
                        if (data.CompanyTypeOfUsage == "Company")
                        {
                            if (!string.IsNullOrEmpty(data.CompanyName)) user.CompanyName = data.CompanyName;
                            if (!string.IsNullOrEmpty(data.CompanyTaxId)) user.CompanyTaxId = data.CompanyTaxId;
                        }

                        user.Active = "Y";
                        user.CreatedBy = DataDAL.GetUserNameByName(user.FirstName, user.LastName);
                        user.Created = now;
                        user.UpdatedBy = DataDAL.GetUserNameByName(user.FirstName, user.LastName);
                        user.Updated = now;

                        db.MobileUsers.Add(user);
                        db.SaveChanges();

                        //attachment file
                        if (!string.IsNullOrEmpty(data.IdCardPath))
                        {
                            string attachmentSubPath = "UPLOAD\\MOBILE_ATTACHMENT_IMAGES\\";
                            MobileFileAttachment att = new MobileFileAttachment();
                            att.FileSubPath = attachmentSubPath;
                            att.FileName = data.IdCardPath;
                            string[] exFileName = data.IdCardPath.Split('.');
                            if (exFileName.Length == 2) att.FileExtension = $"{exFileName[1]}";
                            att.MobileUserId = user.Id;
                            att.Type = 1;
                            att.Active = "Y";
                            att.CreatedBy = DataDAL.GetUserName(user.Id);
                            att.Created = now;
                            att.UpdatedBy = DataDAL.GetUserName(user.Id);
                            att.Updated = now;

                            db.MobileFileAttachments.Add(att);
                            db.SaveChanges();
                        }

                        string token = UserDAL.CreateLoginToken(user.Id);

                        response.Success = true;
                        response.Data = token;
                        return Ok(response);
                    }
                }
            }
            catch(Exception ex) 
            { 
                DataDAL.ErrorLog("register", ex.ToString()); 
            }

            return Content(HttpStatusCode.NotFound, "Not found.");
        }
    }
}
