﻿using System;
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
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DateTime.Now;
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

        [HttpPost]
        public async Task<IHttpActionResult> GetUserCommissionIndex(FilterParams filter)
        {
            try
            {
                string userAuth = UserDAL.UserLoginAuth();
                if (!string.IsNullOrEmpty(userAuth) && !string.IsNullOrEmpty(filter.year) && !string.IsNullOrEmpty(filter.month))
                {
                    filter.status = DataDAL.GetActiveFlag(filter.status);
                    DateTime monthYear = DateTime.ParseExact($"{filter.year} {filter.month} 01", "yyyy MMMM dd", CultureInfo.InvariantCulture);
                    DateTime now = DateTime.Now;
                    using (var db = new spasystemdbEntities())
                    {
                        PaymentDataIndex dataIndex = new PaymentDataIndex();
                        dataIndex.Index = filter.page;
                        int tableMaxRow = int.Parse(DataDAL.GetMobileSetting("TABLE_MAX_ROW"));
                        List<int> userTransCompleted = db.MobileComPayments.Where(c => c.PaymentMonth.Year == monthYear.Year && c.PaymentMonth.Month == monthYear.Month).Select(s => s.MobileUserId).ToList();
                        decimal rowCount = db.MobileUsers.Where(c => filter.status == "Y" ? userTransCompleted.Contains(c.Id) : (filter.status == "N" ? !userTransCompleted.Contains(c.Id) : true)).Count();
                        decimal rowPerPage = rowCount / tableMaxRow;
                        if (rowPerPage > 0)
                        {
                            for (int i = 0; i < rowPerPage; i++)
                            {
                                dataIndex.Indices.Add(i + 1);
                            }
                        }

                        List<MobileUser> users = db.MobileUsers.Where(c => filter.status == "Y" ? userTransCompleted.Contains(c.Id) : (filter.status == "N" ? !userTransCompleted.Contains(c.Id) : true)).OrderBy(o => o.Id).Skip(tableMaxRow * (filter.page-1)).Take(tableMaxRow).ToList();
                        foreach (MobileUser user in users)
                        {
                            PaymentData pData = new PaymentData();
                            pData.Id = user.Id;
                            pData.FirstName = user.FirstName;
                            pData.LastName = user.LastName;
                            pData.ProfilePath = DataDAL.GetProfilePath(user.ProfilePath);
                            pData.PhoneNumber = user.PhoneNumber;
                            pData.BankAccount = user.BankAccount;
                            pData.BankAccountNumber = user.BankAccountNumber;
                            MobileComPayment paymentStatus = db.MobileComPayments.FirstOrDefault(c => c.MobileUserId == user.Id && c.PaymentMonth.Year == monthYear.Year && c.PaymentMonth.Month == monthYear.Month);
                            pData.PaymentStatus = paymentStatus != null ? "Y" : "N";
                            pData.Payment = monthYear.Year <= now.Year && monthYear.Month < now.Month ? true : false;

                            List<MobileComTransaction> comTrans = db.MobileComTransactions.Where(c => 
                                c.MobileUserId == user.Id
                                && c.Created.Year == monthYear.Year
                                && c.Created.Month == monthYear.Month
                                ).ToList();

                            double sumCom = comTrans.Sum(s => s.TotalBaht);
                            MobileComTier tier = db.MobileComTiers.FirstOrDefault(c => sumCom >= c.ComBahtFrom && c.ComBahtTo != null ? sumCom <= c.ComBahtTo : true);
                            pData.ComTrans = comTrans;
                            pData.Commission = sumCom;
                            pData.Tier = tier != null ? tier.TierName : "";
                            dataIndex.Data.Add(pData);
                        }

                        return Ok(dataIndex);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> Payment(FilterParams filter)
        {
            try 
            {
                string userAuth = UserDAL.UserLoginAuth();
                if (!string.IsNullOrEmpty(userAuth) && !string.IsNullOrEmpty(filter.year) && !string.IsNullOrEmpty(filter.month))
                {
                    DateTime monthYear = DateTime.ParseExact($"{filter.year} {filter.month} 01", "yyyy MMMM dd", CultureInfo.InvariantCulture);
                    DateTime now = DateTime.Now;
                    if (monthYear.Year == now.Year && monthYear.Month < now.Month)
                    {
                        using (var db = new spasystemdbEntities())
                        {
                            MobileComPayment payment = new MobileComPayment();
                            payment.MobileUserId = filter.userId;
                            payment.PaymentMonth = monthYear;
                            payment.Created = now;
                            payment.CreatedBy = userAuth;

                            db.MobileComPayments.Add(payment);
                            db.SaveChanges();

                            return Ok(payment);
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }
    }
}
