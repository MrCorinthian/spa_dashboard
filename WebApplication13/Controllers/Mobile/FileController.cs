using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Hosting;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using WebApplication13.Models;
using WebApplication13.Models.Mobile;
using System.Web.Http.Cors;
using WebApplication13.DAL;
using System.IO;
using System.Net.Http.Headers;

namespace WebApplication13.Controllers.Mobile
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FileController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> UploadImage()
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    if (!Request.Content.IsMimeMultipartContent())
                    {
                        return BadRequest("Unsupported media type");
                    }

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);

                    foreach (var file in provider.Contents)
                    {
                        if (!string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                        {
                            string subPath = "UPLOAD\\MOBILE_USER_PROFILE_IMAGES\\";
                            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}{subPath}";
                            string originalFileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                            string extension = originalFileName.Split('.').LastOrDefault();
                            string fileName = GenerateID();
                            string filePath = $"{webRootPath}{fileName}.{extension}";

                            System.IO.Directory.CreateDirectory(webRootPath);
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await file.CopyToAsync(stream);
                                string responsePath = $"{fileName}.{extension}";

                                ResponseData response = new ResponseData();
                                response.Success = true;
                                response.Data = responsePath;

                                return Ok(response);
                            }
                        }
                    }
                }
            }
            catch { }

            return BadRequest("No file uploaded");
        }

        [HttpGet]
        public async Task<IHttpActionResult> ProfileImage(string token)
        {
            var response = new HttpResponseMessage();
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";

            try
            {
                using (var db = new spasystemdbEntities())
                {
                    if (token != null && !string.IsNullOrEmpty(token))
                    {
                        MobileUserLoginToken loginToken = db.MobileUserLoginTokens.FirstOrDefault(c => c.Token == token);
                        if (loginToken != null)
                        {
                            MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == loginToken.MobileUserId && c.Active == "Y");
                            if(user != null)
                            {
                                string imagePath = $"{webRootPath}{user.ProfilePath}";
                                string extension = user.ProfilePath.Split('.').LastOrDefault();

                                var stream = File.OpenRead(imagePath);
                                response.Content = new StreamContent(stream);
                                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                response.Content.Headers.ContentLength = stream.Length;

                                return ResponseMessage(response);
                            }
                        }
                    }
                }
            }
            catch(Exception ex) {  }

            return NotFound();
        }

        [HttpGet]
        public async Task<IHttpActionResult> ProfileImageWeb(int id)
        {
            var response = new HttpResponseMessage();
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";
            try
            {
                var noms = System.Runtime.Caching.MemoryCache.Default["names"];
                if (noms != null)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == id && c.Active == "Y");
                        if (user != null && !string.IsNullOrEmpty(user.ProfilePath))
                        {
                            string imagePath = $"{webRootPath}{user.ProfilePath}";
                            string extension = user.ProfilePath.Split('.').LastOrDefault();

                            var stream = File.OpenRead(imagePath);
                            response.Content = new StreamContent(stream);
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentLength = stream.Length;

                            return ResponseMessage(response);
                        }
                    
                    }
                }
            }
            catch { }

            return NotFound();
        }

        [HttpGet]
        public async Task<IHttpActionResult> ProfileImageWebUpload(string fileName)
        {
            var response = new HttpResponseMessage();
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";
            try
            {
                var noms = UserDAL.UserLoginAuth();
                if (noms != null)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        string subPath = "UPLOAD\\MOBILE_USER_PROFILE_IMAGES\\";
                        string imagePath = $"{webRootPath}{subPath}{fileName}";
                       
                        var stream = File.OpenRead(imagePath);
                        response.Content = new StreamContent(stream);
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        response.Content.Headers.ContentLength = stream.Length;

                        return ResponseMessage(response);
                    }
                }
            }
            catch { }

            return NotFound();
        }


        [HttpPost]
        public async Task<IHttpActionResult> UploadAttachment()
        {
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    DateTime now = DataDAL.GetDateTimeNow();
                    if (!Request.Content.IsMimeMultipartContent())
                    {
                        return BadRequest("Unsupported media type");
                    }

                    var provider = new MultipartMemoryStreamProvider();
                    await Request.Content.ReadAsMultipartAsync(provider);

                    foreach (var file in provider.Contents)
                    {
                        if (!string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                        {
                            string subPath = $"UPLOAD\\MOBILE_ATTACHMENT_IMAGES\\";
                            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}{subPath}";
                            string originalFileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                            string extension = originalFileName.Split('.').LastOrDefault();
                            string fileName = GenerateID();
                            string filePath = $"{webRootPath}{fileName}.{extension}";

                            System.IO.Directory.CreateDirectory(webRootPath);
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await file.CopyToAsync(stream);
                                string responsePath = $"{fileName}.{extension}";

                                ResponseData response = new ResponseData();
                                response.Success = true;
                                response.Data = responsePath;

                                return Ok(response);
                            }
                        }
                    }
                }
            }
            catch { }

            return BadRequest("No file uploaded");
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadUserAttachment(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                MobileUser user = UserDAL.GetUserByToken(token);
                if(user != null)
                {
                    try
                    {
                        using (var db = new spasystemdbEntities())
                        {
                            DateTime now = DataDAL.GetDateTimeNow();
                            if (!Request.Content.IsMimeMultipartContent())
                            {
                                return BadRequest("Unsupported media type");
                            }

                            var provider = new MultipartMemoryStreamProvider();
                            await Request.Content.ReadAsMultipartAsync(provider);

                            foreach (var file in provider.Contents)
                            {
                                if (!string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                                {
                                    string subPath = $"UPLOAD\\MOBILE_ATTACHMENT_IMAGES\\";
                                    string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}{subPath}";
                                    string originalFileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                                    string extension = originalFileName.Split('.').LastOrDefault();
                                    string fileName = GenerateID();
                                    string filePath = $"{webRootPath}{fileName}.{extension}";

                                    System.IO.Directory.CreateDirectory(webRootPath);
                                    using (var stream = System.IO.File.Create(filePath))
                                    {
                                        await file.CopyToAsync(stream);
                                        string responsePath = $"{fileName}.{extension}";

                                        //attachment file
                                        if (!string.IsNullOrEmpty(token))
                                        {
                                            string attachmentSubPath = subPath;
                                            MobileFileAttachment att = new MobileFileAttachment();
                                            att.FileSubPath = attachmentSubPath;
                                            att.FileName = responsePath;
                                            att.FileExtension = extension;
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

                                        ResponseData response = new ResponseData();
                                        response.Success = true;
                                        response.Data = responsePath;

                                        return Ok(response);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DataDAL.ErrorLog("UploadAttachment", ex.ToString(), DataDAL.GetUserName(user.Id));
                    }
                }
            }
                

            return BadRequest("No file uploaded");
        }

        [HttpGet]
        public async Task<IHttpActionResult> AttachmentImage(string token)
        {
            var response = new HttpResponseMessage();
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";

            try
            {
                using (var db = new spasystemdbEntities())
                {
                    if (token != null && !string.IsNullOrEmpty(token))
                    {
                        MobileUser user = UserDAL.GetUserByToken(token);
                        if (user != null)
                        {
                            MobileFileAttachment attImage = db.MobileFileAttachments.Where(c => c.MobileUserId == user.Id && c.Type == 1).OrderByDescending(o => o.Created).FirstOrDefault();
                            if (attImage != null)
                            {
                                string imagePath = $"{webRootPath}{attImage.FileSubPath}{attImage.FileName}";
                                string extension = attImage.FileExtension;

                                var stream = File.OpenRead(imagePath);
                                response.Content = new StreamContent(stream);
                                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                response.Content.Headers.ContentLength = stream.Length;

                                return ResponseMessage(response);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }

            return NotFound();
        }

        private string GenerateID()
        {
            return DataDAL.GetDateTimeNow().ToString("yyyyMMddHHmmssfffffff");
        }
    }
}
