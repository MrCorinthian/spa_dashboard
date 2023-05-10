using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication13.DAL;
using WebApplication13.Models;
using WebApplication13.Models.Mobile;

namespace WebApplication13.Controllers.Mobile
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CommissionController : ApiController
    {

        [HttpPost]
        public async Task<IHttpActionResult> GetCommission(ReportParams data)
        {
            try
            {
                string userAuth = UserDAL.UserLoginAuth();
                if (!string.IsNullOrEmpty(userAuth))
                {
                    DateTime now = DateTime.Now;
                    using (var db = new spasystemdbEntities())
                    {
                        List<MobileComTransaction> comTrans = new List<MobileComTransaction>();
                        if (!string.IsNullOrEmpty(data.Year) && !string.IsNullOrEmpty(data.Month))
                        {
                            DateTime date = DateTime.ParseExact($"{data.Month} {data.Year}", "MMMM yyyy", CultureInfo.InvariantCulture);
                            comTrans = db.MobileComTransactions.Where(c => c.Created.Year == date.Year && c.Created.Month == date.Month).ToList();
                        }
                        else if (!string.IsNullOrEmpty(data.Year))
                        {
                            DateTime date = DateTime.ParseExact($"{data.Year}", "yyyy", CultureInfo.InvariantCulture);
                            comTrans = db.MobileComTransactions.Where(c => c.Created.Year == date.Year).ToList();
                        }
                        else if (!string.IsNullOrEmpty(data.Month))
                        {
                            DateTime date = DateTime.ParseExact($"{data.Month} {now.Year}", "MMMM yyyy", CultureInfo.InvariantCulture);
                            comTrans = db.MobileComTransactions.Where(c => c.Created.Year == date.Year && c.Created.Month == date.Month).ToList();
                        }
                        else
                        {
                            comTrans = db.MobileComTransactions.ToList();
                        }

                        List<ReportBranch> reportBranchs = new List<ReportBranch>();
                        List<Branch> branches = db.Branches.Where(c => c.Id != 99).OrderBy(o => o.Id).ToList();
                        double totalBaht = comTrans.Sum(s => s.TotalBaht);
                        foreach (var item in branches)
                        {
                            ReportBranch report = new ReportBranch();
                            report.BranchId = item.Id;
                            report.BranchName = item.Name;


                            List<MobileComTransaction> branchComTrans = comTrans.Where(c => c.BranchId == item.Id).ToList();
                            double branchTotalBaht = branchComTrans.Sum(s => s.TotalBaht);
                            report.Commission = branchComTrans;
                            report.TotalBaht = branchTotalBaht;
                            report.TotalPercentage = Math.Round((branchTotalBaht / totalBaht) * 100, 2);
                            reportBranchs.Add(report);
                        }

                        return Ok(reportBranchs);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }
    }
}
