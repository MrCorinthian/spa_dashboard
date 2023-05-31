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
using WebApplication13.DAL;
using System.Globalization;

namespace WebApplication13.Controllers.Mobile
{
    public class ReportController : ApiController
    {

        [HttpPost]
        public async Task<IHttpActionResult> GetReport(ReportParams _params)
        {
            try
            {
                MobileUser user = UserDAL.GetUserByToken(_params.Token);
                if (user != null && !string.IsNullOrEmpty(_params.Month) && !string.IsNullOrEmpty(_params.Year))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime dateFrom = DateTime.ParseExact(_params.Month + " " + _params.Year, "MMMM yyyy", CultureInfo.InvariantCulture);
                        DateTime dateTo = dateFrom.AddMonths(1);
                        List<MobileComTransaction> comTtrans = db.MobileComTransactions.Where(c => c.MobileUserId == user.Id && c.Created >= dateFrom && c.Created < dateTo).ToList();
                        if(comTtrans.Count > 0)
                        {
                            List<ReportTransaction> reportTrans = new List<ReportTransaction>();
                            foreach (MobileComTransaction comTtran in comTtrans)
                            {
                                MobileComPayment payments = db.MobileComPayments.FirstOrDefault(c => c.PaymentMonth >= comTtran.Created);
                                ReportTransaction report = new ReportTransaction();
                                report.TotalBaht = comTtran.TotalBaht;
                                report.Created = comTtran.Created;
                                report.PaymentStatus = payments != null ? "Y" : "N";
                                reportTrans.Add(report);
                            }
                            return Ok(reportTrans);
                        }
                    }
                }
                
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }
    }
}
