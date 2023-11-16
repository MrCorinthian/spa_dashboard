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
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth) || true)
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DataDAL.GetDateTimeNow();
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
                        List<int> branchList = DataDAL.GetBranchList().Select(s => s.Id).ToList();
                        List<Branch> branches = db.Branches.Where(c => branchList.Contains(c.Id)).OrderBy(o => o.Id).ToList();
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
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth) && !string.IsNullOrEmpty(filter.year) && !string.IsNullOrEmpty(filter.month))
                {
                    filter.status = DataDAL.GetActiveFlag(filter.status);
                    DateTime monthYear = DateTime.ParseExact($"{filter.year} {filter.month} 01", "yyyy MMMM dd", CultureInfo.InvariantCulture);
                    DateTime now = DataDAL.GetDateTimeNow();
                    using (var db = new spasystemdbEntities())
                    {
                        PaymentDataIndex dataIndex = new PaymentDataIndex();
                        dataIndex.Index = filter.page;
                        int tableMaxRow = int.Parse(DataDAL.GetMobileSetting("TABLE_MAX_ROW"));
                        dataIndex.RowPerPage = tableMaxRow;
                        List<int> userTransCompleted = db.MobileComPayments
                            .Where(c => c.PaymentMonth.Year == monthYear.Year && c.PaymentMonth.Month == monthYear.Month)
                            .Select(s => s.MobileUserId).ToList();
                        if (userTransCompleted.Count > 0)
                        {
                            decimal rowCount = db.MobileUsers.Where(c => filter.status == "Y" ? userTransCompleted.Contains(c.Id) : (filter.status == "N" ? !userTransCompleted.Contains(c.Id) : true)).Count();
                            decimal rowPerPage = rowCount / tableMaxRow;
                            if (rowPerPage > 0)
                            {
                                for (int i = 0; i < rowPerPage; i++)
                                {
                                    dataIndex.Indices.Add(i + 1);
                                }
                            }
                        }

                        int filterCompanyTypeOfUsage = 0;
                        if (string.IsNullOrEmpty(filter.companyTypeOfUsage))
                        {
                            MobileDropdown CompanyTypeOfUsage = db.MobileDropdowns.FirstOrDefault(c => c.GroupName == "COM_TYPE_OF_USAGE" && c.Value == filter.companyTypeOfUsage);
                            if (CompanyTypeOfUsage != null) filterCompanyTypeOfUsage = CompanyTypeOfUsage.Id;
                        }

                        List<MobileUser> users = db.MobileUsers.Where(c => 
                                                    filter.status == "Y" ? userTransCompleted.Contains(c.Id) : (filter.status == "N" ? !userTransCompleted.Contains(c.Id) : true)
                                                    && (filter.companyTypeOfUsage != null ? filterCompanyTypeOfUsage == c.CompanyTypeOfUsage || filter.companyTypeOfUsage == "All" : true)
                                                    ).OrderBy(o => o.Id).Skip(tableMaxRow * (filter.page-1)).Take(tableMaxRow).ToList();
                        foreach (MobileUser user in users)
                        {
                            List<MobileComTransaction> comTrans = db.MobileComTransactions.Where(c =>
                                c.MobileUserId == user.Id
                                && c.Created.Year == monthYear.Year
                                && c.Created.Month == monthYear.Month
                                ).ToList();
                            if (comTrans.Count > 0)
                            {
                                PaymentData pData = new PaymentData();
                                pData.Id = user.Id;
                                pData.FirstName = user.FirstName;
                                pData.LastName = user.LastName;
                                pData.ProfilePath = DataDAL.GetProfilePath(user.ProfilePath);
                                pData.PhoneNumber = user.PhoneNumber;
                                if (filter.companyTypeOfUsage == "Company")
                                {
                                    pData.CompanyName = user.CompanyName;
                                    pData.CompanyTaxId = user.CompanyTaxId;
                                }
                                pData.Bank = user.Bank;
                                pData.BankAccountNumber = user.BankAccountNumber;
                                MobileComPayment paymentStatus = db.MobileComPayments.FirstOrDefault(c => c.MobileUserId == user.Id && c.PaymentMonth.Year == monthYear.Year && c.PaymentMonth.Month == monthYear.Month);
                                pData.PaymentStatus = paymentStatus != null ? "Y" : "N";
                                pData.Payment = monthYear.Year <= now.Year && monthYear.Month < now.Month ? true : false;

                                double sumCom = comTrans.Sum(s => s.TotalBaht);
                                pData.ComTrans = comTrans;
                                pData.Commission = sumCom;
                                pData.Tier = UserDAL.GetUserTier(user.Id)?.TierName;
                                dataIndex.Data.Add(pData);
                            }
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
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth) && !string.IsNullOrEmpty(filter.year) && !string.IsNullOrEmpty(filter.month))
                {
                    DateTime monthYear = DateTime.ParseExact($"{filter.year} {filter.month} 01", "yyyy MMMM dd", CultureInfo.InvariantCulture);
                    DateTime now = DataDAL.GetDateTimeNow();
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

        [HttpPost]
        public async Task<IHttpActionResult> GetCommissionReportIndex(FilterParams filter)
        {
            try
            {
                string userAuth = Request.Headers.GetCookies("UserCookie").FirstOrDefault()?["UserCookie"].Value;
                if (!string.IsNullOrEmpty(userAuth))
                {
                    filter.status = DataDAL.GetActiveFlag(filter.status);
                    DateTime now = DataDAL.GetDateTimeNow();
                    using (var db = new spasystemdbEntities())
                    {
                        CommissionReportDataIndex dataIndex = new CommissionReportDataIndex();
                        dataIndex.Index = filter.page;
                        int tableMaxRow = int.Parse(DataDAL.GetMobileSetting("TABLE_MAX_ROW"));
                        dataIndex.RowPerPage = tableMaxRow;
                        Nullable<DateTime> filterFrom = null;
                        Nullable<DateTime> filterTo = null;
                        if (!string.IsNullOrEmpty(filter.periodFrom) && !string.IsNullOrEmpty(filter.periodTo))
                        {
                            filterFrom = DateTime.ParseExact($"{filter.periodFrom}", "dd MMM yyyy", CultureInfo.InvariantCulture);
                            filterTo = DateTime.ParseExact($"{filter.periodTo}", "dd MMM yyyy", CultureInfo.InvariantCulture).AddDays(1);
                        }

                        int filterCompanyTypeOfUsage = 0;
                        if (string.IsNullOrEmpty(filter.companyTypeOfUsage))
                        {
                            MobileDropdown CompanyTypeOfUsage = db.MobileDropdowns.FirstOrDefault(c => c.GroupName == "COM_TYPE_OF_USAGE" && c.Value == filter.companyTypeOfUsage);
                            if (CompanyTypeOfUsage != null) filterCompanyTypeOfUsage = CompanyTypeOfUsage.Id;
                        }

                        List<int> comTranIds = (from comTran in db.MobileComTransactions
                                               join t2 in db.MobileUsers on comTran.MobileUserId equals t2.Id into g1
                                               from user in g1.DefaultIfEmpty()
                                               where !string.IsNullOrEmpty(filter.firstName) ? user.FirstName.Contains(filter.firstName) : true
                                               && !string.IsNullOrEmpty(filter.lastName) ? user.LastName.Contains(filter.lastName) : true
                                               && !string.IsNullOrEmpty(filter.phone) ? user.PhoneNumber.Contains(filter.phone) : true
                                               && filterFrom != null && filterTo != null ? comTran.Created >= filterFrom && comTran.Created < filterTo : true
                                               && (filter.companyTypeOfUsage != null ? filterCompanyTypeOfUsage == user.CompanyTypeOfUsage || filter.companyTypeOfUsage == "All" : true)
                                               && !string.IsNullOrEmpty(filter.companyName) ? user.CompanyName.Contains(filter.companyName) : true
                                                select comTran.Id).ToList();

                        List<CommissionReportData> comTrans = (from comTran in db.MobileComTransactions.Where(c => comTranIds.Contains(c.Id))
                                                               .OrderByDescending(o => o.Id).Skip(tableMaxRow * (filter.page - 1)).Take(tableMaxRow)
                                                                join t2 in db.MobileUsers on comTran.MobileUserId equals t2.Id into g1
                                                                from user in g1.DefaultIfEmpty()
                                                                select new CommissionReportData
                                                                {
                                                                    Id = comTran.Id,
                                                                    MobileUserId = user.Id,
                                                                    FirstName = user.FirstName,
                                                                    LastName = user.LastName,
                                                                    PhoneNumber = user.PhoneNumber,
                                                                    Commission = comTran.TotalBaht,
                                                                    BranchId = comTran.BranchId,
                                                                    CompanyName = user.CompanyName,
                                                                    Created = comTran.Created,
                                                                    CreatedBy = comTran.CreatedBy
                                                                }).ToList();
                        List<int> branchList = DataDAL.GetBranchList().Select(s => s.Id).ToList();
                        foreach (CommissionReportData comTran in comTrans)
                        {
                            string userTierName = UserDAL.GetUserTier(comTran.MobileUserId)?.TierName;
                            if (string.IsNullOrEmpty(filter.tierName) || filter.tierName == userTierName || filter.tierName == "All")
                            {
                                Branch branch = db.Branches.FirstOrDefault(c => branchList.Contains(comTran.BranchId) && c.Id == comTran.BranchId);
                                if (branch != null && (string.IsNullOrEmpty(filter.branchName) || filter.branchName == branch.Name || filter.branchName == "All"))
                                {
                                    comTran.BranchName = branch.Name;
                                    comTran.Tier = userTierName;
                                    dataIndex.Data.Add(comTran);
                                }
                            }
                        }

                        decimal rowPerPage = Convert.ToDecimal(comTranIds.Count) / Convert.ToDecimal(tableMaxRow);
                        for (int i = 0; i < rowPerPage || i == 0; i++)
                        {
                            dataIndex.Indices.Add(i + 1);
                        }

                        return Ok(dataIndex);
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }

        [HttpPost]
        public async Task<IHttpActionResult> CommissionReceipt(ReceiptParams receiptParams)
        {
            try
            {
                MobileUser user = UserDAL.GetUserByToken(receiptParams.Token);
                if (user != null && !string.IsNullOrEmpty(receiptParams.ReceiptCode))
                {
                    using (var db = new spasystemdbEntities())
                    {
                        DateTime now = DataDAL.GetDateTimeNow();
                        MobileComTier userTier = UserDAL.GetUserTier(user.Id);
                        Receipt receipt = db.Receipts.FirstOrDefault(c => 
                                            c.Code == receiptParams.ReceiptCode
                                            && c.UsedStatus != "Y");
                        DateTime? receiptExpired = receipt?.Created?.ToLocalTime().AddMinutes(15);
                        if (userTier != null && receipt != null && receiptExpired != null && receiptExpired >= now)
                        {
                            List<OrderRecord> orders = db.OrderRecords.Where(c => c.ReceiptId == receipt.Id && c.BranchId == receipt.BranchId).ToList();
                            if (orders.Count > 0)
                            {
                                OrderRecord order = orders.FirstOrDefault();
                                List<OrderRecordWithDiscount> mapOderDiscounts = db.OrderRecordWithDiscounts.Where(c => c.BranchId == order.BranchId && c.OrderRecordId == order.Id).ToList();
                                double totalDiscount = 0;
                                foreach (OrderRecordWithDiscount item in mapOderDiscounts)
                                {
                                    DiscountRecord DiscountRecord = db.DiscountRecords.FirstOrDefault(c => c.BranchId == order.BranchId && c.Id == item.DiscountRecordId);
                                    if (DiscountRecord != null)
                                    {
                                        totalDiscount += Convert.ToDouble(DiscountRecord.Value);
                                    }
                                }

                                double totalBaht = orders.Sum(s => s.Price) - totalDiscount;
                                totalBaht = totalBaht * (userTier.ComPercentage / 100);

                                MobileComTransaction comTran = new MobileComTransaction();
                                comTran.MobileUserId = user.Id;
                                comTran.BranchId = orders.FirstOrDefault().BranchId;
                                comTran.TotalBaht = totalBaht;
                                if (receiptParams.Latitude != null) comTran.Latitude = double.Parse(receiptParams.Latitude);
                                if (receiptParams.Longitude != null) comTran.Longitude = double.Parse(receiptParams.Longitude);
                                comTran.TotalBaht = totalBaht;
                                comTran.Created = now;
                                comTran.CreatedBy = "api.mobile";
                                db.MobileComTransactions.Add(comTran);

                                receipt.UsedStatus = "Y";
                                receipt.Updated = now;
                                receipt.UpdatedBy = "api.mobile";

                                db.SaveChanges();

                                return Ok(comTran);
                            }
                        }
                    }
                }
            }
            catch { }

            return Content(HttpStatusCode.NoContent, "No content.");
        }
    }
}
