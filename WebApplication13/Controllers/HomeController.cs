﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    public class HomeController : Controller
    {
        private spasystemdbEntities db = new spasystemdbEntities();
        string connetionString;
        SqlConnection cnn;
        // GET: Home
        public ActionResult Index()
        {
            //User getUs = getUserAuthen();

            //Check user is login with old logic
            //var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            //if (noms == null)
            //{
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Dashboard");
            //}

            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie_ = Request.Cookies["TokenCookie"];

            //Check user token from cookie
            if (cookie_ != null)
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View();
            }


        }

        [HttpPost]
        public ActionResult Index(UserLogin userModel)
        {
            // if credentials are correct.
            User checkUs = getUserAuthen(userModel);
            if (checkUs != null)
            {

                ////Cookie development process
                // Generate your token. This could be anything, for example:
                string token = Guid.NewGuid().ToString();

                // Create a new cookie
                HttpCookie cookie = new HttpCookie("TokenCookie", token);
                HttpCookie cookie_user = new HttpCookie("UserCookie", checkUs.Username);

                // Optional: Set the cookie expiration
                cookie.Expires = DateTime.Now.AddDays(5);
                cookie_user.Expires = DateTime.Now.AddDays(5);

                // Optional: Secure your cookie (only transmitted over HTTPS)
                //cookie.Secure = true;

                // Add the cookie to the response to set it on the client
                Response.Cookies.Add(cookie);
                Response.Cookies.Add(cookie_user);

                return RedirectToAction("Dashboard");

                // login user logic here.
                //Old logic for login by using server cache
                //var noms = System.Runtime.Caching.
                //;
                //noms = checkUs.Name;
                //System.Runtime.Caching.MemoryCache.Default["names"] = noms;
                //return RedirectToAction("Dashboard");
            }
            else
            {
                // show login page again.
                return View();
            }
        }

        public ActionResult Dashboard(string cmd)
        {

            //Check if Log out button is clicked
            if (cmd != null)
            {

                //Remove cookie when log out
                RemoveCookie();
                return RedirectToAction("Index");
            }

            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;

                    //Check user is current enabled
                    if(CheckUserIsEnable(userName).Equals("false"))
                    {
                        RemoveCookie();
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                HeaderValue hv = new HeaderValue();

                SqlCommand command;
                SqlDataReader dataReader;
                String sql = " ";
                sql = "select sum(dbo.OrderRecord.Price) as 'Total Sales', count(dbo.OrderRecord.Id) as 'Total Pax', (select sum(dbo.Account.StaffAmount) from dbo.Account left join dbo.Branch on dbo.Account.BranchId = dbo.Branch.Id where dbo.Account.Date = (select top 1 dbo.Account.Date from dbo.Account order by dbo.Account.Date desc) and dbo.Branch.Status = 'true' and dbo.Branch.UrbanSystem = 'true') as 'Total Staff', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average per Pax', COUNT(CASE WHEN dbo.OrderRecord.MemberId != '0' THEN dbo.OrderRecord.MemberId ELSE NULL END) as 'Total VIP' from dbo.OrderRecord left join dbo.Account on dbo.OrderRecord.AccountId = dbo.Account.Id and dbo.OrderRecord.BranchId = dbo.Account.BranchId left join dbo.Branch on dbo.OrderRecord.BranchId = dbo.Branch.Id where dbo.Account.Date = (select top 1 dbo.Account.Date from dbo.Account order by dbo.Account.Date desc) and dbo.Branch.Status = 'true' and dbo.Branch.UrbanSystem = 'true'";

                connetionString = ConfigurationManager.AppSettings["cString"];
                cnn = new SqlConnection(connetionString);
                cnn.Open();
                command = new SqlCommand(sql, cnn);

                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader.GetValue(0).Equals(null) || dataReader.GetValue(0).Equals("null"))
                    {
                        hv = new HeaderValue() { strSales = "0", strPax = "0", strStaff = "0", strCommission = "0", strAverage = "0", strVipCount = "0" };
                    }
                    else
                    {
                        hv = new HeaderValue() { strSales = String.Format("{0:n0}", dataReader.GetValue(0)), strPax = String.Format("{0:n0}", dataReader.GetValue(1)), strStaff = String.Format("{0:n0}", dataReader.GetValue(2)), strCommission = String.Format("{0:n0}", dataReader.GetValue(3)), strAverage = String.Format("{0:n0}", dataReader.GetValue(4)), strVipCount = String.Format("{0:n0}", dataReader.GetValue(5)) };
                    }

                }

                hv.strLoginName = userName;

                dataReader.Close();
                command.Dispose();
                cnn.Close();

                return View(hv);
            }
            else
            {
                return RedirectToAction("Index");
            }


        }


        public ActionResult Urban(string accountId, string monthNo, string yearNo, string cmd, int bid)
        {
            int branchIds = bid;

            //Check if Log out button is clicked
            if (cmd != null)
            {

                //Remove cookie when log out
                RemoveCookie();
                return RedirectToAction("Index");
            }

            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;
                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                if (accountId != null)
                {
                    string tSales = " ";
                    string tPaxes = " ";
                    string tAverage = " ";
                    string tStaff = " ";
                    string topAname = " ";
                    string topBname = " ";
                    string tComs = " ";
                    string tOtherS = " ";
                    string tInitMoney = " ";
                    string tOil = " ";
                    int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
                    int sumDiscount = 0; // Updated 11 October 2022
                    int tSaleMinusDiscount = 0; // Updated 11 October 2022
                    string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
                    List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022

                    // Updated 11 October 2022
                    using (var context = new spasystemdbEntities())
                    {

                        listDiscount = context.DiscountRecords
                                        .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger && b.CancelStatus == "false")
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    for (int m = 0; m < listDiscount.Count(); m++)
                    {
                        sumDiscount += Int32.Parse(listDiscount[m].Value);
                    }
                    /////////////////////////

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

                    connetionString = ConfigurationManager.AppSettings["cString"];
                    cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    command = new SqlCommand(sql, cnn);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        tSales = String.Format("{0:n0}", dataReader.GetValue(0));
                        tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
                        tComs = String.Format("{0:n0}", dataReader.GetValue(2));
                        tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
                        tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
                        tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
                        topAname = dataReader.GetValue(6).ToString();
                        topBname = dataReader.GetValue(7).ToString();
                        tOil = String.Format("{0:n0}", dataReader.GetValue(8));
                        tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
                    }

                    dataReader.Close();
                    command.Dispose();
                    cnn.Close();

                    int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
                    string tSales_trim = tSales.Replace(",", "");
                    string tOil_trim = tOil.Replace(",", "");
                    string tOtherS_trim = tOtherS.Replace(",", "");
                    string tComs_trim = tComs.Replace(",", "");

                    if (string.IsNullOrEmpty(tSales_trim))
                    {

                    }
                    else
                    {
                        convert_tSales = Int32.Parse(tSales_trim);

                    }

                    if (string.IsNullOrEmpty(tOtherS_trim))
                    {

                    }
                    else
                    {
                        convert_tOtherS = Int32.Parse(tOtherS_trim);

                    }


                    if (string.IsNullOrEmpty(tOil_trim))
                    {

                    }
                    else
                    {
                        convert_tOil = Int32.Parse(tOil_trim);

                    }

                    if (string.IsNullOrEmpty(tComs_trim))
                    {

                    }
                    else
                    {
                        convert_tComs = Int32.Parse(tComs_trim);

                    }

                    string strDis = String.Format("{0:n0}", sumDiscount);

                    var result = GetTotalCashAndCredit(branchIds, accountIdInInteger);
                    int totalCash = result.TotalCash;
                    int totalCredit = result.TotalCredit;
                    String strCash = String.Format("{0:n0}", totalCash);
                    String strCredit = String.Format("{0:n0}", totalCredit);


                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
                        ////////////
                        //Waiting for confirm this has to be deduct discount 11 October 2022
                        //strSales = tSaleMinusDiscountInString,
                        ////////////
                        strPax = tPaxes,
                        strStaff = tStaff,
                        strCommission = tComs,
                        arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
                        strPieTopAName = topAname,
                        strPieTopBName = topBname,
                        //arrPieTopAVal = getTopAForAday(branchIds),
                        //arrPieTopBVal = getTopBForAday(branchIds),
                        finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAverage,
                        strOtherSale = String.Format("{0:n0}", convert_tOtherS),
                        strInitMoney = tInitMoney,
                        strOilIncome = tOil,
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
                        strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
                        strLoginName = userName,
                        strVoucher = strDis,
                        strCash = strCash,
                        strCredit = strCredit,
                        bid = bid,
                        bName = getBranchName(bid)
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);

                    List<Account> listAccountInMonth = new List<Account>();
                    List<OrderRecord> orderRecords = new List<OrderRecord>();
                    List<OtherSaleRecord> otherSaleRecords = new List<OtherSaleRecord>();
                    SystemSetting oilPriceSetting;
                    List<OrderRecord> orderRecordsVIP = new List<OrderRecord>();
                    List<DiscountRecord> discountRecords = new List<DiscountRecord>();

                    using (var context = new spasystemdbEntities())
                    {
                        // Get accounts for the specified month and year
                        listAccountInMonth = context.Accounts
                                                    .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                                    .ToList();

                        // Get all relevant order records for these accounts
                        var accountIds = listAccountInMonth.Select(a => a.Id).ToList();
                        orderRecords = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.CancelStatus == "false")
                                              .ToList();

                        // Get all relevant other sale records for these accounts
                        otherSaleRecords = context.OtherSaleRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.CancelStatus == "false")
                                                  .ToList();

                        // Get the oil price setting
                        oilPriceSetting = context.SystemSettings
                                                 .Where(ss => ss.BranchId == branchIds && ss.Name == "OilPrice")
                                                 .FirstOrDefault();

                        //Get vip count
                        orderRecordsVIP = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.CancelStatus == "false" && or.MemberId != 0)
                                              .ToList();

                        // Get all voucher or cash card discount
                        discountRecords = context.DiscountRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.CancelStatus == "false")
                                                  .ToList();
                    }

                    int oilPrice = Int32.Parse(oilPriceSetting?.Value ?? "0");
                    int tSales = 0, tPaxNum = 0, tComs = 0, tStaff = 0, tOtherS = 0, tInitMoney = 0, tOil = 0, tBalanceNet = 0, tVipCpunt = 0, tDiscount = 0, tCash = 0, tCredit = 0;

                    foreach (var account in listAccountInMonth)
                    {
                        var accountOrderRecords = orderRecords.Where(or => or.AccountId == account.Id).ToList();
                        var accountOtherSales = otherSaleRecords.Where(osr => osr.AccountId == account.Id).ToList();
                        var accountOrderRecordsVIP = orderRecordsVIP.Where(or => or.AccountId == account.Id).ToList();
                        var accountDiscount = discountRecords.Where(or => or.AccountId == account.Id).ToList();

                        tSales += accountOrderRecords.Sum(or => (int)or.Price);
                        tPaxNum += accountOrderRecords.Count();
                        tComs += accountOrderRecords.Sum(or => (int)or.Commission);
                        tStaff += (int)account.StaffAmount;
                        tOtherS += accountOtherSales.Sum(osr => (int)osr.Price);
                        tInitMoney += (int)account.StartMoney;
                        tVipCpunt += accountOrderRecordsVIP.Count();
                        tDiscount += accountDiscount.Sum(osr => int.Parse(osr.Value));
                        //tCash += getCash(branchIds, account.Id) - getVoucherCash(branchIds, account.Id);
                        //tCredit += getCredit(branchIds, account.Id) - getVoucherCredit(branchIds, account.Id);
                    }

                    tOil = tStaff * oilPrice;

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);
                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    var accountIds_ = listAccountInMonth.Select(a => a.Id).ToList();
                    var summary = GetTotalCashAndCredit_multiAcc(branchIds, accountIds_);
                    tCash = summary.TotalCash;
                    tCredit = summary.TotalCredit;


                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        //arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        //arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet),
                        strLoginName = userName,
                        strVipCount = tVipCpunt.ToString(), //new
                        strVoucher = String.Format("{0:n0}", tDiscount), //new
                        strCash = String.Format("{0:n0}", tCash), //new
                        strCredit = String.Format("{0:n0}", tCredit), //new
                        bid = bid,
                        bName = getBranchName(bid)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);

                    List<Account> listAccountInYear = new List<Account>();
                    List<OrderRecord> orderRecords = new List<OrderRecord>();
                    List<OtherSaleRecord> otherSaleRecords = new List<OtherSaleRecord>();
                    SystemSetting oilPriceSetting;
                    List<OrderRecord> orderRecordsVIP = new List<OrderRecord>();
                    List<DiscountRecord> discountRecords = new List<DiscountRecord>();

                    using (var context = new spasystemdbEntities())
                    {
                        // Get accounts for the specified year
                        listAccountInYear = context.Accounts
                                                   .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                                   .ToList();

                        // Get all relevant order records for these accounts within the year
                        var accountIds = listAccountInYear.Select(a => a.Id).ToList();
                        orderRecords = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.Date.Year == selectedYear && or.CancelStatus == "false")
                                              .ToList();

                        // Get all relevant other sale records for these accounts within the year
                        otherSaleRecords = context.OtherSaleRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.Date.Year == selectedYear && osr.CancelStatus == "false")
                                                  .ToList();

                        // Get the oil price setting
                        oilPriceSetting = context.SystemSettings
                                                 .Where(ss => ss.BranchId == branchIds && ss.Name == "OilPrice")
                                                 .FirstOrDefault();

                        //Get vip count
                        orderRecordsVIP = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.CancelStatus == "false" && or.MemberId != 0)
                                              .ToList();

                        // Get all voucher or cash card discount
                        discountRecords = context.DiscountRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.CancelStatus == "false")
                                                  .ToList();
                    }

                    int oilPrice = Int32.Parse(oilPriceSetting?.Value ?? "0");
                    int tSales = 0, tPaxNum = 0, tComs = 0, tStaff = 0, tOtherS = 0, tInitMoney = 0, tOil = 0, tBalanceNet = 0, tVipCpunt = 0, tDiscount = 0, tCash = 0, tCredit = 0;

                    foreach (var account in listAccountInYear)
                    {
                        var accountOrderRecords = orderRecords.Where(or => or.AccountId == account.Id).ToList();
                        var accountOtherSales = otherSaleRecords.Where(osr => osr.AccountId == account.Id).ToList();
                        var accountOrderRecordsVIP = orderRecordsVIP.Where(or => or.AccountId == account.Id).ToList();
                        var accountDiscount = discountRecords.Where(or => or.AccountId == account.Id).ToList();

                        tSales += accountOrderRecords.Sum(or => (int)or.Price);
                        tPaxNum += accountOrderRecords.Count();
                        tComs += accountOrderRecords.Sum(or => (int)or.Commission);
                        tStaff += (int)account.StaffAmount;
                        tOtherS += accountOtherSales.Sum(osr => (int)osr.Price);
                        tInitMoney += (int)account.StartMoney;
                        tVipCpunt += accountOrderRecordsVIP.Count();
                        tDiscount += accountDiscount.Sum(osr => int.Parse(osr.Value));
                    }

                    tOil = tStaff * oilPrice;

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);
                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    var accountIds_ = listAccountInYear.Select(a => a.Id).ToList();
                    var summary = GetTotalCashAndCredit_multiAcc(branchIds, accountIds_);
                    tCash = summary.TotalCash;
                    tCredit = summary.TotalCredit;

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        //arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        //arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet),
                        strLoginName = userName,
                        strVipCount = tVipCpunt.ToString(), //new
                        strVoucher = String.Format("{0:n0}", tDiscount), //new
                        strCash = String.Format("{0:n0}", tCash), //new
                        strCredit = String.Format("{0:n0}", tCredit), //new
                        bid = bid,
                        bName = getBranchName(bid)
                    };

                    return View(hv);
                }
                else
                {

                    Account ac = getAccountValue(branchIds);
                    string tSales = " ";
                    string tPaxes = " ";
                    string tAverage = " ";
                    string tStaff = " ";
                    string topAname = " ";
                    string topBname = " ";
                    string tComs = " ";
                    string tOtherS = " ";
                    string tInitMoney = " ";
                    string tOil = " ";
                    int accountIdInInteger = ac.Id; // Updated 11 October 2022
                    int sumDiscount = 0; // Updated 11 October 2022

                    List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022

                    // Updated 11 October 2022
                    using (var context = new spasystemdbEntities())
                    {

                        listDiscount = context.DiscountRecords
                                        .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger && b.CancelStatus == "false")
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    for (int m = 0; m < listDiscount.Count(); m++)
                    {
                        sumDiscount += Int32.Parse(listDiscount[m].Value);
                    }
                    /////////////////////////

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + ac.Id + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + ac.Id + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + ac.Id + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + ac.Id + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + ac.Id + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + ac.Id + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + ac.Id + "' and dbo.OrderRecord.CancelStatus = 'false';";

                    connetionString = ConfigurationManager.AppSettings["cString"];
                    cnn = new SqlConnection(connetionString);
                    cnn.Open();
                    command = new SqlCommand(sql, cnn);

                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        tSales = String.Format("{0:n0}", dataReader.GetValue(0));
                        tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
                        tComs = String.Format("{0:n0}", dataReader.GetValue(2));
                        tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
                        tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
                        tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
                        topAname = dataReader.GetValue(6).ToString();
                        topBname = dataReader.GetValue(7).ToString();
                        tOil = String.Format("{0:n0}", dataReader.GetValue(8));
                        tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
                    }

                    dataReader.Close();
                    command.Dispose();
                    cnn.Close();

                    int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
                    string tSales_trim = tSales.Replace(",", "");
                    string tOil_trim = tOil.Replace(",", "");
                    string tOtherS_trim = tOtherS.Replace(",", "");
                    string tComs_trim = tComs.Replace(",", "");

                    if (string.IsNullOrEmpty(tSales_trim))
                    {

                    }
                    else
                    {
                        convert_tSales = Int32.Parse(tSales_trim);

                    }

                    if (string.IsNullOrEmpty(tOtherS_trim))
                    {

                    }
                    else
                    {
                        convert_tOtherS = Int32.Parse(tOtherS_trim);

                    }


                    if (string.IsNullOrEmpty(tOil_trim))
                    {

                    }
                    else
                    {
                        convert_tOil = Int32.Parse(tOil_trim);

                    }

                    if (string.IsNullOrEmpty(tComs_trim))
                    {

                    }
                    else
                    {
                        convert_tComs = Int32.Parse(tComs_trim);

                    }

                    string strDis = String.Format("{0:n0}", sumDiscount);

                    var result = GetTotalCashAndCredit(branchIds, accountIdInInteger);
                    int totalCash = result.TotalCash;
                    int totalCredit = result.TotalCredit;
                    String strCash = String.Format("{0:n0}", totalCash);
                    String strCredit = String.Format("{0:n0}", totalCredit);


                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
                        ////////////
                        //Waiting for confirm this has to be deduct discount 11 October 2022
                        //strSales = tSaleMinusDiscountInString,
                        ////////////
                        strPax = tPaxes,
                        strStaff = tStaff,
                        strCommission = tComs,
                        arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
                        strPieTopAName = topAname,
                        strPieTopBName = topBname,
                        //arrPieTopAVal = getTopAForAday(branchIds),
                        //arrPieTopBVal = getTopBForAday(branchIds),
                        finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAverage,
                        strOtherSale = String.Format("{0:n0}", convert_tOtherS),
                        strInitMoney = tInitMoney,
                        strOilIncome = tOil,
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
                        strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
                        strLoginName = userName,
                        strVoucher = strDis,
                        strCash = strCash,
                        strCredit = strCredit,
                        bid = bid,
                        bName = getBranchName(bid)
                    };


                    return View(hv);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //public ActionResult Khaosan(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 2;

        //    if (cmd != null)
        //    {
        //        foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        {
        //            System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        }
        //    }

        //    var noms = System.Runtime.Caching.MemoryCache.Default["names"];
        //    if (noms == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet)
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet)
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
        //            };


        //            return View(hv);
        //        }
        //    }
        //}

        //public ActionResult UrbanTwo(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 3;

        //    if (cmd != null)
        //    {
        //        foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        {
        //            System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        }
        //    }

        //    var noms = System.Runtime.Caching.MemoryCache.Default["names"];
        //    if (noms == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet)
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet)
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
        //            };


        //            return View(hv);
        //        }
        //    }

        //}

        //public ActionResult UrbanThree(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 4;

        //    //Check if Log out button is clicked
        //    if (cmd != null)
        //    {
        //        //Old logic for Log out by clear all host memory cache
        //        //foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        //{
        //        //    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        //}

        //        //Remove cookie when log out
        //        RemoveCookie();
        //        return RedirectToAction("Index");
        //    }

        //    //Check user token
        //    // Retrieve the cookie from the request
        //    HttpCookie cookie = Request.Cookies["TokenCookie"];
        //    HttpCookie cookie_user = Request.Cookies["UserCookie"];

        //    string tokenValue = null;
        //    string userName = null;

        //    //Check user token from cookie
        //    if (cookie != null)
        //    {
        //        tokenValue = cookie.Value;

        //        //Check user name from cookie
        //        if (cookie_user != null)
        //        {
        //            userName = cookie_user.Value;
        //        }
        //        else
        //        {
        //            userName = "Annonymous";
        //        }

        //        //Prepare content for View
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            int tSaleMinusDiscount = 0; // Updated 11 October 2022
        //            string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }
        //            /////////////////////////

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
        //            tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022
        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                ////////////
        //                //Waiting for confirm this has to be deduct discount 11 October 2022
        //                //strSales = tSaleMinusDiscountInString,
        //                ////////////
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = ac.Id; // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}


        //public ActionResult UrbanFour(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 5;

        //    //Check if Log out button is clicked
        //    if (cmd != null)
        //    {
        //        //Old logic for Log out by clear all host memory cache
        //        //foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        //{
        //        //    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        //}

        //        //Remove cookie when log out
        //        RemoveCookie();
        //        return RedirectToAction("Index");
        //    }

        //    //Check user token
        //    // Retrieve the cookie from the request
        //    HttpCookie cookie = Request.Cookies["TokenCookie"];
        //    HttpCookie cookie_user = Request.Cookies["UserCookie"];

        //    string tokenValue = null;
        //    string userName = null;

        //    //Check user token from cookie
        //    if (cookie != null)
        //    {
        //        tokenValue = cookie.Value;

        //        //Check user name from cookie
        //        if (cookie_user != null)
        //        {
        //            userName = cookie_user.Value;
        //        }
        //        else
        //        {
        //            userName = "Annonymous";
        //        }

        //        //Prepare content for View
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            int tSaleMinusDiscount = 0; // Updated 11 October 2022
        //            string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }
        //            /////////////////////////

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
        //            tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022
        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                ////////////
        //                //Waiting for confirm this has to be deduct discount 11 October 2022
        //                //strSales = tSaleMinusDiscountInString,
        //                ////////////
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = ac.Id; // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        //public ActionResult UrbanFive(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 6;

        //    //Check if Log out button is clicked
        //    if (cmd != null)
        //    {
        //        //Old logic for Log out by clear all host memory cache
        //        //foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        //{
        //        //    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        //}

        //        //Remove cookie when log out
        //        RemoveCookie();
        //        return RedirectToAction("Index");
        //    }

        //    //Check user token
        //    // Retrieve the cookie from the request
        //    HttpCookie cookie = Request.Cookies["TokenCookie"];
        //    HttpCookie cookie_user = Request.Cookies["UserCookie"];

        //    string tokenValue = null;
        //    string userName = null;

        //    //Check user token from cookie
        //    if (cookie != null)
        //    {
        //        tokenValue = cookie.Value;

        //        //Check user name from cookie
        //        if (cookie_user != null)
        //        {
        //            userName = cookie_user.Value;
        //        }
        //        else
        //        {
        //            userName = "Annonymous";
        //        }

        //        //Prepare content for View
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            int tSaleMinusDiscount = 0; // Updated 11 October 2022
        //            string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }
        //            /////////////////////////

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
        //            tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022
        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                ////////////
        //                //Waiting for confirm this has to be deduct discount 11 October 2022
        //                //strSales = tSaleMinusDiscountInString,
        //                ////////////
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = ac.Id; // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        //public ActionResult UrbanSix(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 7;

        //    //Check if Log out button is clicked
        //    if (cmd != null)
        //    {
        //        //Old logic for Log out by clear all host memory cache
        //        //foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        //{
        //        //    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        //}

        //        //Remove cookie when log out
        //        RemoveCookie();
        //        return RedirectToAction("Index");
        //    }

        //    //Check user token
        //    // Retrieve the cookie from the request
        //    HttpCookie cookie = Request.Cookies["TokenCookie"];
        //    HttpCookie cookie_user = Request.Cookies["UserCookie"];

        //    string tokenValue = null;
        //    string userName = null;

        //    //Check user token from cookie
        //    if (cookie != null)
        //    {
        //        tokenValue = cookie.Value;

        //        //Check user name from cookie
        //        if (cookie_user != null)
        //        {
        //            userName = cookie_user.Value;
        //        }
        //        else
        //        {
        //            userName = "Annonymous";
        //        }

        //        //Prepare content for View
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            int tSaleMinusDiscount = 0; // Updated 11 October 2022
        //            string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }
        //            /////////////////////////

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
        //            tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022
        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                ////////////
        //                //Waiting for confirm this has to be deduct discount 11 October 2022
        //                //strSales = tSaleMinusDiscountInString,
        //                ////////////
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = ac.Id; // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        //public ActionResult UrbanSeven(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 8;

        //    //Check if Log out button is clicked
        //    if (cmd != null)
        //    {
        //        //Old logic for Log out by clear all host memory cache
        //        //foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        //{
        //        //    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        //}

        //        //Remove cookie when log out
        //        RemoveCookie();
        //        return RedirectToAction("Index");
        //    }

        //    //Check user token
        //    // Retrieve the cookie from the request
        //    HttpCookie cookie = Request.Cookies["TokenCookie"];
        //    HttpCookie cookie_user = Request.Cookies["UserCookie"];

        //    string tokenValue = null;
        //    string userName = null;

        //    //Check user token from cookie
        //    if (cookie != null)
        //    {
        //        tokenValue = cookie.Value;

        //        //Check user name from cookie
        //        if (cookie_user != null)
        //        {
        //            userName = cookie_user.Value;
        //        }
        //        else
        //        {
        //            userName = "Annonymous";
        //        }

        //        //Prepare content for View
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            int tSaleMinusDiscount = 0; // Updated 11 October 2022
        //            string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }
        //            /////////////////////////

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
        //            tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022
        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                ////////////
        //                //Waiting for confirm this has to be deduct discount 11 October 2022
        //                //strSales = tSaleMinusDiscountInString,
        //                ////////////
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = ac.Id; // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        //public ActionResult UrbanEight(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 10;

        //    //Check if Log out button is clicked
        //    if (cmd != null)
        //    {
        //        //Old logic for Log out by clear all host memory cache
        //        //foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        //{
        //        //    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        //}

        //        //Remove cookie when log out
        //        RemoveCookie();
        //        return RedirectToAction("Index");
        //    }

        //    //Check user token
        //    // Retrieve the cookie from the request
        //    HttpCookie cookie = Request.Cookies["TokenCookie"];
        //    HttpCookie cookie_user = Request.Cookies["UserCookie"];

        //    string tokenValue = null;
        //    string userName = null;

        //    //Check user token from cookie
        //    if (cookie != null)
        //    {
        //        tokenValue = cookie.Value;

        //        //Check user name from cookie
        //        if (cookie_user != null)
        //        {
        //            userName = cookie_user.Value;
        //        }
        //        else
        //        {
        //            userName = "Annonymous";
        //        }

        //        //Prepare content for View
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            int tSaleMinusDiscount = 0; // Updated 11 October 2022
        //            string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }
        //            /////////////////////////

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
        //            tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022
        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                ////////////
        //                //Waiting for confirm this has to be deduct discount 11 October 2022
        //                //strSales = tSaleMinusDiscountInString,
        //                ////////////
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = ac.Id; // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        //public ActionResult ThaiGardenOne(string accountId, string monthNo, string yearNo, string cmd)
        //{
        //    int branchIds = 12;

        //    //Check if Log out button is clicked
        //    if (cmd != null)
        //    {
        //        //Old logic for Log out by clear all host memory cache
        //        //foreach (var element in System.Runtime.Caching.MemoryCache.Default)
        //        //{
        //        //    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
        //        //}

        //        //Remove cookie when log out
        //        RemoveCookie();
        //        return RedirectToAction("Index");
        //    }

        //    //Check user token
        //    // Retrieve the cookie from the request
        //    HttpCookie cookie = Request.Cookies["TokenCookie"];
        //    HttpCookie cookie_user = Request.Cookies["UserCookie"];

        //    string tokenValue = null;
        //    string userName = null;

        //    //Check user token from cookie
        //    if (cookie != null)
        //    {
        //        tokenValue = cookie.Value;

        //        //Check user name from cookie
        //        if (cookie_user != null)
        //        {
        //            userName = cookie_user.Value;
        //        }
        //        else
        //        {
        //            userName = "Annonymous";
        //        }

        //        //Prepare content for View
        //        if (accountId != null)
        //        {
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            int tSaleMinusDiscount = 0; // Updated 11 October 2022
        //            string tSaleMinusDiscountInString = " "; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }
        //            /////////////////////////

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
        //            tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022
        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                ////////////
        //                //Waiting for confirm this has to be deduct discount 11 October 2022
        //                //strSales = tSaleMinusDiscountInString,
        //                ////////////
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, Int32.Parse(accountId)),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, accountId),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //        else if (monthNo != null)
        //        {
        //            int selectedMonth = Int32.Parse(monthNo);
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
        //            List<Account> listAccountInMonth = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInMonth = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInMonth.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
        //                tSales += getTotalSaleInMonth(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInMonth(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //System.Diagnostics.Debug.WriteLine("f");

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
        //                strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
        //                arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else if (yearNo != null)
        //        {
        //            int selectedYear = Int32.Parse(yearNo);
        //            DateTime dts = new DateTime(selectedYear, 1, 1);
        //            List<Account> listAccountInYear = new List<Account>();

        //            using (var context = new spasystemdbEntities())
        //            {

        //                listAccountInYear = context.Accounts
        //                                .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            Account ac = new Account();
        //            int tSales = 0;
        //            int tPaxNum = 0;
        //            int tComs = 0;
        //            int tStaff = 0;
        //            int tOtherS = 0;
        //            int tInitMoney = 0;
        //            int tOil = 0;
        //            int tBalanceNet = 0;

        //            for (int p = 0; p < listAccountInYear.Count(); p++)
        //            {
        //                ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
        //                tSales += getTotalSaleInYear(branchIds, ac.Id);
        //                tPaxNum += getPaxNum(branchIds, ac.Id);
        //                tComs += getTotalCommissionInYear(branchIds, ac.Id);
        //                tStaff += (int)ac.StaffAmount;
        //                tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
        //                tInitMoney += (int)ac.StartMoney;
        //                tOil += tStaff * getOilPrice(branchIds);
        //                tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
        //            }

        //            float tSalesInFloat = (float)tSales;
        //            float tPaxNumInFloat = (float)tPaxNum;
        //            float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = String.Format("{0:n0}", tSales),
        //                strPax = String.Format("{0:n0}", tPaxNum),
        //                strStaff = String.Format("{0:n0}", tStaff),
        //                strCommission = String.Format("{0:n0}", tComs),
        //                arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
        //                strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
        //                arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
        //                finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAvg.ToString(),
        //                strOtherSale = String.Format("{0:n0}", tOtherS),
        //                strInitMoney = String.Format("{0:n0}", tInitMoney),
        //                strOilIncome = String.Format("{0:n0}", tOil),
        //                strBalanceNet = String.Format("{0:n0}", tBalanceNet),
        //                strLoginName = userName
        //            };

        //            return View(hv);
        //        }
        //        else
        //        {

        //            Account ac = getAccountValue(branchIds);
        //            string tSales = " ";
        //            string tPaxes = " ";
        //            string tAverage = " ";
        //            string tStaff = " ";
        //            string topAname = " ";
        //            string topBname = " ";
        //            string tComs = " ";
        //            string tOtherS = " ";
        //            string tInitMoney = " ";
        //            string tOil = " ";
        //            int accountIdInInteger = ac.Id; // Updated 11 October 2022
        //            int sumDiscount = 0; // Updated 11 October 2022
        //            List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022


        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            // Updated 11 October 2022
        //            using (var context = new spasystemdbEntities())
        //            {

        //                listDiscount = context.DiscountRecords
        //                                .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
        //                                .OrderBy(b => b.Id)
        //                                .ToList();
        //            }

        //            for (int m = 0; m < listDiscount.Count(); m++)
        //            {
        //                sumDiscount += Int32.Parse(listDiscount[m].Value);
        //            }

        //            //int tPaxNum = getPaxNum(branchIds, ac.Id);
        //            //string tComs = getTotalCommission(branchIds, ac.Id);
        //            //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
        //            //float tSalesInFloat = (float)tSalesInInteger;
        //            //float tPaxNumInFloat = (float)tPaxNum;
        //            //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
        //            //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

        //            SqlCommand command;
        //            SqlDataReader dataReader;
        //            String sql = " ";
        //            sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
        //            //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

        //            connetionString = ConfigurationManager.AppSettings["cString"];
        //            cnn = new SqlConnection(connetionString);
        //            cnn.Open();
        //            command = new SqlCommand(sql, cnn);

        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                tSales = String.Format("{0:n0}", dataReader.GetValue(0));
        //                tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
        //                tComs = String.Format("{0:n0}", dataReader.GetValue(2));
        //                tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
        //                tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
        //                tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
        //                topAname = dataReader.GetValue(6).ToString();
        //                topBname = dataReader.GetValue(7).ToString();
        //                tOil = String.Format("{0:n0}", dataReader.GetValue(8));
        //                tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
        //            }

        //            dataReader.Close();
        //            command.Dispose();
        //            cnn.Close();

        //            int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
        //            string tSales_trim = tSales.Replace(",", "");
        //            string tOil_trim = tOil.Replace(",", "");
        //            string tOtherS_trim = tOtherS.Replace(",", "");
        //            string tComs_trim = tComs.Replace(",", "");

        //            if (string.IsNullOrEmpty(tSales_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tSales = Int32.Parse(tSales_trim);

        //            }

        //            if (string.IsNullOrEmpty(tOtherS_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOtherS = Int32.Parse(tOtherS_trim);

        //            }


        //            if (string.IsNullOrEmpty(tOil_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tOil = Int32.Parse(tOil_trim);

        //            }

        //            if (string.IsNullOrEmpty(tComs_trim))
        //            {

        //            }
        //            else
        //            {
        //                convert_tComs = Int32.Parse(tComs_trim);

        //            }

        //            string strDis = String.Format("{0:n0}", sumDiscount);
        //            int totalCash = getCash(branchIds, accountIdInInteger) - getVoucherCash(branchIds, accountIdInInteger);
        //            String strCash = String.Format("{0:n0}", totalCash);
        //            int totalCredit = getCredit(branchIds, accountIdInInteger) - getVoucherCredit(branchIds, accountIdInInteger);
        //            String strCredit = String.Format("{0:n0}", totalCredit);

        //            HeaderValue hv = new HeaderValue()
        //            {
        //                strSales = tSales,
        //                strPax = tPaxes,
        //                strStaff = tStaff,
        //                strCommission = tComs,
        //                arrGraphVal = getOrderRecordForGraph(branchIds, ac.Id),
        //                strPieTopAName = topAname,
        //                strPieTopBName = topBname,
        //                //arrPieTopAVal = getTopAForAday(branchIds),
        //                //arrPieTopBVal = getTopBForAday(branchIds),
        //                finalSaleForEach = getFinalSaleForEach(branchIds, ac.Id.ToString()),
        //                listAllAccounts = getAllAccountInSelectionList(branchIds),
        //                listAllMonths = getAllMonthList(),
        //                listAllYears = getAllYearList(),
        //                strAverage = tAverage,
        //                strOtherSale = String.Format("{0:n0}", convert_tOtherS),
        //                strInitMoney = tInitMoney,
        //                strOilIncome = tOil,
        //                strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs)),
        //                strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString(),
        //                strLoginName = userName,
        //                strVoucher = strDis,
        //                strCash = strCash,
        //                strCredit = strCredit
        //            };


        //            return View(hv);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        public ActionResult UrbanBeauty(string accountId, string monthNo, string yearNo, string cmd, int bid, string tab)
        {
            int branchIds = bid;
            int sellItemTypeId = 1;
            if (string.IsNullOrEmpty(tab))
            {
                sellItemTypeId = 1;
            }
            else if(tab.Equals("A"))
            {
                sellItemTypeId = 1;
            }
            else if (tab.Equals("B"))
            {
                sellItemTypeId = 2;
            }


            //Check if Log out button is clicked
            if (cmd != null)
            {

                //Remove cookie when log out
                RemoveCookie();
                return RedirectToAction("Index");
            }

            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;
                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                if (!string.IsNullOrEmpty(accountId))
                {
                    string tSales = " ";
                    string tPaxes = " ";
                    string tAverage = " ";
                    string tStaff = " ";
                    string topAname = " ";
                    string topBname = " ";
                    string tComs = " ";
                    string tOtherS = " ";
                    string tInitMoney = " ";
                    string tOil = " ";

                    string tSales_B = " ";
                    string tPaxes_B = " ";
                    string tAverage_B = " ";
                    string tStaff_B = " ";
                    string topAname_B = " ";
                    string topBname_B = " ";
                    string tComs_B = " ";
                    string tOtherS_B = " ";
                    string tInitMoney_B = " ";
                    string tOil_B = " ";

                    int accountIdInInteger = Int32.Parse(accountId); // Updated 11 October 2022

                    int sumDiscount = 0; // Updated 11 October 2022
                    int sumDiscount_B = 0;

                    List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022
                    List<DiscountRecord> listDiscount_B = new List<DiscountRecord>(); // Updated 11 October 2022

                    // Updated 11 October 2022
                    //using (var context = new spasystemdbEntities())
                    //{

                    //    listDiscount = context.DiscountRecords
                    //                    .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger && b.CancelStatus == "false")
                    //                    .OrderBy(b => b.Id)
                    //                    .ToList();
                    //}

                    //for (int m = 0; m < listDiscount.Count(); m++)
                    //{
                    //    sumDiscount += Int32.Parse(listDiscount[m].Value);
                    //}



                    /////////////////////////


                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";

                    if (sellItemTypeId == 1)
                    {
                        //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
                        sql = "SELECT SUM(dbo.OrderRecord.Price) AS 'Total Sale', COUNT(dbo.OrderRecord.Id) AS 'Total Pax', SUM(dbo.OrderRecord.Commission) AS 'Total Commission', (SUM(dbo.OrderRecord.Price) / COUNT(dbo.OrderRecord.Id)) AS 'Average', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountId + "' AND BranchId = '" + branchIds + "') AS 'Total Staff', (SELECT SUM(dbo.OtherSaleRecord.Price) FROM dbo.OtherSaleRecord WHERE dbo.OtherSaleRecord.BranchId = '" + branchIds + "' AND dbo.OtherSaleRecord.AccountId = '" + accountId + "' AND dbo.OtherSaleRecord.CancelStatus = 'false') AS 'Total Other Sale', (SELECT TOP 1 dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountId + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC) AS 'Top A', (SELECT dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountId + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) AS 'Top B', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountId + "' AND BranchId = '" + branchIds + "') * (SELECT dbo.SystemSetting.Value FROM dbo.SystemSetting WHERE BranchId = '" + branchIds + "' AND Name = 'OilPrice') AS 'Total Oil Income', (SELECT dbo.Account.StartMoney FROM dbo.Account WHERE Id = '" + accountId + "' AND BranchId = '" + branchIds + "') AS 'Initial Money' FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountId + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "';";

                        connetionString = ConfigurationManager.AppSettings["cString"];
                        cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        command = new SqlCommand(sql, cnn);

                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            tSales = String.Format("{0:n0}", dataReader.GetValue(0));
                            tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
                            tComs = String.Format("{0:n0}", dataReader.GetValue(2));
                            tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
                            tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
                            tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
                            topAname = dataReader.GetValue(6).ToString();
                            topBname = dataReader.GetValue(7).ToString();
                            tOil = String.Format("{0:n0}", dataReader.GetValue(8));
                            tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
                        }

                        dataReader.Close();
                        command.Dispose();
                        cnn.Close();

                        int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
                        string tSales_trim = tSales.Replace(",", "");
                        string tOil_trim = tOil.Replace(",", "");
                        string tOtherS_trim = tOtherS.Replace(",", "");
                        string tComs_trim = tComs.Replace(",", "");

                        if (string.IsNullOrEmpty(tSales_trim))
                        {

                        }
                        else
                        {
                            convert_tSales = Int32.Parse(tSales_trim);

                        }

                        if (string.IsNullOrEmpty(tOtherS_trim))
                        {

                        }
                        else
                        {
                            convert_tOtherS = Int32.Parse(tOtherS_trim);

                        }


                        if (string.IsNullOrEmpty(tOil_trim))
                        {

                        }
                        else
                        {
                            convert_tOil = Int32.Parse(tOil_trim);

                        }

                        if (string.IsNullOrEmpty(tComs_trim))
                        {

                        }
                        else
                        {
                            convert_tComs = Int32.Parse(tComs_trim);

                        }

                        sumDiscount = getSumDiscount_B(branchIds, accountIdInInteger, sellItemTypeId);
                        string strDis = String.Format("{0:n0}", sumDiscount);

                        var result = GetTotalCashAndCredit_multiAcc_B(branchIds, accountIdInInteger, sellItemTypeId);
                        int totalCash = result.TotalCash;
                        int totalCredit = result.TotalCredit;
                        String strCash = String.Format("{0:n0}", totalCash);
                        String strCredit = String.Format("{0:n0}", totalCredit);

                        HeaderWithBeauty hv = new HeaderWithBeauty()
                        {
                            strSales = tSales,
                            ////////////
                            //Waiting for confirm this has to be deduct discount 11 October 2022
                            //strSales = tSaleMinusDiscountInString,
                            ////////////
                            strPax = tPaxes,
                            strStaff = tStaff,
                            strCommission = tComs,
                            arrGraphVal = getOrderRecordForGraph_B(branchIds, Int32.Parse(accountId), sellItemTypeId),
                            strPieTopAName = topAname,
                            strPieTopBName = topBname,
                            //arrPieTopAVal = getTopAForAday(branchIds),
                            //arrPieTopBVal = getTopBForAday(branchIds),
                            finalSaleForEach = getFinalSaleForEach_B(branchIds, accountId, sellItemTypeId),
                            listAllAccounts = getAllAccountInSelectionList(branchIds),
                            listAllMonths = getAllMonthList(),
                            listAllYears = getAllYearList(),
                            strAverage = tAverage,
                            strOtherSale = String.Format("{0:n0}", 0),
                            strInitMoney = tInitMoney,
                            strOilIncome = tOil,
                            strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil) - convert_tComs)),
                            strVipCount = getTotalVipAmount_B(branchIds, Int32.Parse(accountId), sellItemTypeId).ToString(),
                            strLoginName = userName,
                            strVoucher = strDis,
                            strCash = strCash,
                            strCredit = strCredit,
                            bid = bid,
                            bName = getBranchName(bid),
                            accountId = accountId
                        };

                        //return View(hv);
                        if (Request.IsAjaxRequest())
                        {
                            // Return partial view for AJAX requests
                            return PartialView("_UrbanBeautyPartial", hv);
                        }
                        else
                        {
                            // Return full view for normal requests
                            return View("UrbanBeauty", hv);
                        }
                    }
                    else
                    {
                        sql = "SELECT SUM(dbo.OrderRecord.Price) AS 'Total Sale', COUNT(dbo.OrderRecord.Id) AS 'Total Pax', (SUM(dbo.OrderRecord.Commission) + (SELECT SUM(dbo.OtherSaleRecord.Commission) FROM dbo.OtherSaleRecord WHERE dbo.OtherSaleRecord.BranchId = '" + branchIds + "' AND dbo.OtherSaleRecord.AccountId = '" + accountId + "' AND dbo.OtherSaleRecord.CancelStatus = 'false')) AS 'Total Commission', (SUM(dbo.OrderRecord.Price) / COUNT(dbo.OrderRecord.Id)) AS 'Average', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountId + "' AND BranchId = '" + branchIds + "') AS 'Total Staff', (SELECT SUM(dbo.OtherSaleRecord.Price) FROM dbo.OtherSaleRecord WHERE dbo.OtherSaleRecord.BranchId = '" + branchIds + "' AND dbo.OtherSaleRecord.AccountId = '" + accountId + "' AND dbo.OtherSaleRecord.CancelStatus = 'false') AS 'Total Other Sale', (SELECT TOP 1 dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountId + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC) AS 'Top A', (SELECT dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountId + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) AS 'Top B', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountId + "' AND BranchId = '" + branchIds + "') * (SELECT dbo.SystemSetting.Value FROM dbo.SystemSetting WHERE BranchId = '" + branchIds + "' AND Name = 'OilPrice') AS 'Total Oil Income', (SELECT dbo.Account.StartMoney FROM dbo.Account WHERE Id = '" + accountId + "' AND BranchId = '" + branchIds + "') AS 'Initial Money' FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountId + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "';";

                        connetionString = ConfigurationManager.AppSettings["cString"];
                        cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        command = new SqlCommand(sql, cnn);

                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            tSales_B = String.Format("{0:n0}", dataReader.GetValue(0));
                            tPaxes_B = String.Format("{0:n0}", dataReader.GetValue(1));
                            tComs_B = String.Format("{0:n0}", dataReader.GetValue(2));
                            tAverage_B = String.Format("{0:n0}", dataReader.GetValue(3));
                            tStaff_B = String.Format("{0:n0}", dataReader.GetValue(4));
                            tOtherS_B = String.Format("{0:n0}", dataReader.GetValue(5));
                            topAname_B = dataReader.GetValue(6).ToString();
                            topBname_B = dataReader.GetValue(7).ToString();
                            tOil_B = String.Format("{0:n0}", dataReader.GetValue(8));
                            tInitMoney_B = String.Format("{0:n0}", dataReader.GetValue(9));
                        }

                        dataReader.Close();
                        command.Dispose();
                        cnn.Close();

                        int convert_tSales_B = 0, convert_tOil_B = 0, convert_tOtherS_B = 0, convert_tComs_B = 0;
                        string tSales_trim_B = tSales_B.Replace(",", "");
                        string tOil_trim_B = tOil_B.Replace(",", "");
                        string tOtherS_trim_B = tOtherS_B.Replace(",", "");
                        string tComs_trim_B = tComs_B.Replace(",", "");

                        if (string.IsNullOrEmpty(tSales_trim_B))
                        {

                        }
                        else
                        {
                            convert_tSales_B = Int32.Parse(tSales_trim_B);

                        }

                        if (string.IsNullOrEmpty(tOtherS_trim_B))
                        {

                        }
                        else
                        {
                            convert_tOtherS_B = Int32.Parse(tOtherS_trim_B);

                        }


                        if (string.IsNullOrEmpty(tOil_trim_B))
                        {

                        }
                        else
                        {
                            convert_tOil_B = Int32.Parse(tOil_trim_B);

                        }

                        if (string.IsNullOrEmpty(tComs_trim_B))
                        {

                        }
                        else
                        {
                            convert_tComs_B = Int32.Parse(tComs_trim_B);

                        }

                        sumDiscount_B = getSumDiscount_B(branchIds, accountIdInInteger, sellItemTypeId);
                        string strDis_B = String.Format("{0:n0}", sumDiscount_B);

                        var result_B = GetTotalCashAndCredit_multiAcc_B(branchIds, accountIdInInteger, sellItemTypeId);
                        int totalCash_B = result_B.TotalCash;
                        int totalCredit_B = result_B.TotalCredit;
                        String strCash_B = String.Format("{0:n0}", totalCash_B);
                        String strCredit_B = String.Format("{0:n0}", totalCredit_B);

                        //HeaderWithBeauty hv = new HeaderWithBeauty();
                        //hv.strSales_B = tSales_B;
                        //hv.strPax_B = tPaxes_B;
                        //hv.strStaff_B = "0";
                        //hv.strCommission_B = tComs_B;
                        //hv.arrGraphVal_B = getOrderRecordForGraph_B(branchIds, Int32.Parse(accountId), sellItemTypeId);
                        //hv.strPieTopAName_B = topAname_B;
                        //hv.strPieTopBName_B = topBname_B;
                        //hv.finalSaleForEach_B = getFinalSaleForEach_B(branchIds, accountId, sellItemTypeId);
                        //hv.strAverage_B = tAverage_B;
                        //hv.strOtherSale_B = String.Format("{0:n0}", convert_tOtherS_B);
                        //hv.strOilIncome_B = "0";
                        //hv.strBalanceNet_B = String.Format("{0:n0}", ((convert_tSales_B + convert_tOtherS_B) - convert_tComs_B));
                        //hv.strVipCount_B = getTotalVipAmount_B(branchIds, Int32.Parse(accountId), sellItemTypeId).ToString();
                        //hv.strVoucher_B = strDis_B;
                        //hv.strCash_B = strCash_B;
                        //hv.strCredit_B = strCredit_B;


                        HeaderWithBeauty hv = new HeaderWithBeauty()
                        {
                            strSales = tSales_B,
                            ////////////
                            //Waiting for confirm this has to be deduct discount 11 October 2022
                            //strSales = tSaleMinusDiscountInString,
                            ////////////
                            strPax = tPaxes_B,
                            strStaff = "0",
                            strCommission = tComs_B,
                            arrGraphVal = getOrderRecordForGraph_B(branchIds, Int32.Parse(accountId), sellItemTypeId),
                            strPieTopAName = topAname_B,
                            strPieTopBName = topBname_B,
                            //arrPieTopAVal = getTopAForAday(branchIds),
                            //arrPieTopBVal = getTopBForAday(branchIds),
                            finalSaleForEach = getFinalSaleForEach_B(branchIds, accountId, sellItemTypeId),
                            listAllAccounts = getAllAccountInSelectionList(branchIds),
                            listAllMonths = getAllMonthList(),
                            listAllYears = getAllYearList(),
                            strAverage = tAverage_B,
                            strOtherSale = String.Format("{0:n0}", convert_tOtherS_B),
                            strInitMoney = tInitMoney_B,
                            strOilIncome = "0",
                            strBalanceNet = String.Format("{0:n0}", ((convert_tSales_B + convert_tOtherS_B) - convert_tComs_B)),
                            strVipCount = getTotalVipAmount_B(branchIds, Int32.Parse(accountId), sellItemTypeId).ToString(),
                            strLoginName = userName,
                            strVoucher = strDis_B,
                            strCash = strCash_B,
                            strCredit = strCredit_B,
                            bid = bid,
                            bName = getBranchName(bid),
                            accountId = accountId
                        };

                        //return View(hv);
                        if (Request.IsAjaxRequest())
                        {
                            // Return partial view for AJAX requests
                            return PartialView("_UrbanBeautyPartial", hv);
                        }
                        else
                        {
                            // Return full view for normal requests
                            return View("UrbanBeauty", hv);
                        }

                    }
                    

                    
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);

                    List<Account> listAccountInMonth = new List<Account>();
                    List<OrderRecord> orderRecords = new List<OrderRecord>();
                    List<OtherSaleRecord> otherSaleRecords = new List<OtherSaleRecord>();
                    SystemSetting oilPriceSetting;
                    List<OrderRecord> orderRecordsVIP = new List<OrderRecord>();
                    List<DiscountRecord> discountRecords = new List<DiscountRecord>();

                    using (var context = new spasystemdbEntities())
                    {
                        // Get accounts for the specified month and year
                        listAccountInMonth = context.Accounts
                                                    .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                                    .ToList();

                        // Get all relevant order records for these accounts
                        var accountIds = listAccountInMonth.Select(a => a.Id).ToList();
                        orderRecords = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.CancelStatus == "false")
                                              .ToList();

                        // Get all relevant other sale records for these accounts
                        otherSaleRecords = context.OtherSaleRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.CancelStatus == "false")
                                                  .ToList();

                        // Get the oil price setting
                        oilPriceSetting = context.SystemSettings
                                                 .Where(ss => ss.BranchId == branchIds && ss.Name == "OilPrice")
                                                 .FirstOrDefault();

                        //Get vip count
                        orderRecordsVIP = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.CancelStatus == "false" && or.MemberId != 0)
                                              .ToList();

                        // Get all voucher or cash card discount
                        discountRecords = context.DiscountRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.CancelStatus == "false")
                                                  .ToList();
                    }

                    int oilPrice = Int32.Parse(oilPriceSetting?.Value ?? "0");
                    int tSales = 0, tPaxNum = 0, tComs = 0, tStaff = 0, tOtherS = 0, tInitMoney = 0, tOil = 0, tBalanceNet = 0, tVipCpunt = 0, tDiscount = 0, tCash = 0, tCredit = 0;

                    foreach (var account in listAccountInMonth)
                    {
                        var accountOrderRecords = orderRecords.Where(or => or.AccountId == account.Id).ToList();
                        var accountOtherSales = otherSaleRecords.Where(osr => osr.AccountId == account.Id).ToList();
                        var accountOrderRecordsVIP = orderRecordsVIP.Where(or => or.AccountId == account.Id).ToList();
                        var accountDiscount = discountRecords.Where(or => or.AccountId == account.Id).ToList();

                        tSales += accountOrderRecords.Sum(or => (int)or.Price);
                        tPaxNum += accountOrderRecords.Count();
                        tComs += accountOrderRecords.Sum(or => (int)or.Commission);
                        tStaff += (int)account.StaffAmount;
                        tOtherS += accountOtherSales.Sum(osr => (int)osr.Price);
                        tInitMoney += (int)account.StartMoney;
                        tVipCpunt += accountOrderRecordsVIP.Count();
                        tDiscount += accountDiscount.Sum(osr => int.Parse(osr.Value));
                        //tCash += getCash(branchIds, account.Id) - getVoucherCash(branchIds, account.Id);
                        //tCredit += getCredit(branchIds, account.Id) - getVoucherCredit(branchIds, account.Id);
                    }

                    tOil = tStaff * oilPrice;

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);
                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    var accountIds_ = listAccountInMonth.Select(a => a.Id).ToList();
                    var summary = GetTotalCashAndCredit_multiAcc(branchIds, accountIds_);
                    tCash = summary.TotalCash;
                    tCredit = summary.TotalCredit;


                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        //arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        //arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet),
                        strLoginName = userName,
                        strVipCount = tVipCpunt.ToString(), //new
                        strVoucher = String.Format("{0:n0}", tDiscount), //new
                        strCash = String.Format("{0:n0}", tCash), //new
                        strCredit = String.Format("{0:n0}", tCredit), //new
                        bid = bid,
                        bName = getBranchName(bid)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);

                    List<Account> listAccountInYear = new List<Account>();
                    List<OrderRecord> orderRecords = new List<OrderRecord>();
                    List<OtherSaleRecord> otherSaleRecords = new List<OtherSaleRecord>();
                    SystemSetting oilPriceSetting;
                    List<OrderRecord> orderRecordsVIP = new List<OrderRecord>();
                    List<DiscountRecord> discountRecords = new List<DiscountRecord>();

                    using (var context = new spasystemdbEntities())
                    {
                        // Get accounts for the specified year
                        listAccountInYear = context.Accounts
                                                   .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                                   .ToList();

                        // Get all relevant order records for these accounts within the year
                        var accountIds = listAccountInYear.Select(a => a.Id).ToList();
                        orderRecords = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.Date.Year == selectedYear && or.CancelStatus == "false")
                                              .ToList();

                        // Get all relevant other sale records for these accounts within the year
                        otherSaleRecords = context.OtherSaleRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.Date.Year == selectedYear && osr.CancelStatus == "false")
                                                  .ToList();

                        // Get the oil price setting
                        oilPriceSetting = context.SystemSettings
                                                 .Where(ss => ss.BranchId == branchIds && ss.Name == "OilPrice")
                                                 .FirstOrDefault();

                        //Get vip count
                        orderRecordsVIP = context.OrderRecords
                                              .Where(or => accountIds.Contains(or.AccountId) && or.BranchId == branchIds && or.CancelStatus == "false" && or.MemberId != 0)
                                              .ToList();

                        // Get all voucher or cash card discount
                        discountRecords = context.DiscountRecords
                                                  .Where(osr => accountIds.Contains(osr.AccountId) && osr.BranchId == branchIds && osr.CancelStatus == "false")
                                                  .ToList();
                    }

                    int oilPrice = Int32.Parse(oilPriceSetting?.Value ?? "0");
                    int tSales = 0, tPaxNum = 0, tComs = 0, tStaff = 0, tOtherS = 0, tInitMoney = 0, tOil = 0, tBalanceNet = 0, tVipCpunt = 0, tDiscount = 0, tCash = 0, tCredit = 0;

                    foreach (var account in listAccountInYear)
                    {
                        var accountOrderRecords = orderRecords.Where(or => or.AccountId == account.Id).ToList();
                        var accountOtherSales = otherSaleRecords.Where(osr => osr.AccountId == account.Id).ToList();
                        var accountOrderRecordsVIP = orderRecordsVIP.Where(or => or.AccountId == account.Id).ToList();
                        var accountDiscount = discountRecords.Where(or => or.AccountId == account.Id).ToList();

                        tSales += accountOrderRecords.Sum(or => (int)or.Price);
                        tPaxNum += accountOrderRecords.Count();
                        tComs += accountOrderRecords.Sum(or => (int)or.Commission);
                        tStaff += (int)account.StaffAmount;
                        tOtherS += accountOtherSales.Sum(osr => (int)osr.Price);
                        tInitMoney += (int)account.StartMoney;
                        tVipCpunt += accountOrderRecordsVIP.Count();
                        tDiscount += accountDiscount.Sum(osr => int.Parse(osr.Value));
                    }

                    tOil = tStaff * oilPrice;

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);
                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    var accountIds_ = listAccountInYear.Select(a => a.Id).ToList();
                    var summary = GetTotalCashAndCredit_multiAcc(branchIds, accountIds_);
                    tCash = summary.TotalCash;
                    tCredit = summary.TotalCredit;

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        //arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        //arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet),
                        strLoginName = userName,
                        strVipCount = tVipCpunt.ToString(), //new
                        strVoucher = String.Format("{0:n0}", tDiscount), //new
                        strCash = String.Format("{0:n0}", tCash), //new
                        strCredit = String.Format("{0:n0}", tCredit), //new
                        bid = bid,
                        bName = getBranchName(bid)
                    };

                    return View(hv);
                }
                else
                {

                    Account ac = getAccountValue(branchIds);

                    string tSales = " ";
                    string tPaxes = " ";
                    string tAverage = " ";
                    string tStaff = " ";
                    string topAname = " ";
                    string topBname = " ";
                    string tComs = " ";
                    string tOtherS = " ";
                    string tInitMoney = " ";
                    string tOil = " ";

                    string tSales_B = " ";
                    string tPaxes_B = " ";
                    string tAverage_B = " ";
                    string tStaff_B = " ";
                    string topAname_B = " ";
                    string topBname_B = " ";
                    string tComs_B = " ";
                    string tOtherS_B = " ";
                    string tInitMoney_B = " ";
                    string tOil_B = " ";

                    int accountIdInInteger = ac.Id; // Updated 11 October 2022

                    int sumDiscount = 0; // Updated 11 October 2022
                    int sumDiscount_B = 0;

                    List<DiscountRecord> listDiscount = new List<DiscountRecord>(); // Updated 11 October 2022
                    List<DiscountRecord> listDiscount_B = new List<DiscountRecord>(); // Updated 11 October 2022

                    // Updated 11 October 2022
                    //using (var context = new spasystemdbEntities())
                    //{

                    //    listDiscount = context.DiscountRecords
                    //                    .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger && b.CancelStatus == "false")
                    //                    .OrderBy(b => b.Id)
                    //                    .ToList();
                    //}

                    //for (int m = 0; m < listDiscount.Count(); m++)
                    //{
                    //    sumDiscount += Int32.Parse(listDiscount[m].Value);
                    //}



                    /////////////////////////


                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";

                    if (sellItemTypeId == 1)
                    {
                        //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '"+accountId+"' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '"+accountId+"' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '"+accountId+"' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '"+accountId+"' and dbo.OrderRecord.CancelStatus = 'false';";
                        sql = "SELECT SUM(dbo.OrderRecord.Price) AS 'Total Sale', COUNT(dbo.OrderRecord.Id) AS 'Total Pax', SUM(dbo.OrderRecord.Commission) AS 'Total Commission', (SUM(dbo.OrderRecord.Price) / COUNT(dbo.OrderRecord.Id)) AS 'Average', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountIdInInteger + "' AND BranchId = '" + branchIds + "') AS 'Total Staff', (SELECT SUM(dbo.OtherSaleRecord.Price) FROM dbo.OtherSaleRecord WHERE dbo.OtherSaleRecord.BranchId = '" + branchIds + "' AND dbo.OtherSaleRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OtherSaleRecord.CancelStatus = 'false') AS 'Total Other Sale', (SELECT TOP 1 dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC) AS 'Top A', (SELECT dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) AS 'Top B', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountIdInInteger + "' AND BranchId = '" + branchIds + "') * (SELECT dbo.SystemSetting.Value FROM dbo.SystemSetting WHERE BranchId = '" + branchIds + "' AND Name = 'OilPrice') AS 'Total Oil Income', (SELECT dbo.Account.StartMoney FROM dbo.Account WHERE Id = '" + accountIdInInteger + "' AND BranchId = '" + branchIds + "') AS 'Initial Money' FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "';";

                        connetionString = ConfigurationManager.AppSettings["cString"];
                        cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        command = new SqlCommand(sql, cnn);

                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            tSales = String.Format("{0:n0}", dataReader.GetValue(0));
                            tPaxes = String.Format("{0:n0}", dataReader.GetValue(1));
                            tComs = String.Format("{0:n0}", dataReader.GetValue(2));
                            tAverage = String.Format("{0:n0}", dataReader.GetValue(3));
                            tStaff = String.Format("{0:n0}", dataReader.GetValue(4));
                            tOtherS = String.Format("{0:n0}", dataReader.GetValue(5));
                            topAname = dataReader.GetValue(6).ToString();
                            topBname = dataReader.GetValue(7).ToString();
                            tOil = String.Format("{0:n0}", dataReader.GetValue(8));
                            tInitMoney = String.Format("{0:n0}", dataReader.GetValue(9));
                        }

                        dataReader.Close();
                        command.Dispose();
                        cnn.Close();

                        int convert_tSales = 0, convert_tOil = 0, convert_tOtherS = 0, convert_tComs = 0;
                        string tSales_trim = tSales.Replace(",", "");
                        string tOil_trim = tOil.Replace(",", "");
                        string tOtherS_trim = tOtherS.Replace(",", "");
                        string tComs_trim = tComs.Replace(",", "");

                        if (string.IsNullOrEmpty(tSales_trim))
                        {

                        }
                        else
                        {
                            convert_tSales = Int32.Parse(tSales_trim);

                        }

                        if (string.IsNullOrEmpty(tOtherS_trim))
                        {

                        }
                        else
                        {
                            convert_tOtherS = Int32.Parse(tOtherS_trim);

                        }


                        if (string.IsNullOrEmpty(tOil_trim))
                        {

                        }
                        else
                        {
                            convert_tOil = Int32.Parse(tOil_trim);

                        }

                        if (string.IsNullOrEmpty(tComs_trim))
                        {

                        }
                        else
                        {
                            convert_tComs = Int32.Parse(tComs_trim);

                        }

                        sumDiscount = getSumDiscount_B(branchIds, accountIdInInteger, sellItemTypeId);
                        string strDis = String.Format("{0:n0}", sumDiscount);

                        var result = GetTotalCashAndCredit_multiAcc_B(branchIds, accountIdInInteger, sellItemTypeId);
                        int totalCash = result.TotalCash;
                        int totalCredit = result.TotalCredit;
                        String strCash = String.Format("{0:n0}", totalCash);
                        String strCredit = String.Format("{0:n0}", totalCredit);

                        HeaderWithBeauty hv = new HeaderWithBeauty()
                        {
                            strSales = tSales,
                            ////////////
                            //Waiting for confirm this has to be deduct discount 11 October 2022
                            //strSales = tSaleMinusDiscountInString,
                            ////////////
                            strPax = tPaxes,
                            strStaff = tStaff,
                            strCommission = tComs,
                            arrGraphVal = getOrderRecordForGraph_B(branchIds, accountIdInInteger, sellItemTypeId),
                            strPieTopAName = topAname,
                            strPieTopBName = topBname,
                            //arrPieTopAVal = getTopAForAday(branchIds),
                            //arrPieTopBVal = getTopBForAday(branchIds),
                            finalSaleForEach = getFinalSaleForEach_B(branchIds, accountIdInInteger.ToString(), sellItemTypeId),
                            listAllAccounts = getAllAccountInSelectionList(branchIds),
                            listAllMonths = getAllMonthList(),
                            listAllYears = getAllYearList(),
                            strAverage = tAverage,
                            strOtherSale = String.Format("{0:n0}", 0),
                            strInitMoney = tInitMoney,
                            strOilIncome = tOil,
                            strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil) - convert_tComs)),
                            strVipCount = getTotalVipAmount_B(branchIds, accountIdInInteger, sellItemTypeId).ToString(),
                            strLoginName = userName,
                            strVoucher = strDis,
                            strCash = strCash,
                            strCredit = strCredit,
                            bid = bid,
                            bName = getBranchName(bid),
                            accountId = accountId
                        };

                        //return View(hv);
                        if (Request.IsAjaxRequest())
                        {
                            // Return partial view for AJAX requests
                            return PartialView("_UrbanBeautyPartial", hv);
                        }
                        else
                        {
                            // Return full view for normal requests
                            return View("UrbanBeauty", hv);
                        }
                    }
                    else
                    {
                        sql = "SELECT SUM(dbo.OrderRecord.Price) AS 'Total Sale', COUNT(dbo.OrderRecord.Id) AS 'Total Pax', (SUM(dbo.OrderRecord.Commission) + (SELECT SUM(dbo.OtherSaleRecord.Commission) FROM dbo.OtherSaleRecord WHERE dbo.OtherSaleRecord.BranchId = '" + branchIds + "' AND dbo.OtherSaleRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OtherSaleRecord.CancelStatus = 'false')) AS 'Total Commission', (SUM(dbo.OrderRecord.Price) / COUNT(dbo.OrderRecord.Id)) AS 'Average', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountIdInInteger + "' AND BranchId = '" + branchIds + "') AS 'Total Staff', (SELECT SUM(dbo.OtherSaleRecord.Price) FROM dbo.OtherSaleRecord WHERE dbo.OtherSaleRecord.BranchId = '" + branchIds + "' AND dbo.OtherSaleRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OtherSaleRecord.CancelStatus = 'false') AS 'Total Other Sale', (SELECT TOP 1 dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC) AS 'Top A', (SELECT dbo.MassageTopic.Name FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "' GROUP BY dbo.MassageTopic.Name ORDER BY COUNT(dbo.OrderRecord.MassageTopicId) DESC OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) AS 'Top B', (SELECT dbo.Account.StaffAmount FROM dbo.Account WHERE Id = '" + accountIdInInteger + "' AND BranchId = '" + branchIds + "') * (SELECT dbo.SystemSetting.Value FROM dbo.SystemSetting WHERE BranchId = '" + branchIds + "' AND Name = 'OilPrice') AS 'Total Oil Income', (SELECT dbo.Account.StartMoney FROM dbo.Account WHERE Id = '" + accountIdInInteger + "' AND BranchId = '" + branchIds + "') AS 'Initial Money' FROM dbo.OrderRecord LEFT JOIN dbo.MassageTopic ON dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id WHERE dbo.OrderRecord.BranchId = '" + branchIds + "' AND dbo.OrderRecord.AccountId = '" + accountIdInInteger + "' AND dbo.OrderRecord.CancelStatus = 'false' AND dbo.MassageTopic.SellItemTypeId = '" + sellItemTypeId + "';";

                        connetionString = ConfigurationManager.AppSettings["cString"];
                        cnn = new SqlConnection(connetionString);
                        cnn.Open();
                        command = new SqlCommand(sql, cnn);

                        dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            tSales_B = String.Format("{0:n0}", dataReader.GetValue(0));
                            tPaxes_B = String.Format("{0:n0}", dataReader.GetValue(1));
                            tComs_B = String.Format("{0:n0}", dataReader.GetValue(2));
                            tAverage_B = String.Format("{0:n0}", dataReader.GetValue(3));
                            tStaff_B = String.Format("{0:n0}", dataReader.GetValue(4));
                            tOtherS_B = String.Format("{0:n0}", dataReader.GetValue(5));
                            topAname_B = dataReader.GetValue(6).ToString();
                            topBname_B = dataReader.GetValue(7).ToString();
                            tOil_B = String.Format("{0:n0}", dataReader.GetValue(8));
                            tInitMoney_B = String.Format("{0:n0}", dataReader.GetValue(9));
                        }

                        dataReader.Close();
                        command.Dispose();
                        cnn.Close();

                        int convert_tSales_B = 0, convert_tOil_B = 0, convert_tOtherS_B = 0, convert_tComs_B = 0;
                        string tSales_trim_B = tSales_B.Replace(",", "");
                        string tOil_trim_B = tOil_B.Replace(",", "");
                        string tOtherS_trim_B = tOtherS_B.Replace(",", "");
                        string tComs_trim_B = tComs_B.Replace(",", "");

                        if (string.IsNullOrEmpty(tSales_trim_B))
                        {

                        }
                        else
                        {
                            convert_tSales_B = Int32.Parse(tSales_trim_B);

                        }

                        if (string.IsNullOrEmpty(tOtherS_trim_B))
                        {

                        }
                        else
                        {
                            convert_tOtherS_B = Int32.Parse(tOtherS_trim_B);

                        }


                        if (string.IsNullOrEmpty(tOil_trim_B))
                        {

                        }
                        else
                        {
                            convert_tOil_B = Int32.Parse(tOil_trim_B);

                        }

                        if (string.IsNullOrEmpty(tComs_trim_B))
                        {

                        }
                        else
                        {
                            convert_tComs_B = Int32.Parse(tComs_trim_B);

                        }

                        sumDiscount_B = getSumDiscount_B(branchIds, accountIdInInteger, sellItemTypeId);
                        string strDis_B = String.Format("{0:n0}", sumDiscount_B);

                        var result_B = GetTotalCashAndCredit_multiAcc_B(branchIds, accountIdInInteger, sellItemTypeId);
                        int totalCash_B = result_B.TotalCash;
                        int totalCredit_B = result_B.TotalCredit;
                        String strCash_B = String.Format("{0:n0}", totalCash_B);
                        String strCredit_B = String.Format("{0:n0}", totalCredit_B);

                        //HeaderWithBeauty hv = new HeaderWithBeauty();
                        //hv.strSales_B = tSales_B;
                        //hv.strPax_B = tPaxes_B;
                        //hv.strStaff_B = "0";
                        //hv.strCommission_B = tComs_B;
                        //hv.arrGraphVal_B = getOrderRecordForGraph_B(branchIds, Int32.Parse(accountId), sellItemTypeId);
                        //hv.strPieTopAName_B = topAname_B;
                        //hv.strPieTopBName_B = topBname_B;
                        //hv.finalSaleForEach_B = getFinalSaleForEach_B(branchIds, accountId, sellItemTypeId);
                        //hv.strAverage_B = tAverage_B;
                        //hv.strOtherSale_B = String.Format("{0:n0}", convert_tOtherS_B);
                        //hv.strOilIncome_B = "0";
                        //hv.strBalanceNet_B = String.Format("{0:n0}", ((convert_tSales_B + convert_tOtherS_B) - convert_tComs_B));
                        //hv.strVipCount_B = getTotalVipAmount_B(branchIds, Int32.Parse(accountId), sellItemTypeId).ToString();
                        //hv.strVoucher_B = strDis_B;
                        //hv.strCash_B = strCash_B;
                        //hv.strCredit_B = strCredit_B;


                        HeaderWithBeauty hv = new HeaderWithBeauty()
                        {
                            strSales = tSales_B,
                            ////////////
                            //Waiting for confirm this has to be deduct discount 11 October 2022
                            //strSales = tSaleMinusDiscountInString,
                            ////////////
                            strPax = tPaxes_B,
                            strStaff = "0",
                            strCommission = tComs_B,
                            arrGraphVal = getOrderRecordForGraph_B(branchIds, accountIdInInteger, sellItemTypeId),
                            strPieTopAName = topAname_B,
                            strPieTopBName = topBname_B,
                            //arrPieTopAVal = getTopAForAday(branchIds),
                            //arrPieTopBVal = getTopBForAday(branchIds),
                            finalSaleForEach = getFinalSaleForEach_B(branchIds, accountIdInInteger.ToString(), sellItemTypeId),
                            listAllAccounts = getAllAccountInSelectionList(branchIds),
                            listAllMonths = getAllMonthList(),
                            listAllYears = getAllYearList(),
                            strAverage = tAverage_B,
                            strOtherSale = String.Format("{0:n0}", convert_tOtherS_B),
                            strInitMoney = tInitMoney_B,
                            strOilIncome = "0",
                            strBalanceNet = String.Format("{0:n0}", ((convert_tSales_B + convert_tOtherS_B) - convert_tComs_B)),
                            strVipCount = getTotalVipAmount_B(branchIds, accountIdInInteger, sellItemTypeId).ToString(),
                            strLoginName = userName,
                            strVoucher = strDis_B,
                            strCash = strCash_B,
                            strCredit = strCredit_B,
                            bid = bid,
                            bName = getBranchName(bid),
                            accountId = accountId
                        };

                        //return View(hv);
                        if (Request.IsAjaxRequest())
                        {
                            // Return partial view for AJAX requests
                            return PartialView("_UrbanBeautyPartial", hv);
                        }
                        else
                        {
                            // Return full view for normal requests
                            return View("UrbanBeauty", hv);
                        }

                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Member(string accountId, string monthNo, string yearNo, string cmd)
        {
            
            //Check lout out button
            //No log out button in this page
            //if (cmd != null)
            //{
            //    foreach (var element in System.Runtime.Caching.MemoryCache.Default)
            //    {
            //        System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
            //    }
            //}

            //var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            //if (noms == null)
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    List<Member> listMem = new List<Member>();

            //    using (var context = new spasystemdbEntities())
            //    {

            //        listMem = context.Members
            //                        .OrderBy(b => b.Id)
            //                        .ToList();
            //    }

            //    List<MemberItem> listMemForView = new List<MemberItem>();

            //    foreach(Member mem in listMem)
            //    {
                    
            //        string[] splitStart = getMemberDetail(mem.Id).StartDate.ToString().Split(' ');
            //        string[] splitExpire = getMemberDetail(mem.Id).ExpireDate.ToString().Split(' ');

            //        MemberItem memItem = new MemberItem() { Id=mem.Id.ToString(), MemberNo = mem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(mem.Id).MemberGroupId).ShowName, Title = mem.Title, FirstName = mem.FirstName, FamilyName = mem.FamilyName, AddressInTH = mem.AddressInTH, City = mem.City, TelephoneNo = mem.TelephoneNo, WhatsAppId = mem.WhatsAppId, LineId = mem.LineId, CreateDate = mem.CreateDateTime.ToString(), VipStart = splitStart[0], VipExpire= splitExpire[0] };
            //        if(mem.ActiveStatus.Equals("true"))
            //        {
            //            memItem.Status = "Active";
            //        }
            //        else
            //        {
            //            memItem.Status = "Inactive";
            //        }

            //        if(!string.IsNullOrEmpty(mem.Birth.ToString()))
            //        {
            //            string[] splitBirth = mem.Birth.ToString().Split(' ');
            //            memItem.Birth = splitBirth[0];
            //        }
                    
            //        listMemForView.Add(memItem);
            //    };

            //    HeaderValueVIP hv = new HeaderValueVIP()
            //    {
            //        MemberList = listMemForView
            //    };


            //    return View(hv);
            //}


            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;
                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                List<Member> listMem = new List<Member>();

                using (var context = new spasystemdbEntities())
                {

                    listMem = context.Members
                                    .OrderBy(b => b.Id)
                                    .ToList();
                }

                List<MemberItem> listMemForView = new List<MemberItem>();

                foreach (Member mem in listMem)
                {

                    string[] splitStart = getMemberDetail(mem.Id).StartDate.ToString().Split(' ');
                    string[] splitExpire = getMemberDetail(mem.Id).ExpireDate.ToString().Split(' ');

                    MemberItem memItem = new MemberItem() { Id = mem.Id.ToString(), MemberNo = mem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(mem.Id).MemberGroupId).ShowName, Title = mem.Title, FirstName = mem.FirstName, FamilyName = mem.FamilyName, AddressInTH = mem.AddressInTH, City = mem.City, TelephoneNo = mem.TelephoneNo, WhatsAppId = mem.WhatsAppId, LineId = mem.LineId, CreateDate = mem.CreateDateTime.ToString(), VipStart = splitStart[0], VipExpire = splitExpire[0] };
                    if (mem.ActiveStatus.Equals("true"))
                    {
                        memItem.Status = "Active";
                    }
                    else
                    {
                        memItem.Status = "Inactive";
                    }

                    if (!string.IsNullOrEmpty(mem.Birth.ToString()))
                    {
                        string[] splitBirth = mem.Birth.ToString().Split(' ');
                        memItem.Birth = splitBirth[0];
                    }

                    listMemForView.Add(memItem);
                };

                HeaderValueVIP hv = new HeaderValueVIP()
                {
                    MemberList = listMemForView,
                    strLoginName = userName
                };


                return View(hv);

            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult ManageMemberType(string accountId, string monthNo, string yearNo, string cmd)
        {
            //Check lout out button
            //No log out button in this page
            //if (cmd != null)
            //{
            //    foreach (var element in System.Runtime.Caching.MemoryCache.Default)
            //    {
            //        System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
            //    }
            //}

            //var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            //if (noms == null)
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    List<MemberGroup> listMemG = new List<MemberGroup>();

            //    using (var context = new spasystemdbEntities())
            //    {

            //        listMemG = context.MemberGroups
            //                        .OrderBy(b => b.Id)
            //                        .ToList();
            //    }

            //    List<MemberGroupItem> listMemGForView = new List<MemberGroupItem>();

            //    foreach (MemberGroup memG in listMemG)
            //    {
            //        MemberGroupItem memGItem = new MemberGroupItem();
            //        memGItem.MemberGroupId = memG.Id.ToString();
            //        memGItem.MemberGroupName = memG.Name;
            //        memGItem.MemberGroupShowName = memG.ShowName;
            //        if (memG.Status.Equals("true"))
            //        {
            //            memGItem.Status = "Active";
            //        }
            //        else
            //        {
            //            memGItem.Status = "Inactive";
            //        }

            //        if(getMemberGroupPriviledge(memG.Id) != null && getMemberGroupPriviledge(memG.Id).Any())
            //        {
            //            memGItem.MemberGroupPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].Id.ToString();
            //            memGItem.MemberPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId.ToString();
            //            memGItem.MemberPriviledgeName = getMemberPriviledgeDetail(getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId).ShowName;
            //        }
            //        //else
            //        //{
            //        //    memGItem.MemberGroupPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].Id.ToString();
            //        //    memGItem.MemberPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId.ToString();
            //        //    memGItem.MemberPriviledgeName = getMemberPriviledgeDetail(getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId).ShowName;
            //        //}

            //        listMemGForView.Add(memGItem);
            //    };

            //    HeaderValueVIPGroup hvg = new HeaderValueVIPGroup()
            //    {
            //        MemberGroupList = listMemGForView
            //    };


            //    return View(hvg);
            //}

            //Check user token
            //Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;
                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                List<MemberGroup> listMemG = new List<MemberGroup>();

                using (var context = new spasystemdbEntities())
                {

                    listMemG = context.MemberGroups
                                    .OrderBy(b => b.Id)
                                    .ToList();
                }

                List<MemberGroupItem> listMemGForView = new List<MemberGroupItem>();

                foreach (MemberGroup memG in listMemG)
                {
                    MemberGroupItem memGItem = new MemberGroupItem();
                    memGItem.MemberGroupId = memG.Id.ToString();
                    memGItem.MemberGroupName = memG.Name;
                    memGItem.MemberGroupShowName = memG.ShowName;
                    if (memG.Status.Equals("true"))
                    {
                        memGItem.Status = "Active";
                    }
                    else
                    {
                        memGItem.Status = "Inactive";
                    }

                    if (getMemberGroupPriviledge(memG.Id) != null && getMemberGroupPriviledge(memG.Id).Any())
                    {
                        memGItem.MemberGroupPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].Id.ToString();
                        memGItem.MemberPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId.ToString();
                        memGItem.MemberPriviledgeName = getMemberPriviledgeDetail(getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId).ShowName;
                    }
                    //else
                    //{
                    //    memGItem.MemberGroupPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].Id.ToString();
                    //    memGItem.MemberPriviledgeId = getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId.ToString();
                    //    memGItem.MemberPriviledgeName = getMemberPriviledgeDetail(getMemberGroupPriviledge(memG.Id)[0].MemberPriviledgeId).ShowName;
                    //}

                    listMemGForView.Add(memGItem);
                };

                HeaderValueVIPGroup hvg = new HeaderValueVIPGroup()
                {
                    MemberGroupList = listMemGForView,
                    strLoginName = userName
                };


                return View(hvg);

            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult ManagePriviledge(string accountId, string monthNo, string yearNo, string cmd)
        {
            //Check lout out button
            //No log out button in this page
            //if (cmd != null)
            //{
            //    foreach (var element in System.Runtime.Caching.MemoryCache.Default)
            //    {
            //        System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
            //    }
            //}

            //var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            //if (noms == null)
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    List<MemberPriviledge> listMemPriv = getAllMemberPriviledge();

            //    List<MemberPriviledgeItem> listMemPrivForView = new List<MemberPriviledgeItem>();

            //    foreach (MemberPriviledge memP in listMemPriv)
            //    {
            //        MemberPriviledgeItem memPrivItem = new MemberPriviledgeItem();
            //        memPrivItem.Id = memP.Id.ToString();
            //        memPrivItem.ShowName = memP.ShowName;
            //        memPrivItem.PriviledgeTypeId = memP.PriviledgeTypeId.ToString();
            //        memPrivItem.PriviledgeTypeName = getPriviledgeTypeDetail(memP.PriviledgeTypeId).Name;
            //        memPrivItem.Value = memP.Value.ToString();
            //        memPrivItem.StartDate = memP.StartDate.ToString();
            //        memPrivItem.ExpireDate = memP.ExpireDate.ToString();

            //        if (memP.Status.Equals("true"))
            //        {
            //            memPrivItem.Status = "Active";
            //        }
            //        else
            //        {
            //            memPrivItem.Status = "Inactive";
            //        }

            //        listMemPrivForView.Add(memPrivItem);
            //    };

            //    HeaderValueVIPPriv hvp = new HeaderValueVIPPriv()
            //    {
            //        MemberPriviledgeList = listMemPrivForView
            //    };


            //    return View(hvp);
            //}


            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;
                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                List<MemberPriviledge> listMemPriv = getAllMemberPriviledge();

                List<MemberPriviledgeItem> listMemPrivForView = new List<MemberPriviledgeItem>();

                foreach (MemberPriviledge memP in listMemPriv)
                {
                    MemberPriviledgeItem memPrivItem = new MemberPriviledgeItem();
                    memPrivItem.Id = memP.Id.ToString();
                    memPrivItem.ShowName = memP.ShowName;
                    memPrivItem.PriviledgeTypeId = memP.PriviledgeTypeId.ToString();
                    memPrivItem.PriviledgeTypeName = getPriviledgeTypeDetail(memP.PriviledgeTypeId).Name;
                    memPrivItem.Value = memP.Value.ToString();
                    memPrivItem.StartDate = memP.StartDate.ToString();
                    memPrivItem.ExpireDate = memP.ExpireDate.ToString();

                    if (memP.Status.Equals("true"))
                    {
                        memPrivItem.Status = "Active";
                    }
                    else
                    {
                        memPrivItem.Status = "Inactive";
                    }

                    listMemPrivForView.Add(memPrivItem);
                };

                HeaderValueVIPPriv hvp = new HeaderValueVIPPriv()
                {
                    MemberPriviledgeList = listMemPrivForView,
                    strLoginName = userName
                };


                return View(hvp);

            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult MemberDetail(string accountId, string cmd, string MemberId)
        {
            //Check lout out button
            //No log out button in this page
            //if (cmd != null)
            //{
            //    foreach (var element in System.Runtime.Caching.MemoryCache.Default)
            //    {
            //        System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
            //    }
            //}

            //var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            //if (noms == null)
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    Member myMem = getMember(Int32.Parse(MemberId));
            //    string[] splitStart = getMemberDetail(myMem.Id).StartDate.ToString().Split(' ');
            //    string[] splitExpire = getMemberDetail(myMem.Id).ExpireDate.ToString().Split(' ');

            //    List<MemberItem> listMemForView = new List<MemberItem>();

            //    MemberItem memItem = new MemberItem() { Id = myMem.Id.ToString(), MemberNo = myMem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(myMem.Id).MemberGroupId).ShowName, Title = myMem.Title, FirstName = myMem.FirstName, FamilyName = myMem.FamilyName, AddressInTH = myMem.AddressInTH, City = myMem.City, TelephoneNo = myMem.TelephoneNo, WhatsAppId = myMem.WhatsAppId, LineId = myMem.LineId, CreateDate = myMem.CreateDateTime.ToString(), VipStart = splitStart[0], VipExpire = splitExpire[0] };
            //    if (myMem.ActiveStatus.Equals("true"))
            //    {
            //        memItem.Status = "Active";
            //    }
            //    else
            //    {
            //        memItem.Status = "Inactive";
            //    }

            //    if (!string.IsNullOrEmpty(myMem.Birth.ToString()))
            //    {
            //        string[] splitBirth = myMem.Birth.ToString().Split(' ');
            //        string[] splitBirthInEach = splitBirth[0].ToString().Split('/');
            //        memItem.Birth = splitBirthInEach[1]+"/"+ splitBirthInEach[0]+"/"+ splitBirthInEach[2];
            //    }
                

            //    listMemForView.Add(memItem);

            //    HeaderValueVIP hv = new HeaderValueVIP()
            //    {
            //        MemberList = listMemForView
            //    };


            //    return View(hv);
            //}

            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;
                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                Member myMem = getMember(Int32.Parse(MemberId));
                string[] splitStart = getMemberDetail(myMem.Id).StartDate.ToString().Split(' ');
                string[] splitExpire = getMemberDetail(myMem.Id).ExpireDate.ToString().Split(' ');

                List<MemberItem> listMemForView = new List<MemberItem>();

                MemberItem memItem = new MemberItem() { Id = myMem.Id.ToString(), MemberNo = myMem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(myMem.Id).MemberGroupId).ShowName, Title = myMem.Title, FirstName = myMem.FirstName, FamilyName = myMem.FamilyName, AddressInTH = myMem.AddressInTH, City = myMem.City, TelephoneNo = myMem.TelephoneNo, WhatsAppId = myMem.WhatsAppId, LineId = myMem.LineId, CreateDate = myMem.CreateDateTime.ToString(), VipStart = splitStart[0], VipExpire = splitExpire[0] };
                if (myMem.ActiveStatus.Equals("true"))
                {
                    memItem.Status = "Active";
                }
                else
                {
                    memItem.Status = "Inactive";
                }

                if (!string.IsNullOrEmpty(myMem.Birth.ToString()))
                {
                    string[] splitBirth = myMem.Birth.ToString().Split(' ');
                    string[] splitBirthInEach = splitBirth[0].ToString().Split('/');
                    memItem.Birth = splitBirthInEach[1] + "/" + splitBirthInEach[0] + "/" + splitBirthInEach[2];
                }


                listMemForView.Add(memItem);

                HeaderValueVIP hv = new HeaderValueVIP()
                {
                    MemberList = listMemForView,
                    strLoginName = userName
                };


                return View(hv);

            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public string SaveMember(ForInsertMember myData)
        {
            if(myData.pageMode.Equals("New"))
            {
                if (string.IsNullOrEmpty(myData.memberNoVal) || string.IsNullOrEmpty(myData.titleVal) || string.IsNullOrEmpty(myData.firstNameVal) || string.IsNullOrEmpty(myData.familyNameVal) || string.IsNullOrEmpty(myData.vipTypeVal) || string.IsNullOrEmpty(myData.startDayVal) || string.IsNullOrEmpty(myData.expireDayVal) || string.IsNullOrEmpty(myData.statusVal))
                {
                    return "NoReq";
                }
                else
                {
                    using (var db = new spasystemdbEntities())
                    {
                        Member newMem = new Member()
                        {
                            MemberNo = myData.memberNoVal,
                            Title = myData.titleVal,
                            FirstName = myData.firstNameVal,
                            FamilyName = myData.familyNameVal,
                            AddressInTH = myData.addressVal,
                            City = myData.cityVal,
                            TelephoneNo = myData.telephoneVal,
                            WhatsAppId = myData.whatsappVal,
                            LineId = myData.lineVal,
                            CreatedBy = "Admin",
                            CreateDateTime = DateTime.Now
                        };
                        if (myData.statusVal.Equals("Active"))
                        {
                            newMem.ActiveStatus = "true";
                        }
                        else
                        {
                            newMem.ActiveStatus = "false";
                        }

                        if(!string.IsNullOrEmpty(myData.birthDayVal))
                        {
                            string fullBirthday = myData.birthMonthVal + "/" + myData.birthDayVal + "/" + myData.birthYearVal;
                            newMem.Birth = DateTime.Parse(fullBirthday);
                        }
                        

                        db.Members.Add(newMem);
                        db.SaveChanges();

                        string fullVipStart = myData.startMonthVal + "/" + myData.startDayVal + "/" + myData.startYearVal;
                        string fullVipExpire = myData.expireMonthVal + "/" + myData.expireDayVal + "/" + myData.expireYearVal;

                        MemberDetail newMemDetail = new MemberDetail()
                        {
                            MemberId = getLastestMember().Id,
                            MemberGroupId = Int32.Parse(myData.vipTypeVal),
                            StartDate = DateTime.Parse(fullVipStart),
                            ExpireDate = DateTime.Parse(fullVipExpire),
                            Status = "true",
                            CreateDateTime = DateTime.Now,
                            CreatedBy = "Admin"
                        };
                        db.MemberDetails.Add(newMemDetail);
                        db.SaveChanges();

                        return "Success";
                    }
                }
                
            }
            else
            {
                if (string.IsNullOrEmpty(myData.memberNoVal) || string.IsNullOrEmpty(myData.titleVal) || string.IsNullOrEmpty(myData.firstNameVal) || string.IsNullOrEmpty(myData.familyNameVal) || string.IsNullOrEmpty(myData.vipTypeVal) || string.IsNullOrEmpty(myData.startDayVal) || string.IsNullOrEmpty(myData.expireDayVal) || string.IsNullOrEmpty(myData.statusVal))
                {
                    return "NoReq";
                }
                else
                {
                    int usedMemId = Int32.Parse(myData.Id);
                    using (var db = new spasystemdbEntities())
                    {
                        Member curMem = db.Members
                                .Where(b => b.Id == usedMemId)
                                .FirstOrDefault();

                        //Member curMem = getMember(Int32.Parse(myData.Id));
                        curMem.MemberNo = myData.memberNoVal;
                        curMem.Title = myData.titleVal;
                        curMem.FirstName = myData.firstNameVal;
                        curMem.FamilyName = myData.familyNameVal;

                        if (!string.IsNullOrEmpty(myData.birthDayVal))
                        {
                            string fullBirthday = myData.birthMonthVal + "/" + myData.birthDayVal + "/" + myData.birthYearVal;
                            curMem.Birth = DateTime.Parse(fullBirthday);
                        }
                        
                        curMem.AddressInTH = myData.addressVal;
                        curMem.City = myData.cityVal;
                        curMem.TelephoneNo = myData.telephoneVal;
                        curMem.WhatsAppId = myData.whatsappVal;
                        curMem.LineId = myData.lineVal;
                        curMem.UpdatedBy = "Admin";
                        curMem.UpdateDateTime = DateTime.Now;

                        if (myData.statusVal.Equals("Active"))
                        {
                            curMem.ActiveStatus = "true";
                        }
                        else
                        {
                            curMem.ActiveStatus = "false";
                        }

                        string fullVipStart = myData.startMonthVal + "/" + myData.startDayVal + "/" + myData.startYearVal;
                        string fullVipExpire = myData.expireMonthVal + "/" + myData.expireDayVal + "/" + myData.expireYearVal;

                        //db.SaveChanges();
                        MemberDetail curMemDetail = db.MemberDetails
                                            .Where(b => b.MemberId == usedMemId)
                                            .FirstOrDefault();
                        //MemberDetail curMemDetail = getMemberDetail(Int32.Parse(myData.Id));
                        curMemDetail.MemberGroupId = Int32.Parse(myData.vipTypeVal);
                        curMemDetail.StartDate = DateTime.Parse(fullVipStart);
                        curMemDetail.ExpireDate = DateTime.Parse(fullVipExpire);
                        //curMemDetail.Status = "true";
                        curMemDetail.UpdateDateTime = DateTime.Now;
                        curMemDetail.UpdatedBy = "Admin";

                        //db.MemberDetails.Add(newMemDetail);
                        db.SaveChanges();

                        return "Success";
                    }
                }
                
            }
            
        }
        [HttpPost]
        public string DeleteMember(string id)
        {
            
            
                if (string.IsNullOrEmpty(id))
                {
                    return "IdNull";
                }
                else
                {
                    int usedMemId = Int32.Parse(id);
                    using (var db = new spasystemdbEntities())
                    {
                        MemberDetail curMemDetail = db.MemberDetails
                                                .Where(b => b.MemberId == usedMemId)
                                                .FirstOrDefault();

                        db.MemberDetails.Remove(curMemDetail);
                        db.SaveChanges();

                        // Decrease the auto-increment seed
                        //var tableName = db.Model.GetEntityTypes().First().Relational().TableName;
                        //var command = $"DBCC CHECKIDENT ('Member', RESEED, {id - 1});";
                        //db.Database.ExecuteSqlRaw(command);

                    Member curMem = db.Members
                                    .Where(b => b.Id == usedMemId)
                                    .FirstOrDefault();

                        db.Members.Remove(curMem);
                        db.SaveChanges();

                            return "Success";
                    }
                }

            
        }

        [HttpPost]
        public ActionResult UpdateMemberTable(string selectedOption)
        {
            //Console.WriteLine("print from c#"+selectedOption);
            List<Member> listMem = new List<Member>();

            if(selectedOption.Equals("1"))
            {
                using (var context = new spasystemdbEntities())
                {

                    listMem = context.Members
                                    .OrderBy(b => b.Id)
                                    .ToList();
                }
            }
            else if(selectedOption.Equals("2"))
            {
                using (var context = new spasystemdbEntities())
                {

                    listMem = context.Members
                                    .Where(b => b.ActiveStatus == "true")
                                    .OrderBy(b => b.Id)
                                    .ToList();
                }
            }
            else
            {
                using (var context = new spasystemdbEntities())
                {

                    listMem = context.Members
                                    .Where(b => b.ActiveStatus == "false")
                                    .OrderBy(b => b.Id)
                                    .ToList();
                }
            }

            List<MemberItem> listMemForView = new List<MemberItem>();

            foreach (Member mem in listMem)
            {
                
                string[] splitStart = getMemberDetail(mem.Id).StartDate.ToString().Split(' ');
                string[] splitExpire = getMemberDetail(mem.Id).ExpireDate.ToString().Split(' ');

                MemberItem memItem = new MemberItem() { Id = mem.Id.ToString(), MemberNo = mem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(mem.Id).MemberGroupId).ShowName, Title = mem.Title, FirstName = mem.FirstName, FamilyName = mem.FamilyName, AddressInTH = mem.AddressInTH, City = mem.City, TelephoneNo = mem.TelephoneNo, WhatsAppId = mem.WhatsAppId, LineId = mem.LineId, CreateDate = mem.CreateDateTime.ToString(), VipStart = splitStart[0], VipExpire = splitExpire[0] };
                if (mem.ActiveStatus.Equals("true"))
                {
                    memItem.Status = "Active";
                }
                else
                {
                    memItem.Status = "Inactive";
                }

                if(!string.IsNullOrEmpty(mem.Birth.ToString()))
                {
                    string[] splitBirth = mem.Birth.ToString().Split(' ');
                    memItem.Birth = splitBirth[0];
                }

                listMemForView.Add(memItem);
            };

            HeaderValueVIP hv = new HeaderValueVIP()
            {
                MemberList = listMemForView
            };


            return View("Member",hv);
        }
        public ActionResult EditMemberDetail(string accountId, string cmd, string MemberId, string Mode)
        {
            //Check lout out button
            //No log out button in this page
            //if (cmd != null)
            //{
            //    foreach (var element in System.Runtime.Caching.MemoryCache.Default)
            //    {
            //        System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
            //    }
            //}

            //var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            //if (noms == null)
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    if(Mode.Equals("Edit"))
            //    {
            //        Member myMem = getMember(Int32.Parse(MemberId));
                    
            //        string[] splitStart = getMemberDetail(myMem.Id).StartDate.ToString().Split(' ');
            //        string[] splitExpire = getMemberDetail(myMem.Id).ExpireDate.ToString().Split(' ');

                    
            //        string[] splitVipStartPart = splitStart[0].Split('/');
            //        string[] splitVipExpirePart = splitExpire[0].Split('/');

            //        List<MemberItem> listMemForView = new List<MemberItem>();

            //        MemberItem memItem = new MemberItem() { Id = myMem.Id.ToString(), MemberNo = myMem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(myMem.Id).MemberGroupId).ShowName, Title = myMem.Title, FirstName = myMem.FirstName, FamilyName = myMem.FamilyName, AddressInTH = myMem.AddressInTH, City = myMem.City, TelephoneNo = myMem.TelephoneNo, WhatsAppId = myMem.WhatsAppId, LineId = myMem.LineId, CreateDate = myMem.CreateDateTime.ToString(),VipStart = splitStart[0], VipExpire = splitExpire[0],VipStartDay=splitVipStartPart[1],VipStartMonth=splitVipStartPart[0],VipStartYear=splitVipStartPart[2],VipExpireDay=splitVipExpirePart[1],VipExpireMonth=splitVipExpirePart[0],VipExpireYear=splitVipExpirePart[2] };
            //        if (myMem.ActiveStatus.Equals("true"))
            //        {
            //            memItem.Status = "Active";
            //        }
            //        else
            //        {
            //            memItem.Status = "Inactive";
            //        }

            //        if(!string.IsNullOrEmpty(myMem.Birth.ToString()))
            //        {
            //            string[] splitBirth = myMem.Birth.ToString().Split(' ');
            //            string[] splitBirthPart = splitBirth[0].Split('/');
            //            memItem.Birth = splitBirth[0];
            //            memItem.BirthDay = splitBirthPart[1];
            //            memItem.BirthMonth = splitBirthPart[0];
            //            memItem.BirthYear = splitBirthPart[2];
            //        }

            //        memItem.MemberGroupForSelect = getAllMemberGroup();
            //        memItem.MemberGroupId = getMemberDetail(myMem.Id).MemberGroupId.ToString();
            //        memItem.PageMode = Mode;

            //        listMemForView.Add(memItem);

            //        HeaderValueVIP hv = new HeaderValueVIP()
            //        {
            //            MemberList = listMemForView
            //        };


            //        return View(hv);
            //    }
            //    else
            //    {
            //        List<MemberItem> listMemForView = new List<MemberItem>();

            //        MemberItem memItem = new MemberItem();
            //        memItem.MemberGroupForSelect = getAllMemberGroup();
            //        memItem.PageMode = Mode;

            //        listMemForView.Add(memItem);

            //        HeaderValueVIP hv = new HeaderValueVIP()
            //        {
            //            MemberList = listMemForView
            //        };


            //        return View(hv);
            //    }
            //}

            //Check user token
            // Retrieve the cookie from the request
            HttpCookie cookie = Request.Cookies["TokenCookie"];
            HttpCookie cookie_user = Request.Cookies["UserCookie"];

            string tokenValue = null;
            string userName = null;

            //Check user token from cookie
            if (cookie != null)
            {
                tokenValue = cookie.Value;

                //Check user name from cookie
                if (cookie_user != null)
                {
                    userName = cookie_user.Value;
                }
                else
                {
                    userName = "Annonymous";
                }

                //Prepare content for View
                if (Mode.Equals("Edit"))
                {
                    Member myMem = getMember(Int32.Parse(MemberId));

                    string[] splitStart = getMemberDetail(myMem.Id).StartDate.ToString().Split(' ');
                    string[] splitExpire = getMemberDetail(myMem.Id).ExpireDate.ToString().Split(' ');


                    string[] splitVipStartPart = splitStart[0].Split('/');
                    string[] splitVipExpirePart = splitExpire[0].Split('/');

                    List<MemberItem> listMemForView = new List<MemberItem>();

                    MemberItem memItem = new MemberItem() { Id = myMem.Id.ToString(), MemberNo = myMem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(myMem.Id).MemberGroupId).ShowName, Title = myMem.Title, FirstName = myMem.FirstName, FamilyName = myMem.FamilyName, AddressInTH = myMem.AddressInTH, City = myMem.City, TelephoneNo = myMem.TelephoneNo, WhatsAppId = myMem.WhatsAppId, LineId = myMem.LineId, CreateDate = myMem.CreateDateTime.ToString(), VipStart = splitStart[0], VipExpire = splitExpire[0], VipStartDay = splitVipStartPart[1], VipStartMonth = splitVipStartPart[0], VipStartYear = splitVipStartPart[2], VipExpireDay = splitVipExpirePart[1], VipExpireMonth = splitVipExpirePart[0], VipExpireYear = splitVipExpirePart[2] };
                    if (myMem.ActiveStatus.Equals("true"))
                    {
                        memItem.Status = "Active";
                    }
                    else
                    {
                        memItem.Status = "Inactive";
                    }

                    if (!string.IsNullOrEmpty(myMem.Birth.ToString()))
                    {
                        string[] splitBirth = myMem.Birth.ToString().Split(' ');
                        string[] splitBirthPart = splitBirth[0].Split('/');
                        memItem.Birth = splitBirth[0];
                        memItem.BirthDay = splitBirthPart[1];
                        memItem.BirthMonth = splitBirthPart[0];
                        memItem.BirthYear = splitBirthPart[2];
                    }

                    memItem.MemberGroupForSelect = getAllMemberGroup();
                    memItem.MemberGroupId = getMemberDetail(myMem.Id).MemberGroupId.ToString();
                    memItem.PageMode = Mode;

                    listMemForView.Add(memItem);

                    HeaderValueVIP hv = new HeaderValueVIP()
                    {
                        MemberList = listMemForView,
                        strLoginName = userName
                    };


                    return View(hv);
                }
                else
                {
                    List<MemberItem> listMemForView = new List<MemberItem>();

                    MemberItem memItem = new MemberItem();
                    memItem.MemberGroupForSelect = getAllMemberGroup();
                    memItem.PageMode = Mode;

                    listMemForView.Add(memItem);

                    HeaderValueVIP hv = new HeaderValueVIP()
                    {
                        MemberList = listMemForView,
                        strLoginName = userName
                    };


                    return View(hv);
                }

            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //public ActionResult AccountChosen(string accountId)
        //{
        //    // if credentials are correct.
        //    return View();
        //}
        

        public User getUserAuthen(UserLogin ul)
        {
            User us = new User();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                us = context.Users
                                .Where(b => b.Username == ul.Username && b.Password == ul.Password)
                                .FirstOrDefault();
            }

            return us;
        }

        public int getPaxNum(int branchId, int accountId)
        {
            List<OrderRecord> orderList = new List<OrderRecord>();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                orderList = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .ToList();
            }

            return orderList.Count();
        }

        public int getOilPrice(int branchId)
        {
            SystemSetting getOilVal = new SystemSetting();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                getOilVal = context.SystemSettings
                                .Where(b => b.BranchId == branchId && b.Name == "OilPrice")
                                .FirstOrDefault();
            }

            return Int32.Parse(getOilVal.Value);
        }

        public string getTotalSale(int branchId, int accountId)
        {
            var totalSales = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalSales = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Price)
                                .ToList().Sum();
            }



            return String.Format("{0:n0}", totalSales);
        }

        public int getTotalSaleInInteger(int branchId, int accountId)
        {
            var totalSales = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalSales = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Price)
                                .ToList().Sum();
            }



            return totalSales;
        }

        public string getTotalCommission(int branchId, int accountId)
        {
            var totalComs = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalComs = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Commission)
                                .ToList().Sum();
            }



            return String.Format("{0:n0}", totalComs);
        }

        public int getTotalCommissionForDashboard(int branchId, int accountId)
        {
            var totalComs = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalComs = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Commission)
                                .ToList().Sum();
            }



            return totalComs;
        }

        public string getTotalOtherSale(int branchId, int accountId)
        {
            var totalComs = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalComs = context.OtherSaleRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.OtherSaleId == 3 && b.CancelStatus == "false")
                                .Select(b => (int)b.Price)
                                .ToList().Sum();
            }



            return String.Format("{0:n0}", totalComs);
        }

        public Account getAccountValueFromAccountId(int branchId,int accountId)
        {
            Account ac = new Account();


            using (var context = new spasystemdbEntities())
            {
                ac = context.Accounts
                                .Where(b => b.Id == accountId && b.BranchId == branchId)
                                .FirstOrDefault();
            }

            return ac;
        }
        public Account getAccountValue(int branchId)
        {
            Account ac = new Account();


            using (var context = new spasystemdbEntities())
            {

                ac = context.Accounts
                                .Where(b => b.BranchId == branchId)
                                .OrderByDescending(b => b.Id)
                                .FirstOrDefault();
            }

            return ac;
        }

        public SelectList getAllAccountInSelectionList(int branchId)
        {
            using (var context = new spasystemdbEntities())
            {
                // Fetch the accounts from the database
                var listAllAccountsFD = context.Accounts
                                               .Where(b => b.BranchId == branchId)
                                               .OrderByDescending(b => b.CreateDateTime)
                                               .ToList() // Bring the data into memory
                                               .Select(account => new AccountFMD
                                               {
                                                   Id = account.Id,
                                                   CreateDateTime = (DateTime)account.CreateDateTime,
                                                   FormattedCreateDateTime = account.CreateDateTime.Value.ToString("dd-MM-yyyy HH:mm:ss")
                                               })
                                               .ToList();

                return new SelectList(listAllAccountsFD, "Id", "FormattedCreateDateTime");
            }
        }


        public SelectList getAllMonthList()
        {
            var listAllMonths = new List<SelectListItem>();
            var months = new[]
            {
                new { Text = "January", Value = "01" },
                new { Text = "February", Value = "02" },
                new { Text = "March", Value = "03" },
                new { Text = "April", Value = "04" },
                new { Text = "May", Value = "05" },
                new { Text = "June", Value = "06" },
                new { Text = "July", Value = "07" },
                new { Text = "August", Value = "08" },
                new { Text = "September", Value = "09" },
                new { Text = "October", Value = "10" },
                new { Text = "November", Value = "11" },
                new { Text = "December", Value = "12" }
            };

            foreach (var month in months)
            {
                listAllMonths.Add(new SelectListItem { Text = month.Text, Value = month.Value });
            }

            int currentMonth = DateTime.Now.Month;
            string currentMonthValue = currentMonth.ToString("D2");

            return new SelectList(listAllMonths, "Value", "Text", currentMonthValue);
        }


        public SelectList getAllYearList()
        {
            var listAllYears = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;

            for (int year = currentYear - 3; year <= currentYear + 3; year++)
            {
                listAllYears.Add(new SelectListItem() { Text = year.ToString(), Value = year.ToString() });
            }

            return new SelectList(listAllYears, "Value", "Text", currentYear.ToString());
        }


        //public JsonProp[] getOrderRecordForGraph(int branchId, int accountId)
        //{
        //    List<OrderRecord> listOrderRecord = new List<OrderRecord>();
        //    JsonProp[] arrPrePareToJS = new JsonProp[24];
        //    //var totalComs = new int();
        //    //DateTime current = DateTime.Now;
        //    //string curDateTime = current.ToString("yyyy-MM-dd");
        //    //DateTime dtStart = DateTime.ParseExact(curDateTime + " 05:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        //    //DateTime tomorrow = DateTime.Now.AddDays(1);
        //    //string tmrDateTime = tomorrow.ToString("yyyy-MM-dd");
        //    //DateTime dtEnd = DateTime.ParseExact(tmrDateTime + " 05:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        //    using (var context = new spasystemdbEntities())
        //    {
        //        // Query for all blogs with names starting with B 
        //        //var blogs = from b in context.Accounts
        //        //            where b.Date.Equals(2016-12-22)
        //        //            select b;

        //        //ac.Add((Account)blogs);
        //        // Query for the Blog named ADO.NET Blog
        //        //.AddDays(36)


        //        listOrderRecord = context.OrderRecords
        //            .Where(r => r.BranchId == branchId && r.AccountId == accountId && r.CancelStatus == "false")
        //            .ToList();

        //        //totalComs = context.OrderRecords
        //        //                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
        //        //                .Select(b => (int)b.Commission)
        //        //                .ToList().Sum();
        //    }

        //    int t07 = 0, t08 = 0, t09 = 0, t10 = 0, t11 = 0, t12 = 0, t13 = 0, t14 = 0, t15 = 0, t16 = 0, t17 = 0, t18 = 0, t19 = 0, t20 = 0, t21 = 0, t22 = 0, t23 = 0, t00 = 0, t01 = 0, t02 = 0, t03 = 0, t04 = 0;
        //    TimeSpan ts0630 = new TimeSpan(6, 30, 0);
        //    TimeSpan ts0730 = new TimeSpan(7, 30, 0);
        //    TimeSpan ts0830 = new TimeSpan(8, 30, 0);
        //    TimeSpan ts0930 = new TimeSpan(9, 30, 0);
        //    TimeSpan ts1030 = new TimeSpan(10, 30, 0);
        //    TimeSpan ts1130 = new TimeSpan(11, 30, 0);
        //    TimeSpan ts1230 = new TimeSpan(12, 30, 0);
        //    TimeSpan ts1330 = new TimeSpan(13, 30, 0);
        //    TimeSpan ts1430 = new TimeSpan(14, 30, 0);
        //    TimeSpan ts1530 = new TimeSpan(15, 30, 0);
        //    TimeSpan ts1630 = new TimeSpan(16, 30, 0);
        //    TimeSpan ts1730 = new TimeSpan(17, 30, 0);
        //    TimeSpan ts1830 = new TimeSpan(18, 30, 0);
        //    TimeSpan ts1930 = new TimeSpan(19, 30, 0);
        //    TimeSpan ts2030 = new TimeSpan(20, 30, 0);
        //    TimeSpan ts2130 = new TimeSpan(21, 30, 0);
        //    TimeSpan ts2230 = new TimeSpan(22, 30, 0);
        //    TimeSpan ts2330 = new TimeSpan(23, 30, 0);
        //    TimeSpan ts2359 = new TimeSpan(23, 59, 59);
        //    TimeSpan ts0000 = new TimeSpan(0, 0, 1);
        //    TimeSpan ts0030 = new TimeSpan(0, 30, 0);
        //    TimeSpan ts0130 = new TimeSpan(1, 30, 0);
        //    TimeSpan ts0230 = new TimeSpan(2, 30, 0);
        //    TimeSpan ts0330 = new TimeSpan(3, 30, 0);
        //    TimeSpan ts0430 = new TimeSpan(4, 30, 0);

        //    int dataNum = listOrderRecord.Count();

        //    foreach (OrderRecord or in listOrderRecord)
        //    {
        //        if ((or.Time > ts0630) && (or.Time < ts0730))
        //        {
        //            t07++;
        //        }
        //        else if ((or.Time > ts0730) && (or.Time < ts0830))
        //        {
        //            t08++;
        //        }
        //        else if ((or.Time > ts0830) && (or.Time < ts0930))
        //        {
        //            t09++;
        //        }
        //        else if ((or.Time > ts0930) && (or.Time < ts1030))
        //        {
        //            t10++;
        //        }
        //        else if ((or.Time > ts1030) && (or.Time < ts1130))
        //        {
        //            t11++;
        //        }
        //        else if ((or.Time > ts1130) && (or.Time < ts1230))
        //        {
        //            t12++;
        //        }
        //        else if ((or.Time > ts1230) && (or.Time < ts1330))
        //        {
        //            t13++;
        //        }
        //        else if ((or.Time > ts1330) && (or.Time < ts1430))
        //        {
        //            t14++;
        //        }
        //        else if ((or.Time > ts1430) && (or.Time < ts1530))
        //        {
        //            t15++;
        //        }
        //        else if ((or.Time > ts1530) && (or.Time < ts1630))
        //        {
        //            t16++;
        //        }
        //        else if ((or.Time > ts1630) && (or.Time < ts1730))
        //        {
        //            t17++;
        //        }
        //        else if ((or.Time > ts1730) && (or.Time < ts1830))
        //        {
        //            t18++;
        //        }
        //        else if ((or.Time > ts1830) && (or.Time < ts1930))
        //        {
        //            t19++;
        //        }
        //        else if ((or.Time > ts1930) && (or.Time < ts2030))
        //        {
        //            t20++;
        //        }
        //        else if ((or.Time > ts2030) && (or.Time < ts2130))
        //        {
        //            t21++;
        //        }
        //        else if ((or.Time > ts2130) && (or.Time < ts2230))
        //        {
        //            t22++;
        //        }
        //        else if ((or.Time > ts2230) && (or.Time < ts2330))
        //        {
        //            t23++;
        //        }
        //        else if ((or.Time > ts2330) && (or.Time < ts2359))
        //        {
        //            t00++;
        //        }
        //        else if ((or.Time > ts0000) && (or.Time < ts0030))
        //        {
        //            t00++;
        //        }
        //        else if ((or.Time > ts0030) && (or.Time < ts0130))
        //        {
        //            t01++;
        //        }
        //        else if ((or.Time > ts0130) && (or.Time < ts0230))
        //        {
        //            t02++;
        //        }
        //        else if ((or.Time > ts0230) && (or.Time < ts0330))
        //        {
        //            t03++;
        //        }
        //        else if ((or.Time > ts0330) && (or.Time < ts0430))
        //        {
        //            t04++;
        //        }
        //    }

        //    if(listOrderRecord.Count()==0)
        //    {
        //        arrPrePareToJS[0] = new JsonProp() { x = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), y = t07 };
        //        arrPrePareToJS[1] = new JsonProp() { x = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t08 };
        //        arrPrePareToJS[2] = new JsonProp() { x = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t09 };
        //        arrPrePareToJS[3] = new JsonProp() { x = DateTime.Now.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t10 };
        //        arrPrePareToJS[4] = new JsonProp() { x = DateTime.Now.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t11 };
        //        arrPrePareToJS[5] = new JsonProp() { x = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = t12 };
        //        arrPrePareToJS[6] = new JsonProp() { x = DateTime.Now.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = t13 };
        //        arrPrePareToJS[7] = new JsonProp() { x = DateTime.Now.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), y = t14 };
        //        arrPrePareToJS[8] = new JsonProp() { x = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), y = t15 };
        //        arrPrePareToJS[9] = new JsonProp() { x = DateTime.Now.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"), y = t16 };
        //        arrPrePareToJS[10] = new JsonProp() { x = DateTime.Now.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"), y = t17 };
        //        arrPrePareToJS[11] = new JsonProp() { x = DateTime.Now.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"), y = t18 };
        //        arrPrePareToJS[12] = new JsonProp() { x = DateTime.Now.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"), y = t19 };
        //        //arrPrePareToJS[17] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.ToString("yyyy-MM-dd HH:mm:ss"), y = t00 };
        //        //arrPrePareToJS[18] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t01 };
        //        //arrPrePareToJS[19] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t02 };
        //        //arrPrePareToJS[20] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t03 };
        //        //arrPrePareToJS[21] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t04 };
        //        //arrPrePareToJS[22] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
        //        //arrPrePareToJS[23] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
        //    }
        //    else
        //    {
        //        arrPrePareToJS[0] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), y = t07 };
        //        arrPrePareToJS[1] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), y = t08 };
        //        arrPrePareToJS[2] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"), y = t09 };
        //        arrPrePareToJS[3] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"), y = t10 };
        //        arrPrePareToJS[4] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"), y = t11 };
        //        arrPrePareToJS[5] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"), y = t12 };
        //        arrPrePareToJS[6] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(13).ToString("yyyy-MM-dd HH:mm:ss"), y = t13 };
        //        arrPrePareToJS[7] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(14).ToString("yyyy-MM-dd HH:mm:ss"), y = t14 };
        //        arrPrePareToJS[8] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(15).ToString("yyyy-MM-dd HH:mm:ss"), y = t15 };
        //        arrPrePareToJS[9] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(16).ToString("yyyy-MM-dd HH:mm:ss"), y = t16 };
        //        arrPrePareToJS[10] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(17).ToString("yyyy-MM-dd HH:mm:ss"), y = t17 };
        //        arrPrePareToJS[11] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(18).ToString("yyyy-MM-dd HH:mm:ss"), y = t18 };
        //        arrPrePareToJS[12] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(19).ToString("yyyy-MM-dd HH:mm:ss"), y = t19 };
        //        arrPrePareToJS[13] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(20).ToString("yyyy-MM-dd HH:mm:ss"), y = t20 };
        //        arrPrePareToJS[14] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(21).ToString("yyyy-MM-dd HH:mm:ss"), y = t21 };
        //        arrPrePareToJS[15] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(22).ToString("yyyy-MM-dd HH:mm:ss"), y = t22 };
        //        arrPrePareToJS[16] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(23).ToString("yyyy-MM-dd HH:mm:ss"), y = t23 };
        //        arrPrePareToJS[17] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.ToString("yyyy-MM-dd HH:mm:ss"), y = t00 };
        //        arrPrePareToJS[18] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t01 };
        //        arrPrePareToJS[19] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t02 };
        //        arrPrePareToJS[20] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t03 };
        //        arrPrePareToJS[21] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t04 };
        //        arrPrePareToJS[22] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
        //        arrPrePareToJS[23] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
        //        //arrPrePareToJS[0] = "{ x: " + listOrderRecord[0].Date.AddHours(7).ToString().Replace("/", "-") + ", y: " + t07 + " }";
        //        //arrPrePareToJS[1] = "{ x: " + listOrderRecord[0].Date.AddHours(8).ToString().Replace("/", "-") + ", y: " + t08 + " }";
        //        //arrPrePareToJS[2] = "{ x: " + listOrderRecord[0].Date.AddHours(9).ToString().Replace("/", "-") + ", y: " + t09 + " }";
        //        //arrPrePareToJS[3] = "{ x: " + listOrderRecord[0].Date.AddHours(10).ToString().Replace("/", "-") + ", y: " + t10 + " }";
        //        //arrPrePareToJS[4] = "{ x: " + listOrderRecord[0].Date.AddHours(11).ToString().Replace("/", "-") + ", y: " + t11 + " }";
        //        //arrPrePareToJS[5] = "{ x: " + listOrderRecord[0].Date.AddHours(12).ToString().Replace("/", "-") + ", y: " + t12 + " }";
        //        //arrPrePareToJS[6] = "{ x: " + listOrderRecord[0].Date.AddHours(13).ToString().Replace("/", "-") + ", y: " + t13 + " }";
        //        //arrPrePareToJS[7] = "{ x: " + listOrderRecord[0].Date.AddHours(14).ToString().Replace("/", "-") + ", y: " + t14 + " }";
        //        //arrPrePareToJS[8] = "{ x: " + listOrderRecord[0].Date.AddHours(15).ToString().Replace("/", "-") + ", y: " + t15 + " }";
        //        //arrPrePareToJS[9] = "{ x: " + listOrderRecord[0].Date.AddHours(16).ToString().Replace("/", "-") + ", y: " + t16 + " }";
        //        //arrPrePareToJS[10] = "{ x: " + listOrderRecord[0].Date.AddHours(17).ToString().Replace("/", "-") + ", y: " + t17 + " }";
        //        //arrPrePareToJS[11] = "{ x: " + listOrderRecord[0].Date.AddHours(18).ToString().Replace("/", "-") + ", y: " + t18 + " }";
        //        //arrPrePareToJS[12] = "{ x: " + listOrderRecord[0].Date.AddHours(19).ToString().Replace("/", "-") + ", y: " + t19 + " }";
        //        //arrPrePareToJS[13] = "{ x: " + listOrderRecord[0].Date.AddHours(20).ToString().Replace("/", "-") + ", y: " + t20 + " }";
        //        //arrPrePareToJS[14] = "{ x: " + listOrderRecord[0].Date.AddHours(21).ToString().Replace("/", "-") + ", y: " + t21 + " }";
        //        //arrPrePareToJS[15] = "{ x: " + listOrderRecord[0].Date.AddHours(22).ToString().Replace("/", "-") + ", y: " + t22 + " }";
        //        //arrPrePareToJS[16] = "{ x: " + listOrderRecord[0].Date.AddHours(23).ToString().Replace("/", "-") + ", y: " + t23 + " }";
        //        //arrPrePareToJS[17] = "{ x: " + listOrderRecord[dataNum - 1].Date.ToString().Replace("/", "-") + ", y: " + t00 + " }";
        //        //arrPrePareToJS[18] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(1).ToString().Replace("/", "-") + ", y: " + t01 + " }";
        //        //arrPrePareToJS[19] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(2).ToString().Replace("/", "-") + ", y: " + t02 + " }";
        //        //arrPrePareToJS[20] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(3).ToString().Replace("/", "-") + ", y: " + t03 + " }";
        //        //arrPrePareToJS[21] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(4).ToString().Replace("/", "-") + ", y: " + t04 + " }";
        //        //arrPrePareToJS[22] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(5).ToString().Replace("/", "-") + ", y: 0 }";
        //        //arrPrePareToJS[23] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(6).ToString().Replace("/", "-") + ", y: 0 }";
        //    }
        //    return arrPrePareToJS;
        //}

        public JsonProp[] getOrderRecordForGraph(int branchId, int accountId)
        {
            JsonProp[] arrPrePareToJS = new JsonProp[24];
            TimeSpan[] timeSlots = new TimeSpan[]
            {
        new TimeSpan(6, 30, 0), new TimeSpan(7, 30, 0), new TimeSpan(8, 30, 0), new TimeSpan(9, 30, 0),
        new TimeSpan(10, 30, 0), new TimeSpan(11, 30, 0), new TimeSpan(12, 30, 0), new TimeSpan(13, 30, 0),
        new TimeSpan(14, 30, 0), new TimeSpan(15, 30, 0), new TimeSpan(16, 30, 0), new TimeSpan(17, 30, 0),
        new TimeSpan(18, 30, 0), new TimeSpan(19, 30, 0), new TimeSpan(20, 30, 0), new TimeSpan(21, 30, 0),
        new TimeSpan(22, 30, 0), new TimeSpan(23, 30, 0), new TimeSpan(23, 59, 59), new TimeSpan(0, 0, 1),
        new TimeSpan(0, 30, 0), new TimeSpan(1, 30, 0), new TimeSpan(2, 30, 0), new TimeSpan(3, 30, 0)
            };

            int[] counts = new int[24];

            using (var context = new spasystemdbEntities())
            {
                var orders = context.OrderRecords
                    .Where(r => r.BranchId == branchId && r.AccountId == accountId && r.CancelStatus == "false")
                    .Select(r => r.Time)
                    .ToList();

                foreach (var orderTime in orders)
                {
                    for (int i = 0; i < timeSlots.Length - 1; i++)
                    {
                        if (orderTime > timeSlots[i] && orderTime <= timeSlots[i + 1])
                        {
                            counts[i]++;
                            break;
                        }
                    }
                }
            }

            DateTime startDateTime = DateTime.Now.Date.AddHours(7); // Start time at 7:00 AM
            for (int i = 0; i < 24; i++)
            {
                arrPrePareToJS[i] = new JsonProp
                {
                    x = startDateTime.AddHours(i).ToString("yyyy-MM-dd HH:mm:ss"),
                    y = counts[i]
                };
            }

            return arrPrePareToJS;
        }

        public JsonProp[] getOrderRecordForGraph_B(int branchId, int accountId, int sellItemTypeId)
        {
            JsonProp[] arrPrePareToJS = new JsonProp[24];
            TimeSpan[] timeSlots = new TimeSpan[]
            {
        new TimeSpan(6, 30, 0), new TimeSpan(7, 30, 0), new TimeSpan(8, 30, 0), new TimeSpan(9, 30, 0),
        new TimeSpan(10, 30, 0), new TimeSpan(11, 30, 0), new TimeSpan(12, 30, 0), new TimeSpan(13, 30, 0),
        new TimeSpan(14, 30, 0), new TimeSpan(15, 30, 0), new TimeSpan(16, 30, 0), new TimeSpan(17, 30, 0),
        new TimeSpan(18, 30, 0), new TimeSpan(19, 30, 0), new TimeSpan(20, 30, 0), new TimeSpan(21, 30, 0),
        new TimeSpan(22, 30, 0), new TimeSpan(23, 30, 0), new TimeSpan(23, 59, 59), new TimeSpan(0, 0, 1),
        new TimeSpan(0, 30, 0), new TimeSpan(1, 30, 0), new TimeSpan(2, 30, 0), new TimeSpan(3, 30, 0)
            };

            int[] counts = new int[24];

            using (var context = new spasystemdbEntities())
            {
                List<int> orderReceipts = context.OrderReceipts
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.SellItemTypeId == sellItemTypeId)
                                .Select(b => b.Id)
                                .ToList();

                var orderResults = (from or in context.OrderRecords
                                    where or.BranchId == branchId
                                          && orderReceipts.Contains((int)or.OrderReceiptId)
                                          && or.CancelStatus == "false"
                                    select or.Time).ToList();

                //var orders = context.OrderRecords
                //    .Where(r => r.BranchId == branchId && r.AccountId == accountId && r.CancelStatus == "false")
                //    .Select(r => r.Time)
                //    .ToList();

                foreach (var orderTime in orderResults)
                {
                    for (int i = 0; i < timeSlots.Length - 1; i++)
                    {
                        if (orderTime > timeSlots[i] && orderTime <= timeSlots[i + 1])
                        {
                            counts[i]++;
                            break;
                        }
                    }
                }
            }

            DateTime startDateTime = DateTime.Now.Date.AddHours(7); // Start time at 7:00 AM
            for (int i = 0; i < 24; i++)
            {
                arrPrePareToJS[i] = new JsonProp
                {
                    x = startDateTime.AddHours(i).ToString("yyyy-MM-dd HH:mm:ss"),
                    y = counts[i]
                };
            }

            return arrPrePareToJS;
        }

        public IEnumerable<List<OrderRecord>> getBestSeller(int branchId, int accountId)
        {
            IEnumerable<List<OrderRecord>> groupedObjects;
            //List<OrderRecord> listOrderTopA = new List<OrderRecord>();
            //List<OrderRecord> listOrderTopB = new List<OrderRecord>();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                groupedObjects = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .GroupBy(b => b.MassageTopicId)
                                .Select(group => group.ToList())
                                .OrderByDescending(c => c.Count())
                                .ToList();
            }

            //foreach(var a in groupedObjects)
            //{
            //    if(listOrderTopA.Count==0)
            //    {
            //        listOrderTopA = a;
            //    }
            //    else if(listOrderTopB.Count()==0)
            //    {
            //        listOrderTopB = a;
            //    }
            //}


            return groupedObjects;
        }

        public JsonPropForTopAB[] getTopAForAday(int branchId)
        {
            List<JsonPropForTopAB> listResultTopA = new List<JsonPropForTopAB>();
            JsonPropForTopAB[] arrPrePareToJSForPie = new JsonPropForTopAB[0];

            SqlCommand command;
            SqlDataReader dataReader;
            String sql = " ";
            sql = "select dbo.MassagePlan.Name, count(dbo.OrderRecord.MassagePlanId) as 'Total Pax' from dbo.OrderRecord left join dbo.MassagePlan on dbo.OrderRecord.MassagePlanId = dbo.MassagePlan.Id where BranchId = '" + branchId + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) and CancelStatus = 'false' and MassageTopicId in (select top 1 dbo.OrderRecord.MassageTopicId from dbo.OrderRecord where BranchId = '" + branchId + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.OrderRecord.MassageTopicId order by count(dbo.OrderRecord.MassageTopicId) desc) group by dbo.MassagePlan.Name order by count(dbo.OrderRecord.MassagePlanId) desc;";

            connetionString = ConfigurationManager.AppSettings["cString"];
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (dataReader.GetValue(0).Equals(null) || dataReader.GetValue(0).ToString().Equals("null"))
                {
                    break;
                }
                else
                {
                    listResultTopA.Add(new JsonPropForTopAB() { label = "Plan : " + dataReader.GetValue(0).ToString(), value = (int)dataReader.GetValue(1) });
                    arrPrePareToJSForPie = listResultTopA.ToArray();
                }

            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

            return arrPrePareToJSForPie;
        }
        
        public JsonPropForTopAB[] getTopBForAday(int branchId)
        {
            List<JsonPropForTopAB> listResultTopA = new List<JsonPropForTopAB>();
            JsonPropForTopAB[] arrPrePareToJSForPie = new JsonPropForTopAB[0];

            SqlCommand command;
            SqlDataReader dataReader;
            String sql = " ";
            sql = "select dbo.MassagePlan.Name, count(dbo.OrderRecord.MassagePlanId) as 'Total Pax' from dbo.OrderRecord left join dbo.MassagePlan on dbo.OrderRecord.MassagePlanId = dbo.MassagePlan.Id where BranchId = '" + branchId + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) and CancelStatus = 'false' and MassageTopicId in (select dbo.OrderRecord.MassageTopicId from dbo.OrderRecord where BranchId = '" + branchId + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.OrderRecord.MassageTopicId order by count(dbo.OrderRecord.MassageTopicId) desc offset 1 row fetch next 1 row only) group by dbo.MassagePlan.Name order by count(dbo.OrderRecord.MassagePlanId) desc;";

            connetionString = ConfigurationManager.AppSettings["cString"];
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (dataReader.GetValue(0).Equals(null) || dataReader.GetValue(0).ToString().Equals("null"))
                {
                    break;
                }
                else
                {
                    listResultTopA.Add(new JsonPropForTopAB() { label = "Plan : " + dataReader.GetValue(0).ToString(), value = (int)dataReader.GetValue(1) });
                    arrPrePareToJSForPie = listResultTopA.ToArray();
                }

            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

            return arrPrePareToJSForPie;
        }

        public JsonPropForTopAB[] getTopA(IEnumerable<List<OrderRecord>> enumTopAB, int branchId)
        {
            string _temp = "";
            
            List<OrderRecord> listOrderTopA = new List<OrderRecord>();
            if(enumTopAB.Count() != 0)
            {
                foreach (var a in enumTopAB)
                {
                    if (listOrderTopA.Count == 0)
                    {
                        listOrderTopA = a;
                    }
                }

                List<int> listMassagePlanId = new List<int>();

                int massageSetId = getMassageSetId(branchId);
                int massageTopicId = listOrderTopA[0].MassageTopicId;


                using (var context = new spasystemdbEntities())
                {

                    listMassagePlanId = context.MassageSets
                                    .Where(b => b.Id == massageSetId && b.MassageTopicId == massageTopicId)
                                    .Select(b => b.MassagePlanId)
                                    .ToList();
                }

                List<int> listDupMassagePlanId = new List<int>();
                for (int x = 0; x < listOrderTopA.Count(); x++)
                {
                    listDupMassagePlanId.Add(listOrderTopA[x].MassagePlanId);
                }

                for(int k=0;k<listDupMassagePlanId.Count();k++)
                {
                    for(int j=0;j<listMassagePlanId.Count();j++)
                    {
                        if(listDupMassagePlanId[k] == listMassagePlanId[j])
                        {
                            _temp += j + "N" + 1 +"Y";
                        }
                    }
                }

                int[] finalVal = new int[listMassagePlanId.Count()];
                string[] _doTemps = _temp.Split('Y');
                for(int g=0;g<_doTemps.Length-1;g++)
                {
                    string[] _cutTemps = _doTemps[g].Split('N');
                    int getIndex = Int32.Parse(_cutTemps[0]);
                    int getVal = Int32.Parse(_cutTemps[1]);
                    finalVal[getIndex] = finalVal[getIndex]+getVal;
                }


                //var set = new HashSet<int>(listPlanId);
                //List<int> finalPlanId = set.ToList<int>();

                //var duplicates = listPlanId.GroupBy(s => s)
                //    .SelectMany(grp => grp.Skip(1));

                JsonPropForTopAB[] arrPrePareToJSForPie = new JsonPropForTopAB[listMassagePlanId.Count()];
                for (int i = 0; i < listMassagePlanId.Count(); i++)
                {
                    arrPrePareToJSForPie[i] = new JsonPropForTopAB() { label = "Plan : "+getPlanName(listMassagePlanId[i]), value = finalVal[i] };
                }

                return arrPrePareToJSForPie;
            }
            else
            {
                JsonPropForTopAB[] arrPrePareToJSForPie = new JsonPropForTopAB[0];
                return arrPrePareToJSForPie;
            }
        }

        public JsonPropForTopAB[] getTopB(IEnumerable<List<OrderRecord>> enumTopAB, int branchId)
        {
            string _temp = "";

            List<OrderRecord> _listOrderTopA = new List<OrderRecord>();
            List<OrderRecord> listOrderTopB = new List<OrderRecord>();
            if(enumTopAB.Count() > 1)
            {
                foreach (var a in enumTopAB)
                {
                    if (_listOrderTopA.Count == 0)
                    {
                        _listOrderTopA = a;
                    }
                    else if(listOrderTopB.Count == 0)
                    {
                        listOrderTopB = a;
                    }
                }

                List<int> listMassagePlanId = new List<int>();

                int massageSetId = getMassageSetId(branchId);
                int massageTopicId = listOrderTopB[0].MassageTopicId;


                using (var context = new spasystemdbEntities())
                {

                    listMassagePlanId = context.MassageSets
                                    .Where(b => b.Id == massageSetId && b.MassageTopicId == massageTopicId)
                                    .Select(b => b.MassagePlanId)
                                    .ToList();
                }

                List<int> listDupMassagePlanId = new List<int>();
                for (int x = 0; x < listOrderTopB.Count(); x++)
                {
                    listDupMassagePlanId.Add(listOrderTopB[x].MassagePlanId);
                }

                for (int k = 0; k < listDupMassagePlanId.Count(); k++)
                {
                    for (int j = 0; j < listMassagePlanId.Count(); j++)
                    {
                        if (listDupMassagePlanId[k] == listMassagePlanId[j])
                        {
                            _temp += j + "N" + 1 + "Y";
                        }
                    }
                }

                int[] finalVal = new int[listMassagePlanId.Count()];
                string[] _doTemps = _temp.Split('Y');
                for (int g = 0; g < _doTemps.Length - 1; g++)
                {
                    string[] _cutTemps = _doTemps[g].Split('N');
                    int getIndex = Int32.Parse(_cutTemps[0]);
                    int getVal = Int32.Parse(_cutTemps[1]);
                    finalVal[getIndex] = finalVal[getIndex] + getVal;
                }


                //var set = new HashSet<int>(listPlanId);
                //List<int> finalPlanId = set.ToList<int>();

                //var duplicates = listPlanId.GroupBy(s => s)
                //    .SelectMany(grp => grp.Skip(1));

                JsonPropForTopAB[] arrPrePareToJSForPie = new JsonPropForTopAB[listMassagePlanId.Count()];
                for (int i = 0; i < listMassagePlanId.Count(); i++)
                {
                    arrPrePareToJSForPie[i] = new JsonPropForTopAB() { label = "Plan : " + getPlanName(listMassagePlanId[i]), value = finalVal[i] };
                }

                return arrPrePareToJSForPie;
            }
            else
            {
                JsonPropForTopAB[] arrPrePareToJSForPie = new JsonPropForTopAB[0];
                return arrPrePareToJSForPie;
            }
        }

        public string getTopicName(int TopicId)
        {
            string topicName;
            using (var context = new spasystemdbEntities())
            {

                topicName = context.MassageTopics
                                .Where(b => b.Id == TopicId)
                                .Select(b => b.Name)
                                .FirstOrDefault();
            }

            return topicName;
        }
        public string getPlanName(int PlanId)
        {
            string planName;
            using (var context = new spasystemdbEntities())
            {

                planName = context.MassagePlans
                                .Where(b => b.Id == PlanId)
                                .Select(b => b.Name)
                                .FirstOrDefault();
            }

            return planName;
        }

        public string getTopATopicNameForAday(int branchId)
        {
            //List<OrderRecord> listOrderTopA = new List<OrderRecord>();
            string topATopicName = "No record";
            
            SqlCommand command;
            SqlDataReader dataReader;
            String sql = " ";
            sql = "select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchId + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc;";

            connetionString = ConfigurationManager.AppSettings["cString"];
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                topATopicName = dataReader.GetValue(0).ToString();
            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

            return topATopicName;

        }

        public string getTopBTopicNameForAday(int branchId)
        {
            //List<OrderRecord> listOrderTopA = new List<OrderRecord>();
            string topBTopicName = "No record";
            //if (enumTopAB.Count() != 0)
            //{
            //    foreach (var a in enumTopAB)
            //    {
            //        if (listOrderTopA.Count == 0)
            //        {
            //            listOrderTopA = a;
            //        }
            //    }

            //    int topicId = listOrderTopA[0].MassageTopicId;

            //    using (var context = new spasystemdbEntities())
            //    {
            //        // Query for all blogs with names starting with B 
            //        //var blogs = from b in context.Accounts
            //        //            where b.Date.Equals(2016-12-22)
            //        //            select b;

            //        //ac.Add((Account)blogs);
            //        // Query for the Blog named ADO.NET Blog

            //        topATopicName = context.MassageTopics
            //                        .Where(b => b.Id == topicId)
            //                        .Select(b => b.Name)
            //                        .FirstOrDefault();
            //    }
            //}

            SqlCommand command;
            SqlDataReader dataReader;
            String sql = " ";
            sql = "select top 2 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchId + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc;";

            connetionString = ConfigurationManager.AppSettings["cString"];
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                topBTopicName = dataReader.GetValue(0).ToString();
            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

            return topBTopicName;

        }

        public string getTopATopicName(IEnumerable<List<OrderRecord>> enumTopAB)
        {
            List<OrderRecord> listOrderTopA = new List<OrderRecord>();
            string topATopicName ="No record";
            if (enumTopAB.Count() != 0)
            {
                foreach (var a in enumTopAB)
                {
                    if (listOrderTopA.Count == 0)
                    {
                        listOrderTopA = a;
                    }
                }

                int topicId = listOrderTopA[0].MassageTopicId;

                using (var context = new spasystemdbEntities())
                {
                    // Query for all blogs with names starting with B 
                    //var blogs = from b in context.Accounts
                    //            where b.Date.Equals(2016-12-22)
                    //            select b;

                    //ac.Add((Account)blogs);
                    // Query for the Blog named ADO.NET Blog

                    topATopicName = context.MassageTopics
                                    .Where(b => b.Id == topicId)
                                    .Select(b => b.Name)
                                    .FirstOrDefault();
                }
            }


            return topATopicName;

        }

        public string getTopBTopicName(IEnumerable<List<OrderRecord>> enumTopAB)
        {
            List<OrderRecord> _temp = new List<OrderRecord>();
            List<OrderRecord> listOrderTopB = new List<OrderRecord>();
            string topBTopicName = "No record";
            if (enumTopAB.Count() != 0)
            {
                foreach (var b in enumTopAB)
                {
                    if (_temp.Count == 0)
                    {
                        _temp = b;
                    }
                    else if (listOrderTopB.Count == 0)
                    {
                        listOrderTopB = b;
                    }
                }

                if(listOrderTopB.Count!=0)
                {

                
                int topicId = listOrderTopB[0].MassageTopicId;

                    using (var context = new spasystemdbEntities())
                    {
                        // Query for all blogs with names starting with B 
                        //var blogs = from b in context.Accounts
                        //            where b.Date.Equals(2016-12-22)
                        //            select b;

                        //ac.Add((Account)blogs);
                        // Query for the Blog named ADO.NET Blog

                        topBTopicName = context.MassageTopics
                                        .Where(b => b.Id == topicId)
                                        .Select(b => b.Name)
                                        .FirstOrDefault();
                    }
                }
            }
            return topBTopicName;
        }

        public int getMassageSetId(int BranchId)
        {
            int massageSetId;

            using (var context = new spasystemdbEntities())
            {

                massageSetId = context.Branches
                                .Where(b => b.Id == BranchId)
                                .Select(b => b.MassageSetId)
                                .FirstOrDefault();
            }

            return massageSetId;

        }

        public List<MassageTopic> getListAllTopic(int massageSetId)
        {
            List<MassageTopic> listAllMassageTopic = new List<MassageTopic>();
            List<int> listAllMassageTopicId;
            using (var context = new spasystemdbEntities())
            {

                listAllMassageTopicId = context.MassageSets
                                .Where(b => b.Id == massageSetId)
                                .Select(b => b.MassageTopicId)
                                .ToList();
            }

            var set = new HashSet<int>(listAllMassageTopicId);
            List<int> finalListMassageTopicId = set.ToList<int>();

            for(int i=0;i<finalListMassageTopicId.Count();i++)
            {
                int getMsgTopicId = finalListMassageTopicId[i];
                MassageTopic msgTopic = new MassageTopic();
                using (var contexts = new spasystemdbEntities())
                {

                    msgTopic = contexts.MassageTopics
                                    .Where(b => b.Id == getMsgTopicId)
                                    .FirstOrDefault();
                }

                listAllMassageTopic.Add(msgTopic);

            }

            return listAllMassageTopic;
        }

        //public List<MassagePlan> getListAllPlan(int massageSetId)
        //{
        //    List<MassagePlan> listAllMassagePlan = new List<MassagePlan>();
        //    List<int> listAllMassagePlanId;
        //    using (var context = new spasystemdbEntities())
        //    {

        //        listAllMassagePlanId = context.MassageSets
        //                        .Where(b => b.Id == massageSetId)
        //                        .Select(b => b.MassagePlanId)
        //                        .ToList();
        //    }

        //    var set = new HashSet<int>(listAllMassagePlanId);
        //    List<int> finalListMassagePlanId = set.ToList<int>();

        //    for (int i = 0; i < finalListMassagePlanId.Count(); i++)
        //    {
        //        int getMsgPlanId = finalListMassagePlanId[i];
        //        MassagePlan msgPlan = new MassagePlan();
        //        using (var contexts = new spasystemdbEntities())
        //        {

        //            msgPlan = contexts.MassagePlans
        //                            .Where(b => b.Id == getMsgPlanId)
        //                            .FirstOrDefault();
        //        }

        //        listAllMassagePlan.Add(msgPlan);

        //    }

        //    return listAllMassagePlan;
        //}

        public JsonPropForTopAB[] getPlanInTopicVal(int branchId, int accountId, int massageTopicId)
        {
            string _temp = "";

            List<int> listMassagePlanId = new List<int>();

            int massageSetId = getMassageSetId(branchId);
            //int massageTopicId = listOrderTopA[0].MassageTopicId;
            using (var context = new spasystemdbEntities())
            {

                listMassagePlanId = context.MassageSets
                                .Where(b => b.Id == massageSetId && b.MassageTopicId == massageTopicId)
                                .Select(b => b.MassagePlanId)
                                .ToList();
            }

            List<int> listDupMassagePlanId = new List<int>();
            using (var context = new spasystemdbEntities())
            {

                listDupMassagePlanId = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.MassageTopicId == massageTopicId)
                                .Select(b => b.MassagePlanId)
                                .ToList();
            }


            for (int k = 0; k < listDupMassagePlanId.Count(); k++)
            {
                for (int j = 0; j < listMassagePlanId.Count(); j++)
                {
                    if (listDupMassagePlanId[k] == listMassagePlanId[j])
                    {
                        _temp += j + "N" + 1 + "Y";
                    }
                }
            }

            int[] finalVal = new int[listMassagePlanId.Count()];
            string[] _doTemps = _temp.Split('Y');
            for (int g = 0; g < _doTemps.Length - 1; g++)
            {
                string[] _cutTemps = _doTemps[g].Split('N');
                int getIndex = Int32.Parse(_cutTemps[0]);
                int getVal = Int32.Parse(_cutTemps[1]);
                finalVal[getIndex] = finalVal[getIndex] + getVal;
            }


            //var set = new HashSet<int>(listPlanId);
            //List<int> finalPlanId = set.ToList<int>();

            //var duplicates = listPlanId.GroupBy(s => s)
            //    .SelectMany(grp => grp.Skip(1));

            JsonPropForTopAB[] arrPrePareToJSForPie = new JsonPropForTopAB[listMassagePlanId.Count()];
            for (int i = 0; i < listMassagePlanId.Count(); i++)
            {
                arrPrePareToJSForPie[i] = new JsonPropForTopAB() { label = getPlanName(listMassagePlanId[i]), value = finalVal[i] };
            }

            return arrPrePareToJSForPie;
        }

        //public List<OrderRecordWithName> getAllOrderRecordForTable(int branchId, int accountId)
        //{
        //    List<OrderRecordWithName> getListAllOrderRecordWithName = new List<OrderRecordWithName>();
        //    List<OrderRecord> getListAllOrderRecord;

        //    using (var contexts = new spasystemdbEntities())
        //    {

        //        getListAllOrderRecord = contexts.OrderRecords
        //                        .Where(b => b.BranchId == branchId && b.AccountId == accountId)
        //                        .ToList();
        //    }

        //    for(int i=0;i<getListAllOrderRecord.Count();i++)
        //    {
        //        OrderRecordWithName _item = new OrderRecordWithName()
        //        {
        //            Date = getListAllOrderRecord[i].Date.ToString("dd-MM-yyyy"),
        //            Time = getListAllOrderRecord[i].Time,
        //            MassageTopicName = getTopicName(getListAllOrderRecord[i].MassageTopicId),
        //            MassagePlanName = getPlanName(getListAllOrderRecord[i].MassagePlanId),
        //            Price = String.Format("{0:n}", getListAllOrderRecord[i].Price),
        //            Commission = String.Format("{0:n}", getListAllOrderRecord[i].Commission),
        //            CancelStatus = BoolToWord(getListAllOrderRecord[i].CancelStatus)

        //        };

        //        getListAllOrderRecordWithName.Add(_item);

        //    }

        //    return getListAllOrderRecordWithName;
        //}

        public List<FinalSaleForEachTopic> getFinalSaleForEach(int branchId, string accountId)
        {
            List<FinalSaleForEachTopic> listOfFinalSale = new List<FinalSaleForEachTopic>();

            SqlCommand command;
            SqlDataReader dataReader;
            String sql = " ";
            if(accountId == null)
            {
                sql = "select dbo.MassageTopic.Name, count(dbo.OrderRecord.MassageTopicId) as 'Total Pax', sum(Price) as 'Total Sale' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchId + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc;";
            }
            else
            {
                sql = "select dbo.MassageTopic.Name, count(dbo.OrderRecord.MassageTopicId) as 'Total Pax', sum(Price) as 'Total Sale' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchId + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc;";
            }
            

            connetionString = ConfigurationManager.AppSettings["cString"];
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            command = new SqlCommand(sql, cnn);

            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                listOfFinalSale.Add(new FinalSaleForEachTopic { MassageTopicName = dataReader.GetValue(0).ToString(), TotalPax = String.Format("{0:n0}", dataReader.GetValue(1)), TotalSale = String.Format("{0:n}", dataReader.GetValue(2)) });
            }

            dataReader.Close();
            command.Dispose();
            cnn.Close();

            return listOfFinalSale;

        }

        public List<FinalSaleForEachTopic> getFinalSaleForEach_B(int branchId, string accountId, int sellItemTypeId)
        {
            List<FinalSaleForEachTopic> listOfFinalSale = new List<FinalSaleForEachTopic>();

            SqlCommand command;
            SqlDataReader dataReader;
            String sql = " ";
            int useAccount = int.Parse(accountId);


            using (var dbContext = new spasystemdbEntities())
            {
                List<int> orderReceipts = dbContext.OrderReceipts
                            .Where(b => b.BranchId == branchId && b.AccountId == useAccount && b.CancelStatus == "false" && b.SellItemTypeId == sellItemTypeId)
                            .Select(b => b.Id)
                            .ToList();

                // Fetch all relevant OrderRecords
                listOfFinalSale = (from or in dbContext.OrderRecords
                                   join topic in dbContext.MassageTopics on or.MassageTopicId equals topic.Id
                                   where or.BranchId == branchId
                                         && orderReceipts.Contains((int)or.OrderReceiptId)
                                         && or.CancelStatus == "false"
                                   group or by topic.Name into grouped
                                   select new FinalSaleForEachTopic
                                   {
                                       MassageTopicName = grouped.Key,
                                       TotalPax = grouped.Count().ToString(),
                                       TotalSale = grouped.Sum(o => o.Price).ToString()
                                   }).OrderByDescending(g => g.TotalPax).ToList();


            }


            //connetionString = ConfigurationManager.AppSettings["cString"];
            //cnn = new SqlConnection(connetionString);
            //cnn.Open();
            //command = new SqlCommand(sql, cnn);

            //dataReader = command.ExecuteReader();
            //while (dataReader.Read())
            //{
            //    listOfFinalSale.Add(new FinalSaleForEachTopic { MassageTopicName = dataReader.GetValue(0).ToString(), TotalPax = String.Format("{0:n0}", dataReader.GetValue(1)), TotalSale = String.Format("{0:n}", dataReader.GetValue(2)) });
            //}

            //dataReader.Close();
            //command.Dispose();
            //cnn.Close();

            return listOfFinalSale;

        }

        public string BoolToWord(string boolWord)
        {
            if(boolWord.Equals("true"))
            {
                return "Cancelled";
            }
            else
            {
                return "Sold";
            }
        }

        public int getRandomNo()
        {
            Random rnd = new Random();
            int a = rnd.Next(52);     // creates a number between 0 and 51
            return a;
        }

        //------------------------- Month -------------------------------------------
        public int getTotalSaleInMonth(int branchId, int accountId)
        {
            var totalSales = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalSales = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Price)
                                .ToList().Sum();
            }



            return totalSales;
        }

        public int getTotalCommissionInMonth(int branchId, int accountId)
        {
            var totalComs = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalComs = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Commission)
                                .ToList().Sum();
            }



            return totalComs;
        }

        public int getTotalOtherSaleInMonth(int branchId, int accountId)
        {
            var totalComs = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalComs = context.OtherSaleRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.OtherSaleId == 3 && b.CancelStatus == "false")
                                .Select(b => (int)b.Price)
                                .ToList().Sum();
            }



            return totalComs;
        }
        public JsonProp[] getOrderRecordForGraphInMonth(int branchId, List<Account> listAccountId)
        {
            List<OrderRecord> listOrderRecord = new List<OrderRecord>();
            JsonProp[] arrPrePareToJS = new JsonProp[24];
            int getFirstId = 50000000;
            int getLastId = 60000000;
            if (listAccountId.Count() != 0)
            {
                getFirstId = listAccountId[0].Id;
                getLastId = listAccountId[listAccountId.Count() - 1].Id;
            }

            //var totalComs = new int();
            //DateTime current = DateTime.Now;
            //string curDateTime = current.ToString("yyyy-MM-dd");
            //DateTime dtStart = DateTime.ParseExact(curDateTime + " 05:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            //DateTime tomorrow = DateTime.Now.AddDays(1);
            //string tmrDateTime = tomorrow.ToString("yyyy-MM-dd");
            //DateTime dtEnd = DateTime.ParseExact(tmrDateTime + " 05:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog
                //.AddDays(36)


                listOrderRecord = context.OrderRecords
                    .Where(r => r.BranchId == branchId && r.AccountId >= getFirstId && r.AccountId <= getLastId && r.CancelStatus == "false")
                    .ToList();

                //totalComs = context.OrderRecords
                //                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                //                .Select(b => (int)b.Commission)
                //                .ToList().Sum();
            }

            int t07 = 0, t08 = 0, t09 = 0, t10 = 0, t11 = 0, t12 = 0, t13 = 0, t14 = 0, t15 = 0, t16 = 0, t17 = 0, t18 = 0, t19 = 0, t20 = 0, t21 = 0, t22 = 0, t23 = 0, t00 = 0, t01 = 0, t02 = 0, t03 = 0, t04 = 0;
            TimeSpan ts0630 = new TimeSpan(6, 30, 0);
            TimeSpan ts0730 = new TimeSpan(7, 30, 0);
            TimeSpan ts0830 = new TimeSpan(8, 30, 0);
            TimeSpan ts0930 = new TimeSpan(9, 30, 0);
            TimeSpan ts1030 = new TimeSpan(10, 30, 0);
            TimeSpan ts1130 = new TimeSpan(11, 30, 0);
            TimeSpan ts1230 = new TimeSpan(12, 30, 0);
            TimeSpan ts1330 = new TimeSpan(13, 30, 0);
            TimeSpan ts1430 = new TimeSpan(14, 30, 0);
            TimeSpan ts1530 = new TimeSpan(15, 30, 0);
            TimeSpan ts1630 = new TimeSpan(16, 30, 0);
            TimeSpan ts1730 = new TimeSpan(17, 30, 0);
            TimeSpan ts1830 = new TimeSpan(18, 30, 0);
            TimeSpan ts1930 = new TimeSpan(19, 30, 0);
            TimeSpan ts2030 = new TimeSpan(20, 30, 0);
            TimeSpan ts2130 = new TimeSpan(21, 30, 0);
            TimeSpan ts2230 = new TimeSpan(22, 30, 0);
            TimeSpan ts2330 = new TimeSpan(23, 30, 0);
            TimeSpan ts2359 = new TimeSpan(23, 59, 59);
            TimeSpan ts0000 = new TimeSpan(0, 0, 1);
            TimeSpan ts0030 = new TimeSpan(0, 30, 0);
            TimeSpan ts0130 = new TimeSpan(1, 30, 0);
            TimeSpan ts0230 = new TimeSpan(2, 30, 0);
            TimeSpan ts0330 = new TimeSpan(3, 30, 0);
            TimeSpan ts0430 = new TimeSpan(4, 30, 0);

            int dataNum = listOrderRecord.Count();

            foreach (OrderRecord or in listOrderRecord)
            {
                if ((or.Time > ts0630) && (or.Time < ts0730))
                {
                    t07++;
                }
                else if ((or.Time > ts0730) && (or.Time < ts0830))
                {
                    t08++;
                }
                else if ((or.Time > ts0830) && (or.Time < ts0930))
                {
                    t09++;
                }
                else if ((or.Time > ts0930) && (or.Time < ts1030))
                {
                    t10++;
                }
                else if ((or.Time > ts1030) && (or.Time < ts1130))
                {
                    t11++;
                }
                else if ((or.Time > ts1130) && (or.Time < ts1230))
                {
                    t12++;
                }
                else if ((or.Time > ts1230) && (or.Time < ts1330))
                {
                    t13++;
                }
                else if ((or.Time > ts1330) && (or.Time < ts1430))
                {
                    t14++;
                }
                else if ((or.Time > ts1430) && (or.Time < ts1530))
                {
                    t15++;
                }
                else if ((or.Time > ts1530) && (or.Time < ts1630))
                {
                    t16++;
                }
                else if ((or.Time > ts1630) && (or.Time < ts1730))
                {
                    t17++;
                }
                else if ((or.Time > ts1730) && (or.Time < ts1830))
                {
                    t18++;
                }
                else if ((or.Time > ts1830) && (or.Time < ts1930))
                {
                    t19++;
                }
                else if ((or.Time > ts1930) && (or.Time < ts2030))
                {
                    t20++;
                }
                else if ((or.Time > ts2030) && (or.Time < ts2130))
                {
                    t21++;
                }
                else if ((or.Time > ts2130) && (or.Time < ts2230))
                {
                    t22++;
                }
                else if ((or.Time > ts2230) && (or.Time < ts2330))
                {
                    t23++;
                }
                else if ((or.Time > ts2330) && (or.Time < ts2359))
                {
                    t00++;
                }
                else if ((or.Time > ts0000) && (or.Time < ts0030))
                {
                    t00++;
                }
                else if ((or.Time > ts0030) && (or.Time < ts0130))
                {
                    t01++;
                }
                else if ((or.Time > ts0130) && (or.Time < ts0230))
                {
                    t02++;
                }
                else if ((or.Time > ts0230) && (or.Time < ts0330))
                {
                    t03++;
                }
                else if ((or.Time > ts0330) && (or.Time < ts0430))
                {
                    t04++;
                }
            }

            if (listOrderRecord.Count() == 0)
            {
                arrPrePareToJS[0] = new JsonProp() { x = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), y = t07 };
                arrPrePareToJS[1] = new JsonProp() { x = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t08 };
                arrPrePareToJS[2] = new JsonProp() { x = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t09 };
                arrPrePareToJS[3] = new JsonProp() { x = DateTime.Now.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t10 };
                arrPrePareToJS[4] = new JsonProp() { x = DateTime.Now.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t11 };
                arrPrePareToJS[5] = new JsonProp() { x = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = t12 };
                arrPrePareToJS[6] = new JsonProp() { x = DateTime.Now.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = t13 };
                arrPrePareToJS[7] = new JsonProp() { x = DateTime.Now.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), y = t14 };
                arrPrePareToJS[8] = new JsonProp() { x = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), y = t15 };
                arrPrePareToJS[9] = new JsonProp() { x = DateTime.Now.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"), y = t16 };
                arrPrePareToJS[10] = new JsonProp() { x = DateTime.Now.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"), y = t17 };
                arrPrePareToJS[11] = new JsonProp() { x = DateTime.Now.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"), y = t18 };
                arrPrePareToJS[12] = new JsonProp() { x = DateTime.Now.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"), y = t19 };
                //arrPrePareToJS[17] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.ToString("yyyy-MM-dd HH:mm:ss"), y = t00 };
                //arrPrePareToJS[18] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t01 };
                //arrPrePareToJS[19] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t02 };
                //arrPrePareToJS[20] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t03 };
                //arrPrePareToJS[21] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t04 };
                //arrPrePareToJS[22] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
                //arrPrePareToJS[23] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
            }
            else
            {
                arrPrePareToJS[0] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), y = t07 };
                arrPrePareToJS[1] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), y = t08 };
                arrPrePareToJS[2] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"), y = t09 };
                arrPrePareToJS[3] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"), y = t10 };
                arrPrePareToJS[4] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"), y = t11 };
                arrPrePareToJS[5] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"), y = t12 };
                arrPrePareToJS[6] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(13).ToString("yyyy-MM-dd HH:mm:ss"), y = t13 };
                arrPrePareToJS[7] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(14).ToString("yyyy-MM-dd HH:mm:ss"), y = t14 };
                arrPrePareToJS[8] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(15).ToString("yyyy-MM-dd HH:mm:ss"), y = t15 };
                arrPrePareToJS[9] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(16).ToString("yyyy-MM-dd HH:mm:ss"), y = t16 };
                arrPrePareToJS[10] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(17).ToString("yyyy-MM-dd HH:mm:ss"), y = t17 };
                arrPrePareToJS[11] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(18).ToString("yyyy-MM-dd HH:mm:ss"), y = t18 };
                arrPrePareToJS[12] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(19).ToString("yyyy-MM-dd HH:mm:ss"), y = t19 };
                arrPrePareToJS[13] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(20).ToString("yyyy-MM-dd HH:mm:ss"), y = t20 };
                arrPrePareToJS[14] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(21).ToString("yyyy-MM-dd HH:mm:ss"), y = t21 };
                arrPrePareToJS[15] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(22).ToString("yyyy-MM-dd HH:mm:ss"), y = t22 };
                arrPrePareToJS[16] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(23).ToString("yyyy-MM-dd HH:mm:ss"), y = t23 };
                arrPrePareToJS[17] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t00 };
                arrPrePareToJS[18] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t01 };
                arrPrePareToJS[19] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t02 };
                arrPrePareToJS[20] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t03 };
                arrPrePareToJS[21] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t04 };
                arrPrePareToJS[22] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
                arrPrePareToJS[23] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
                //arrPrePareToJS[0] = "{ x: " + listOrderRecord[0].Date.AddHours(7).ToString().Replace("/", "-") + ", y: " + t07 + " }";
                //arrPrePareToJS[1] = "{ x: " + listOrderRecord[0].Date.AddHours(8).ToString().Replace("/", "-") + ", y: " + t08 + " }";
                //arrPrePareToJS[2] = "{ x: " + listOrderRecord[0].Date.AddHours(9).ToString().Replace("/", "-") + ", y: " + t09 + " }";
                //arrPrePareToJS[3] = "{ x: " + listOrderRecord[0].Date.AddHours(10).ToString().Replace("/", "-") + ", y: " + t10 + " }";
                //arrPrePareToJS[4] = "{ x: " + listOrderRecord[0].Date.AddHours(11).ToString().Replace("/", "-") + ", y: " + t11 + " }";
                //arrPrePareToJS[5] = "{ x: " + listOrderRecord[0].Date.AddHours(12).ToString().Replace("/", "-") + ", y: " + t12 + " }";
                //arrPrePareToJS[6] = "{ x: " + listOrderRecord[0].Date.AddHours(13).ToString().Replace("/", "-") + ", y: " + t13 + " }";
                //arrPrePareToJS[7] = "{ x: " + listOrderRecord[0].Date.AddHours(14).ToString().Replace("/", "-") + ", y: " + t14 + " }";
                //arrPrePareToJS[8] = "{ x: " + listOrderRecord[0].Date.AddHours(15).ToString().Replace("/", "-") + ", y: " + t15 + " }";
                //arrPrePareToJS[9] = "{ x: " + listOrderRecord[0].Date.AddHours(16).ToString().Replace("/", "-") + ", y: " + t16 + " }";
                //arrPrePareToJS[10] = "{ x: " + listOrderRecord[0].Date.AddHours(17).ToString().Replace("/", "-") + ", y: " + t17 + " }";
                //arrPrePareToJS[11] = "{ x: " + listOrderRecord[0].Date.AddHours(18).ToString().Replace("/", "-") + ", y: " + t18 + " }";
                //arrPrePareToJS[12] = "{ x: " + listOrderRecord[0].Date.AddHours(19).ToString().Replace("/", "-") + ", y: " + t19 + " }";
                //arrPrePareToJS[13] = "{ x: " + listOrderRecord[0].Date.AddHours(20).ToString().Replace("/", "-") + ", y: " + t20 + " }";
                //arrPrePareToJS[14] = "{ x: " + listOrderRecord[0].Date.AddHours(21).ToString().Replace("/", "-") + ", y: " + t21 + " }";
                //arrPrePareToJS[15] = "{ x: " + listOrderRecord[0].Date.AddHours(22).ToString().Replace("/", "-") + ", y: " + t22 + " }";
                //arrPrePareToJS[16] = "{ x: " + listOrderRecord[0].Date.AddHours(23).ToString().Replace("/", "-") + ", y: " + t23 + " }";
                //arrPrePareToJS[17] = "{ x: " + listOrderRecord[dataNum - 1].Date.ToString().Replace("/", "-") + ", y: " + t00 + " }";
                //arrPrePareToJS[18] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(1).ToString().Replace("/", "-") + ", y: " + t01 + " }";
                //arrPrePareToJS[19] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(2).ToString().Replace("/", "-") + ", y: " + t02 + " }";
                //arrPrePareToJS[20] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(3).ToString().Replace("/", "-") + ", y: " + t03 + " }";
                //arrPrePareToJS[21] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(4).ToString().Replace("/", "-") + ", y: " + t04 + " }";
                //arrPrePareToJS[22] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(5).ToString().Replace("/", "-") + ", y: 0 }";
                //arrPrePareToJS[23] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(6).ToString().Replace("/", "-") + ", y: 0 }";
            }
            return arrPrePareToJS;
        }
        public IEnumerable<List<OrderRecord>> getBestSellerInMonth(int branchId, List<Account> listAccountId)
        {
            IEnumerable<List<OrderRecord>> groupedObjects;
            int getFirstId = 50000000;
            int getLastId = 60000000;
            if (listAccountId.Count() != 0)
            {
                getFirstId = listAccountId[0].Id;
                getLastId = listAccountId[listAccountId.Count() - 1].Id;
            }
            //List<OrderRecord> listOrderTopA = new List<OrderRecord>();
            //List<OrderRecord> listOrderTopB = new List<OrderRecord>();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                groupedObjects = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId >= getFirstId && b.AccountId <= getLastId && b.CancelStatus == "false")
                                .GroupBy(b => b.MassageTopicId)
                                .Select(group => group.ToList())
                                .OrderByDescending(c => c.Count())
                                .ToList();
            }

            //foreach(var a in groupedObjects)
            //{
            //    if(listOrderTopA.Count==0)
            //    {
            //        listOrderTopA = a;
            //    }
            //    else if(listOrderTopB.Count()==0)
            //    {
            //        listOrderTopB = a;
            //    }
            //}


            return groupedObjects;
        }
        public List<FinalSaleForEachTopic> getFinalSaleForEachInMonth(int branchId, List<Account> listAccountId, int massageSetId)
        {
            int getFirstId = 50000000;
            int getLastId = 60000000;
            if (listAccountId.Count() != 0)
            {
                getFirstId = listAccountId[0].Id;
                getLastId = listAccountId[listAccountId.Count() - 1].Id;
            }
            List<int> listMassageTopic;
            List<FinalSaleForEachTopic> listOfFinalSale = new List<FinalSaleForEachTopic>();
            List<OrderRecord> _temp;
            using (var contexts = new spasystemdbEntities())
            {

                listMassageTopic = contexts.MassageSets
                                .Where(b => b.Id == massageSetId)
                                .Select(b => b.MassageTopicId)
                                .ToList();
            }

            var set = new HashSet<int>(listMassageTopic);
            List<int> finalListMassageTopicId = set.ToList<int>();

            for (int j = 0; j < finalListMassageTopicId.Count(); j++)
            {
                int getId = finalListMassageTopicId[j];
                int totalSale = 0;
                using (var contexts = new spasystemdbEntities())
                {

                    _temp = contexts.OrderRecords
                                    .Where(b => b.BranchId == branchId && b.AccountId >= getFirstId && b.AccountId <= getLastId && b.MassageTopicId == getId && b.CancelStatus == "false")
                                    .ToList();
                }

                for (int u = 0; u < _temp.Count(); u++)
                {
                    totalSale += _temp[u].Price;
                }

                FinalSaleForEachTopic fet = new FinalSaleForEachTopic()
                {
                    MassageTopicName = getTopicName(getId),
                    TotalPax = String.Format("{0:n0}", _temp.Count()),
                    TotalSale = String.Format("{0:n}", totalSale)
                };

                listOfFinalSale.Add(fet);
            }

            return listOfFinalSale;

        }

        //------------------------- Year -------------------------------------------
        public int getTotalSaleInYear(int branchId, int accountId)
        {
            var totalSales = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalSales = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Price)
                                .ToList().Sum();
            }



            return totalSales;
        }

        public int getTotalCommissionInYear(int branchId, int accountId)
        {
            var totalComs = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalComs = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                                .Select(b => (int)b.Commission)
                                .ToList().Sum();
            }



            return totalComs;
        }
        public int getTotalOtherSaleInYear(int branchId, int accountId)
        {
            var totalComs = new int();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalComs = context.OtherSaleRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.OtherSaleId == 3 && b.CancelStatus == "false")
                                .Select(b => (int)b.Price)
                                .ToList().Sum();
            }



            return totalComs;
        }
        public JsonProp[] getOrderRecordForGraphInYear(int branchId, List<Account> listAccountId)
        {
            List<OrderRecord> listOrderRecord = new List<OrderRecord>();
            JsonProp[] arrPrePareToJS = new JsonProp[24];
            int getFirstId = 50000000;
            int getLastId = 60000000;
            if (listAccountId.Count() != 0)
            {
                getFirstId = listAccountId[0].Id;
                getLastId = listAccountId[listAccountId.Count() - 1].Id;
            }
            //var totalComs = new int();
            //DateTime current = DateTime.Now;
            //string curDateTime = current.ToString("yyyy-MM-dd");
            //DateTime dtStart = DateTime.ParseExact(curDateTime + " 05:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            //DateTime tomorrow = DateTime.Now.AddDays(1);
            //string tmrDateTime = tomorrow.ToString("yyyy-MM-dd");
            //DateTime dtEnd = DateTime.ParseExact(tmrDateTime + " 05:00:00", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog
                //.AddDays(36)


                listOrderRecord = context.OrderRecords
                    .Where(r => r.BranchId == branchId && r.AccountId >= getFirstId && r.AccountId <= getLastId && r.CancelStatus == "false")
                    .ToList();

                //totalComs = context.OrderRecords
                //                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false")
                //                .Select(b => (int)b.Commission)
                //                .ToList().Sum();
            }

            int t07 = 0, t08 = 0, t09 = 0, t10 = 0, t11 = 0, t12 = 0, t13 = 0, t14 = 0, t15 = 0, t16 = 0, t17 = 0, t18 = 0, t19 = 0, t20 = 0, t21 = 0, t22 = 0, t23 = 0, t00 = 0, t01 = 0, t02 = 0, t03 = 0, t04 = 0;
            TimeSpan ts0630 = new TimeSpan(6, 30, 0);
            TimeSpan ts0730 = new TimeSpan(7, 30, 0);
            TimeSpan ts0830 = new TimeSpan(8, 30, 0);
            TimeSpan ts0930 = new TimeSpan(9, 30, 0);
            TimeSpan ts1030 = new TimeSpan(10, 30, 0);
            TimeSpan ts1130 = new TimeSpan(11, 30, 0);
            TimeSpan ts1230 = new TimeSpan(12, 30, 0);
            TimeSpan ts1330 = new TimeSpan(13, 30, 0);
            TimeSpan ts1430 = new TimeSpan(14, 30, 0);
            TimeSpan ts1530 = new TimeSpan(15, 30, 0);
            TimeSpan ts1630 = new TimeSpan(16, 30, 0);
            TimeSpan ts1730 = new TimeSpan(17, 30, 0);
            TimeSpan ts1830 = new TimeSpan(18, 30, 0);
            TimeSpan ts1930 = new TimeSpan(19, 30, 0);
            TimeSpan ts2030 = new TimeSpan(20, 30, 0);
            TimeSpan ts2130 = new TimeSpan(21, 30, 0);
            TimeSpan ts2230 = new TimeSpan(22, 30, 0);
            TimeSpan ts2330 = new TimeSpan(23, 30, 0);
            TimeSpan ts2359 = new TimeSpan(23, 59, 59);
            TimeSpan ts0000 = new TimeSpan(0, 0, 1);
            TimeSpan ts0030 = new TimeSpan(0, 30, 0);
            TimeSpan ts0130 = new TimeSpan(1, 30, 0);
            TimeSpan ts0230 = new TimeSpan(2, 30, 0);
            TimeSpan ts0330 = new TimeSpan(3, 30, 0);
            TimeSpan ts0430 = new TimeSpan(4, 30, 0);

            int dataNum = listOrderRecord.Count();

            foreach (OrderRecord or in listOrderRecord)
            {
                if ((or.Time > ts0630) && (or.Time < ts0730))
                {
                    t07++;
                }
                else if ((or.Time > ts0730) && (or.Time < ts0830))
                {
                    t08++;
                }
                else if ((or.Time > ts0830) && (or.Time < ts0930))
                {
                    t09++;
                }
                else if ((or.Time > ts0930) && (or.Time < ts1030))
                {
                    t10++;
                }
                else if ((or.Time > ts1030) && (or.Time < ts1130))
                {
                    t11++;
                }
                else if ((or.Time > ts1130) && (or.Time < ts1230))
                {
                    t12++;
                }
                else if ((or.Time > ts1230) && (or.Time < ts1330))
                {
                    t13++;
                }
                else if ((or.Time > ts1330) && (or.Time < ts1430))
                {
                    t14++;
                }
                else if ((or.Time > ts1430) && (or.Time < ts1530))
                {
                    t15++;
                }
                else if ((or.Time > ts1530) && (or.Time < ts1630))
                {
                    t16++;
                }
                else if ((or.Time > ts1630) && (or.Time < ts1730))
                {
                    t17++;
                }
                else if ((or.Time > ts1730) && (or.Time < ts1830))
                {
                    t18++;
                }
                else if ((or.Time > ts1830) && (or.Time < ts1930))
                {
                    t19++;
                }
                else if ((or.Time > ts1930) && (or.Time < ts2030))
                {
                    t20++;
                }
                else if ((or.Time > ts2030) && (or.Time < ts2130))
                {
                    t21++;
                }
                else if ((or.Time > ts2130) && (or.Time < ts2230))
                {
                    t22++;
                }
                else if ((or.Time > ts2230) && (or.Time < ts2330))
                {
                    t23++;
                }
                else if ((or.Time > ts2330) && (or.Time < ts2359))
                {
                    t00++;
                }
                else if ((or.Time > ts0000) && (or.Time < ts0030))
                {
                    t00++;
                }
                else if ((or.Time > ts0030) && (or.Time < ts0130))
                {
                    t01++;
                }
                else if ((or.Time > ts0130) && (or.Time < ts0230))
                {
                    t02++;
                }
                else if ((or.Time > ts0230) && (or.Time < ts0330))
                {
                    t03++;
                }
                else if ((or.Time > ts0330) && (or.Time < ts0430))
                {
                    t04++;
                }
            }

            if (listOrderRecord.Count() == 0)
            {
                arrPrePareToJS[0] = new JsonProp() { x = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), y = t07 };
                arrPrePareToJS[1] = new JsonProp() { x = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t08 };
                arrPrePareToJS[2] = new JsonProp() { x = DateTime.Now.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t09 };
                arrPrePareToJS[3] = new JsonProp() { x = DateTime.Now.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t10 };
                arrPrePareToJS[4] = new JsonProp() { x = DateTime.Now.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t11 };
                arrPrePareToJS[5] = new JsonProp() { x = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = t12 };
                arrPrePareToJS[6] = new JsonProp() { x = DateTime.Now.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = t13 };
                arrPrePareToJS[7] = new JsonProp() { x = DateTime.Now.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), y = t14 };
                arrPrePareToJS[8] = new JsonProp() { x = DateTime.Now.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), y = t15 };
                arrPrePareToJS[9] = new JsonProp() { x = DateTime.Now.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"), y = t16 };
                arrPrePareToJS[10] = new JsonProp() { x = DateTime.Now.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"), y = t17 };
                arrPrePareToJS[11] = new JsonProp() { x = DateTime.Now.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"), y = t18 };
                arrPrePareToJS[12] = new JsonProp() { x = DateTime.Now.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"), y = t19 };
                //arrPrePareToJS[17] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.ToString("yyyy-MM-dd HH:mm:ss"), y = t00 };
                //arrPrePareToJS[18] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t01 };
                //arrPrePareToJS[19] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t02 };
                //arrPrePareToJS[20] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t03 };
                //arrPrePareToJS[21] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t04 };
                //arrPrePareToJS[22] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
                //arrPrePareToJS[23] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
            }
            else
            {
                arrPrePareToJS[0] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(7).ToString("yyyy-MM-dd HH:mm:ss"), y = t07 };
                arrPrePareToJS[1] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"), y = t08 };
                arrPrePareToJS[2] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"), y = t09 };
                arrPrePareToJS[3] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"), y = t10 };
                arrPrePareToJS[4] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"), y = t11 };
                arrPrePareToJS[5] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"), y = t12 };
                arrPrePareToJS[6] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(13).ToString("yyyy-MM-dd HH:mm:ss"), y = t13 };
                arrPrePareToJS[7] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(14).ToString("yyyy-MM-dd HH:mm:ss"), y = t14 };
                arrPrePareToJS[8] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(15).ToString("yyyy-MM-dd HH:mm:ss"), y = t15 };
                arrPrePareToJS[9] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(16).ToString("yyyy-MM-dd HH:mm:ss"), y = t16 };
                arrPrePareToJS[10] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(17).ToString("yyyy-MM-dd HH:mm:ss"), y = t17 };
                arrPrePareToJS[11] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(18).ToString("yyyy-MM-dd HH:mm:ss"), y = t18 };
                arrPrePareToJS[12] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(19).ToString("yyyy-MM-dd HH:mm:ss"), y = t19 };
                arrPrePareToJS[13] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(20).ToString("yyyy-MM-dd HH:mm:ss"), y = t20 };
                arrPrePareToJS[14] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(21).ToString("yyyy-MM-dd HH:mm:ss"), y = t21 };
                arrPrePareToJS[15] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(22).ToString("yyyy-MM-dd HH:mm:ss"), y = t22 };
                arrPrePareToJS[16] = new JsonProp() { x = listOrderRecord[0].Date.AddHours(23).ToString("yyyy-MM-dd HH:mm:ss"), y = t23 };
                arrPrePareToJS[17] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t00 };
                arrPrePareToJS[18] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t01 };
                arrPrePareToJS[19] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t02 };
                arrPrePareToJS[20] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t03 };
                arrPrePareToJS[21] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t04 };
                arrPrePareToJS[22] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
                arrPrePareToJS[23] = new JsonProp() { x = listOrderRecord[0].Date.AddDays(1).AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
                //arrPrePareToJS[0] = "{ x: " + listOrderRecord[0].Date.AddHours(7).ToString().Replace("/", "-") + ", y: " + t07 + " }";
                //arrPrePareToJS[1] = "{ x: " + listOrderRecord[0].Date.AddHours(8).ToString().Replace("/", "-") + ", y: " + t08 + " }";
                //arrPrePareToJS[2] = "{ x: " + listOrderRecord[0].Date.AddHours(9).ToString().Replace("/", "-") + ", y: " + t09 + " }";
                //arrPrePareToJS[3] = "{ x: " + listOrderRecord[0].Date.AddHours(10).ToString().Replace("/", "-") + ", y: " + t10 + " }";
                //arrPrePareToJS[4] = "{ x: " + listOrderRecord[0].Date.AddHours(11).ToString().Replace("/", "-") + ", y: " + t11 + " }";
                //arrPrePareToJS[5] = "{ x: " + listOrderRecord[0].Date.AddHours(12).ToString().Replace("/", "-") + ", y: " + t12 + " }";
                //arrPrePareToJS[6] = "{ x: " + listOrderRecord[0].Date.AddHours(13).ToString().Replace("/", "-") + ", y: " + t13 + " }";
                //arrPrePareToJS[7] = "{ x: " + listOrderRecord[0].Date.AddHours(14).ToString().Replace("/", "-") + ", y: " + t14 + " }";
                //arrPrePareToJS[8] = "{ x: " + listOrderRecord[0].Date.AddHours(15).ToString().Replace("/", "-") + ", y: " + t15 + " }";
                //arrPrePareToJS[9] = "{ x: " + listOrderRecord[0].Date.AddHours(16).ToString().Replace("/", "-") + ", y: " + t16 + " }";
                //arrPrePareToJS[10] = "{ x: " + listOrderRecord[0].Date.AddHours(17).ToString().Replace("/", "-") + ", y: " + t17 + " }";
                //arrPrePareToJS[11] = "{ x: " + listOrderRecord[0].Date.AddHours(18).ToString().Replace("/", "-") + ", y: " + t18 + " }";
                //arrPrePareToJS[12] = "{ x: " + listOrderRecord[0].Date.AddHours(19).ToString().Replace("/", "-") + ", y: " + t19 + " }";
                //arrPrePareToJS[13] = "{ x: " + listOrderRecord[0].Date.AddHours(20).ToString().Replace("/", "-") + ", y: " + t20 + " }";
                //arrPrePareToJS[14] = "{ x: " + listOrderRecord[0].Date.AddHours(21).ToString().Replace("/", "-") + ", y: " + t21 + " }";
                //arrPrePareToJS[15] = "{ x: " + listOrderRecord[0].Date.AddHours(22).ToString().Replace("/", "-") + ", y: " + t22 + " }";
                //arrPrePareToJS[16] = "{ x: " + listOrderRecord[0].Date.AddHours(23).ToString().Replace("/", "-") + ", y: " + t23 + " }";
                //arrPrePareToJS[17] = "{ x: " + listOrderRecord[dataNum - 1].Date.ToString().Replace("/", "-") + ", y: " + t00 + " }";
                //arrPrePareToJS[18] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(1).ToString().Replace("/", "-") + ", y: " + t01 + " }";
                //arrPrePareToJS[19] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(2).ToString().Replace("/", "-") + ", y: " + t02 + " }";
                //arrPrePareToJS[20] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(3).ToString().Replace("/", "-") + ", y: " + t03 + " }";
                //arrPrePareToJS[21] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(4).ToString().Replace("/", "-") + ", y: " + t04 + " }";
                //arrPrePareToJS[22] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(5).ToString().Replace("/", "-") + ", y: 0 }";
                //arrPrePareToJS[23] = "{ x: " + listOrderRecord[dataNum - 1].Date.AddHours(6).ToString().Replace("/", "-") + ", y: 0 }";
            }
            return arrPrePareToJS;
        }
        public IEnumerable<List<OrderRecord>> getBestSellerInYear(int branchId, List<Account> listAccountId)
        {
            IEnumerable<List<OrderRecord>> groupedObjects;
            int getFirstId = 50000000;
            int getLastId = 60000000;
            if (listAccountId.Count() != 0)
            {
                getFirstId = listAccountId[0].Id;
                getLastId = listAccountId[listAccountId.Count() - 1].Id;
            }
            //List<OrderRecord> listOrderTopA = new List<OrderRecord>();
            //List<OrderRecord> listOrderTopB = new List<OrderRecord>();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                groupedObjects = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId >= getFirstId && b.AccountId <= getLastId && b.CancelStatus == "false")
                                .GroupBy(b => b.MassageTopicId)
                                .Select(group => group.ToList())
                                .OrderByDescending(c => c.Count())
                                .ToList();
            }

            //foreach(var a in groupedObjects)
            //{
            //    if(listOrderTopA.Count==0)
            //    {
            //        listOrderTopA = a;
            //    }
            //    else if(listOrderTopB.Count()==0)
            //    {
            //        listOrderTopB = a;
            //    }
            //}


            return groupedObjects;
        }
        public List<FinalSaleForEachTopic> getFinalSaleForEachInYear(int branchId, List<Account> listAccountId, int massageSetId)
        {
            int getFirstId = 50000000;
            int getLastId = 60000000;
            if (listAccountId.Count() != 0)
            {
                getFirstId = listAccountId[0].Id;
                getLastId = listAccountId[listAccountId.Count() - 1].Id;
            }
            List<int> listMassageTopic;
            List<FinalSaleForEachTopic> listOfFinalSale = new List<FinalSaleForEachTopic>();
            List<OrderRecord> _temp;
            using (var contexts = new spasystemdbEntities())
            {

                listMassageTopic = contexts.MassageSets
                                .Where(b => b.Id == massageSetId)
                                .Select(b => b.MassageTopicId)
                                .ToList();
            }

            var set = new HashSet<int>(listMassageTopic);
            List<int> finalListMassageTopicId = set.ToList<int>();

            for (int j = 0; j < finalListMassageTopicId.Count(); j++)
            {
                int getId = finalListMassageTopicId[j];
                int totalSale = 0;
                using (var contexts = new spasystemdbEntities())
                {

                    _temp = contexts.OrderRecords
                                    .Where(b => b.BranchId == branchId && b.AccountId >= getFirstId && b.AccountId <= getLastId && b.MassageTopicId == getId && b.CancelStatus == "false")
                                    .ToList();
                }

                for (int u = 0; u < _temp.Count(); u++)
                {
                    totalSale += _temp[u].Price;
                }

                FinalSaleForEachTopic fet = new FinalSaleForEachTopic()
                {
                    MassageTopicName = getTopicName(getId),
                    TotalPax = String.Format("{0:n0}", _temp.Count()),
                    TotalSale = String.Format("{0:n}", totalSale)
                };

                listOfFinalSale.Add(fet);
            }

            return listOfFinalSale;

        }

        public Member getLastestMember()
        {
            Member mem = new Member();

            using (var contexts = new spasystemdbEntities())
            {

                mem = contexts.Members
                                .OrderByDescending(b => b.Id)
                                .FirstOrDefault();
            }


            return mem;

        }

        public Member getMember(int memberId)
        {
            Member mem = new Member();

            using (var contexts = new spasystemdbEntities())
            {

                mem = contexts.Members
                                .Where(b => b.Id == memberId)
                                .FirstOrDefault();
            }


            return mem;

        }

        public MemberDetail getMemberDetail(int memberId)
        {
            MemberDetail memDetail = new MemberDetail();

            using (var contexts = new spasystemdbEntities())
            {

                memDetail = contexts.MemberDetails
                                .Where(b => b.MemberId == memberId)
                                .FirstOrDefault();
            }

            
            return memDetail;

        }

        public MemberGroup getMemberGroupDetail(int memberGroupId)
        {
            MemberGroup memGroupDetail = new MemberGroup();

            using (var contexts = new spasystemdbEntities())
            {

                memGroupDetail = contexts.MemberGroups
                                .Where(b => b.Id == memberGroupId)
                                .FirstOrDefault();
            }


            return memGroupDetail;

        }

        public MemberPriviledge getMemberPriviledgeDetail(int memberPriviledgeId)
        {
            MemberPriviledge memPriviledgeDetail = new MemberPriviledge();

            using (var contexts = new spasystemdbEntities())
            {

                memPriviledgeDetail = contexts.MemberPriviledges
                                .Where(b => b.Id == memberPriviledgeId)
                                .FirstOrDefault();
            }


            return memPriviledgeDetail;

        }
        public PriviledgeType getPriviledgeTypeDetail(int priviledgeId)
        {
            PriviledgeType privType = new PriviledgeType();

            using (var contexts = new spasystemdbEntities())
            {

                privType = contexts.PriviledgeTypes
                                .Where(b => b.Id == priviledgeId)
                                .FirstOrDefault();
            }


            return privType;

        }

        public List<MemberGroup> getAllMemberGroup()
        {
            List<MemberGroup> allMemGroup = new List<MemberGroup>();

            using (var contexts = new spasystemdbEntities())
            {

                allMemGroup = contexts.MemberGroups
                                .Where(b => b.Status == "true")
                                .ToList();
            }


            return allMemGroup;

        }

        public List<MemberPriviledge> getAllMemberPriviledge()
        {
            List<MemberPriviledge> allMemPriviledge = new List<MemberPriviledge>();

            using (var contexts = new spasystemdbEntities())
            {

                allMemPriviledge = contexts.MemberPriviledges
                                .ToList();
            }


            return allMemPriviledge;

        }


        public List<PriviledgeType> getAllPriviledgeType()
        {
            List<PriviledgeType> allPriviledgeType = new List<PriviledgeType>();

            using (var contexts = new spasystemdbEntities())
            {

                allPriviledgeType = contexts.PriviledgeTypes
                                .ToList();
            }


            return allPriviledgeType;

        }

        public List<MemberGroupPriviledge> getMemberGroupPriviledge(int memberGroupId)
        {
            List<MemberGroupPriviledge> memGroupPriv = new List<MemberGroupPriviledge>();

            using (var contexts = new spasystemdbEntities())
            {

                memGroupPriv = contexts.MemberGroupPriviledges
                                .Where(b => b.MemberGroupId == memberGroupId)
                                .ToList();
            }


            return memGroupPriv;

        }

        public int getTotalVipAmount(int branchId, int accountId)
        {
            int totalVip = 0;

            using (var context = new spasystemdbEntities())
            {


                totalVip = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.MemberId != 0)
                                .ToList().Count();
            }

            return totalVip;
        }

        public int getTotalVipAmount_B(int branchId, int accountId, int sellItemTypeId)
        {
            int totalVip = 0;

            using (var context = new spasystemdbEntities())
            {
                List<int> orderReceipts = context.OrderReceipts
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.SellItemTypeId == sellItemTypeId)
                                .Select(b => b.Id)
                                .ToList();

                // Fetch all relevant OrderRecords
                totalVip = (from or in context.OrderRecords
                                    where or.BranchId == branchId
                                          && orderReceipts.Contains((int)or.OrderReceiptId)
                                          && or.CancelStatus == "false"
                                          && or.MemberId != 0
                                    select or.Id).ToList().Count();

                //totalVip = context.OrderRecords
                //                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.MemberId != 0)
                //                .ToList().Count();
            }

            return totalVip;
        }

        public int getCash(int branchId, int accountId)
        {
            using (var context = new spasystemdbEntities())
            {
                // Perform the sum directly in the database query
                int totalCash = context.OrderRecords
                                        .Where(b => b.BranchId == branchId
                                                    && b.AccountId == accountId
                                                    && b.CancelStatus == "false"
                                                    && b.IsCreditCard == "false")
                                        .Sum(b => (int?)b.Price) ?? 0; // Use nullable int to handle potential null values

                return totalCash;
            }
        }

        public int getCredit(int branchId, int accountId)
        {
            using (var context = new spasystemdbEntities())
            {
                // Perform the sum directly in the database query
                int totalCredit = context.OrderRecords
                                          .Where(b => b.BranchId == branchId
                                                      && b.AccountId == accountId
                                                      && b.CancelStatus == "false"
                                                      && b.IsCreditCard == "true")
                                          .Sum(b => (int?)b.Price) ?? 0; // Use nullable int to handle potential null values

                return totalCredit;
            }
        }


        public int getVoucherCash(int branchId, int accountId)
        {
            using (var context = new spasystemdbEntities())
            {
                // Fetch the relevant DiscountRecords from the database
                var discountValues = context.DiscountRecords
                                            .Where(b => b.BranchId == branchId
                                                        && b.AccountId == accountId
                                                        && b.CancelStatus == "false"
                                                        && b.IsCreditCard == "false")
                                            .Select(b => b.Value)
                                            .ToList();

                // Sum the parsed integer values in memory
                int discountWithCash = discountValues
                                       .Where(value => !string.IsNullOrEmpty(value))
                                       .Sum(value => Int32.Parse(value));

                return discountWithCash;
            }
        }



        public int getVoucherCredit(int branchId, int accountId)
        {
            using (var context = new spasystemdbEntities())
            {
                // Fetch the relevant DiscountRecords from the database
                var discountValues = context.DiscountRecords
                                            .Where(b => b.BranchId == branchId
                                                        && b.AccountId == accountId
                                                        && b.CancelStatus == "false"
                                                        && b.IsCreditCard == "true")
                                            .Select(b => b.Value)
                                            .ToList();

                // Sum the parsed integer values in memory
                int discountWithCredit = discountValues
                                         .Where(value => !string.IsNullOrEmpty(value))
                                         .Sum(value => Int32.Parse(value));

                return discountWithCredit;
            }
        }

        public class CashCreditResult
        {
            public int TotalCash { get; set; }
            public int TotalCredit { get; set; }
        }

        public CashCreditResult GetTotalCashAndCredit(int branchId, int accountId)
        {
            using (var context = new spasystemdbEntities())
            {
                var data = (from or in context.OrderRecords
                            where or.BranchId == branchId && or.AccountId == accountId && or.CancelStatus == "false"
                            select new
                            {
                                Price = (int?)or.Price,
                                or.IsCreditCard
                            })
                            .ToList();

                var discountData = (from dr in context.DiscountRecords
                                    where dr.BranchId == branchId && dr.AccountId == accountId && dr.CancelStatus == "false"
                                    select new
                                    {
                                        dr.Value,
                                        dr.IsCreditCard
                                    })
                                    .ToList();

                int totalCash = data.Where(or => or.IsCreditCard == "false").Sum(or => or.Price ?? 0)
                               - discountData.Where(dr => dr.IsCreditCard == "false").Sum(dr => Int32.Parse(dr.Value));

                int totalCredit = data.Where(or => or.IsCreditCard == "true").Sum(or => or.Price ?? 0)
                                 - discountData.Where(dr => dr.IsCreditCard == "true").Sum(dr => Int32.Parse(dr.Value));

                return new CashCreditResult
                {
                    TotalCash = totalCash,
                    TotalCredit = totalCredit
                };
            }
        }

        public class CashCreditSummary
        {
            public int TotalCash { get; set; }
            public int TotalCredit { get; set; }
        }


        public CashCreditSummary GetTotalCashAndCredit_multiAcc(int branchId, List<int> accountIds)
        {
            using (var context = new spasystemdbEntities())
            {
                // Fetch all relevant OrderRecords
                var orderResults = (from or in context.OrderRecords
                                    where or.BranchId == branchId
                                          && accountIds.Contains(or.AccountId)
                                          && or.CancelStatus == "false"
                                    group or by or.IsCreditCard into grouped
                                    select new
                                    {
                                        IsCreditCard = grouped.Key,
                                        TotalPrice = grouped.Sum(or => (int?)or.Price) ?? 0
                                    }).ToList();

                // Fetch all relevant DiscountRecords
                var discountData = (from dr in context.DiscountRecords
                                    where dr.BranchId == branchId
                                          && accountIds.Contains(dr.AccountId)
                                          && dr.CancelStatus == "false"
                                    select new
                                    {
                                        dr.Value,
                                        dr.IsCreditCard
                                    }).ToList();

                // Sum the parsed integer values in memory
                int totalCashDiscount = discountData
                    .Where(dr => dr.IsCreditCard == "false" && !string.IsNullOrEmpty(dr.Value))
                    .Sum(dr => Int32.Parse(dr.Value));

                int totalCreditDiscount = discountData
                    .Where(dr => dr.IsCreditCard == "true" && !string.IsNullOrEmpty(dr.Value))
                    .Sum(dr => Int32.Parse(dr.Value));

                // Sum the values for cash and credit
                int totalCash = orderResults
                    .Where(or => or.IsCreditCard == "false")
                    .Sum(or => or.TotalPrice) - totalCashDiscount;

                int totalCredit = orderResults
                    .Where(or => or.IsCreditCard == "true")
                    .Sum(or => or.TotalPrice) - totalCreditDiscount;

                return new CashCreditSummary
                {
                    TotalCash = totalCash,
                    TotalCredit = totalCredit
                };
            }
        }

        public CashCreditSummary GetTotalCashAndCredit_multiAcc_B(int branchId, int accountId, int sellItemTypeId)
        {
            using (var context = new spasystemdbEntities())
            {
                List<int> orderReceipts = context.OrderReceipts
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.SellItemTypeId == sellItemTypeId)
                                .Select(b => b.Id)
                                .ToList();

                // Fetch all relevant OrderRecords
                var orderResults = (from or in context.OrderRecords
                                    where or.BranchId == branchId
                                          && orderReceipts.Contains((int)or.OrderReceiptId)
                                          && or.CancelStatus == "false"
                                    group or by or.IsCreditCard into grouped
                                    select new
                                    {
                                        IsCreditCard = grouped.Key,
                                        TotalPrice = grouped.Sum(or => (int?)or.Price) ?? 0
                                    }).ToList();

                // Fetch all relevant DiscountRecords
                var discountData = (from dr in context.DiscountRecords
                                    where dr.BranchId == branchId
                                          && orderReceipts.Contains((int)dr.OrderReceiptId)
                                          && dr.CancelStatus == "false"
                                    select new
                                    {
                                        dr.Value,
                                        dr.IsCreditCard
                                    }).ToList();

                // Sum the parsed integer values in memory
                int totalCashDiscount = discountData
                    .Where(dr => dr.IsCreditCard == "false" && !string.IsNullOrEmpty(dr.Value))
                    .Sum(dr => Int32.Parse(dr.Value));

                int totalCreditDiscount = discountData
                    .Where(dr => dr.IsCreditCard == "true" && !string.IsNullOrEmpty(dr.Value))
                    .Sum(dr => Int32.Parse(dr.Value));

                // Sum the values for cash and credit
                int totalCash = orderResults
                    .Where(or => or.IsCreditCard == "false")
                    .Sum(or => or.TotalPrice) - totalCashDiscount;

                int totalCredit = orderResults
                    .Where(or => or.IsCreditCard == "true")
                    .Sum(or => or.TotalPrice) - totalCreditDiscount;

                return new CashCreditSummary
                {
                    TotalCash = totalCash,
                    TotalCredit = totalCredit
                };
            }
        }


        public string CheckUserIsEnable(string userName)
        {
            string userStatus = "";
            User userDetail = new User();

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                userDetail = context.Users
                                .Where(b => b.Username == userName)
                                .FirstOrDefault();

                try
                {
                    if (userDetail.Status.Equals("true"))
                    {
                        if (userDetail.UrbanSystem.Equals("true"))
                        {
                            userStatus = "true";
                        }
                        else
                        {
                            userStatus = "false";
                        }
                    }
                    else
                    {
                        userStatus = "false";
                    }
                }
                catch(Exception io)
                {
                    userStatus = "false";
                }
                
            }

            return userStatus;
        }

        public void RemoveCookie()
        {
            if (Request.Cookies["TokenCookie"] != null)
            {
                var _cookie = new HttpCookie("TokenCookie")
                {
                    Expires = DateTime.Now.AddDays(-1), // Expire the cookie
                                                        //Secure = true
                };
                var _cookie_user = new HttpCookie("UserCookie")
                {
                    Expires = DateTime.Now.AddDays(-1), // Expire the cookie
                                                        //Secure = true
                };
                Response.Cookies.Add(_cookie);
                Response.Cookies.Add(_cookie_user);
            }
        }

        public string getBranchName(int branchId)
        {
            string bName;

            using (var context = new spasystemdbEntities())
            {
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                bName = context.Branches
                                .Where(b => b.Id == branchId)
                                .Select(s => s.Name)
                                .FirstOrDefault();
            }

            return bName;
        }

        //public int getSumDiscount_B(int branchId, int accountId, int sellItemTypeId)
        //{
        //    int sumDiscount = 0;
        //    using (var context = new spasystemdbEntities())
        //    {
        //        List<int> orderReceipts = context.OrderReceipts
        //                        .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.SellItemTypeId == sellItemTypeId)
        //                        .Select(b => b.Id)
        //                        .ToList();

        //        sumDiscount = (from dr in context.DiscountRecords
        //                            where dr.BranchId == branchId
        //                                  && orderReceipts.Contains((int)dr.OrderReceiptId)
        //                                  && dr.CancelStatus == "false"
        //                            select dr.Value).Sum();
        //    }

        //    return sumDiscount;
        //}

        public int getSumDiscount_B(int branchId, int accountId, int sellItemTypeId)
        {
            int sumDiscount = 0;

            using (var context = new spasystemdbEntities())
            {
                List<int> orderReceipts = context.OrderReceipts
                                    .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.SellItemTypeId == sellItemTypeId)
                                    .Select(b => b.Id)
                                    .ToList();

                sumDiscount = context.DiscountRecords
                                     .Where(dr => dr.BranchId == branchId
                                                  && orderReceipts.Contains((int)dr.OrderReceiptId)
                                                  && dr.CancelStatus == "false")
                                     .AsEnumerable() // Execute the query in memory
                                     .Sum(dr =>
                                     {
                                         int parsedValue;
                                         return int.TryParse(dr.Value, out parsedValue) ? parsedValue : 0;
                                     });
            }

            return sumDiscount;
        }



    }
    public static class ExtensionHelpers
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }

    public class AccountFMD
    {
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string FormattedCreateDateTime { get; set; } // Add this property
    }

}