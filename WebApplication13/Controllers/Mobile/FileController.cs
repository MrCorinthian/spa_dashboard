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
using System.Globalization;

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
                var noms = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
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

            MobileUser user = UserDAL.GetUserByToken(token);
            if (user != null)
            {
                try
                {
                    using (var db = new spasystemdbEntities())
                    {
                        if (token != null && !string.IsNullOrEmpty(token))
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
                catch (Exception ex) { }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserAttachment(FileAttachmentParams data)
        {
            if (data.MobileUserId > 0)
            {
                using (var db = new spasystemdbEntities())
                {
                    string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                    MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == data.MobileUserId);
                    if (!string.IsNullOrEmpty(userAuth) && user != null)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(data.Filename))
                            {
                                DateTime now = DataDAL.GetDateTimeNow();
                                string subPath = $"UPLOAD\\MOBILE_ATTACHMENT_IMAGES\\";
                                string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}{subPath}";
                                string extension = data.Filename.Split('.').LastOrDefault();
                                string filePath = $"{webRootPath}{data.Filename}";

                                MobileFileAttachment att = new MobileFileAttachment();
                                att.FileSubPath = subPath;
                                att.FileName = data.Filename;
                                att.FileExtension = extension;
                                att.MobileUserId = user.Id;
                                att.Type = 1;
                                att.Active = "Y";
                                att.CreatedBy = userAuth;
                                att.Created = now;
                                att.UpdatedBy = userAuth;
                                att.Updated = now;

                                db.MobileFileAttachments.Add(att);
                                db.SaveChanges();

                                ResponseData response = new ResponseData();
                                response.Success = true;
                                response.Data = $"File/AttachmentImageWeb?filename={data.Filename}";

                                return Ok(response);
                            }
                            else
                            {
                                MobileFileAttachment fileAtt = db.MobileFileAttachments.Where(c => c.MobileUserId == user.Id && c.Type == 1 && c.Active == "Y").OrderByDescending(o => o.Created).FirstOrDefault();
                                if (fileAtt != null)
                                {
                                    ResponseData response = new ResponseData();
                                    response.Success = true;
                                    response.Data = $"File/AttachmentImageWeb?filename={fileAtt.FileName}";
                                    return Ok(response);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DataDAL.ErrorLog("UploadAttachment", ex.ToString(), DataDAL.GetUserName(user.Id));
                        }
                    }
                }
            }


            return BadRequest("No file uploaded");
        }

        [HttpGet]
        public async Task<IHttpActionResult> UserAttachmentImageWeb(int id)
        {
            var response = new HttpResponseMessage();
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";

            string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
            if (!string.IsNullOrEmpty(userAuth))
            {
                try
                {
                    using (var db = new spasystemdbEntities())
                    {
                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == id);
                        if (user != null)
                        {
                            MobileFileAttachment attImage = db.MobileFileAttachments.Where(c => c.MobileUserId == user.Id && c.Type == 1 && c.Active == "Y").OrderByDescending(o => o.Created).FirstOrDefault();
                            string imagePath = $"{webRootPath}{attImage.FileSubPath}{attImage.FileName}";

                            var stream = File.OpenRead(imagePath);
                            response.Content = new StreamContent(stream);
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentLength = stream.Length;

                            return ResponseMessage(response);

                        }
                    }
                }
                catch (Exception ex) { }
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IHttpActionResult> AttachmentImageWeb(string filename)
        {
            var response = new HttpResponseMessage();
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";

            string user = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
            if (!string.IsNullOrEmpty(user))
            {
                try
                {
                    using (var db = new spasystemdbEntities())
                    {
                        if (filename != null && !string.IsNullOrEmpty(filename))
                        {
                            string subPath = $"UPLOAD\\MOBILE_ATTACHMENT_IMAGES\\";
                            string imagePath = $"{webRootPath}{subPath}{filename}";
                            string extension = filename.Split('.').LastOrDefault();

                            var stream = File.OpenRead(imagePath);
                            response.Content = new StreamContent(stream);
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentLength = stream.Length;

                            return ResponseMessage(response);

                        }
                    }
                }
                catch { }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IHttpActionResult> ExportInvoice(ReportParams _params)
        {
            var response = new HttpResponseMessage();
            string webRootPath = $"{HostingEnvironment.ApplicationPhysicalPath}";

            string user = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
            if (!string.IsNullOrEmpty(user) && _params.MobileUserId > 0 && !string.IsNullOrEmpty(_params.Month) && !string.IsNullOrEmpty(_params.Year))
            {
                try
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DateTime.Now;
                        string subPath = $"UPLOAD\\MOBILE_EXPORT_INVOICE\\";
                        string filePath = "";
                        string filename = "";
                        int docCount = 0;
                        string docNo = "";
                        DateTime comDate = DateTime.ParseExact(_params.Month + " " + _params.Year, "MMMM yyyy", CultureInfo.InvariantCulture);
                        DateTime docDate = DateTime.ParseExact(_params.Month + " " + _params.Year, "MMMM yyyy", CultureInfo.InvariantCulture).AddMonths(1);
                        List<MobileExportInvoice> exportInvoices = db.MobileExportInvoices.Where(c => c.DocDate == docDate).ToList();
                        MobileExportInvoice exportInvoiceByUser = exportInvoices.FirstOrDefault(c => c.MobileUserId == _params.MobileUserId && c.Active == "Y");
                        if (exportInvoiceByUser != null)
                        {
                            filename = $"{exportInvoiceByUser.DocName}.pdf";
                        }
                        else
                        {
                            docCount = exportInvoices.Count + 1;
                            docNo = $"BN{docDate.Year}{docDate.Month.ToString("00")}{docDate.Day.ToString("00")}{docCount.ToString("000")}";
                            filename = $"INVOICE_{docNo}.pdf";
                            filePath = PdfDAL.GenerateExportInvoice(_params.MobileUserId, docNo, comDate, webRootPath, $"{webRootPath}{subPath}{filename}");

                            if (!string.IsNullOrEmpty(filePath))
                            {
                                MobileExportInvoice newExportInvoice = new MobileExportInvoice();
                                newExportInvoice.MobileUserId = _params.MobileUserId;
                                newExportInvoice.DocDate = comDate;
                                newExportInvoice.DocNo = docCount;
                                newExportInvoice.DocName = docNo;
                                newExportInvoice.Active = "Y";
                                newExportInvoice.Created = now;
                                newExportInvoice.CreatedBy = user;
                                db.MobileExportInvoices.Add(newExportInvoice);
                                db.SaveChanges();
                            }
                        }
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            var stream = File.OpenRead(filePath);
                            response.Content = new StreamContent(stream);
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentLength = stream.Length;
                            response.Content.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = filename
                            };

                            return ResponseMessage(response);

                        }
                    }
                }
                catch { }
            }

            return NotFound();
        }

        private string GenerateID()
        {
            return DataDAL.GetDateTimeNow().ToString("yyyyMMddHHmmssfffffff");
        }
    }
}
