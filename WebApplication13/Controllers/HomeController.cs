using System;
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

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Dashboard");
            }
            
            
        }

        [HttpPost]
        public ActionResult Index(UserLogin userModel)
        {
            // if credentials are correct.
            User checkUs = getUserAuthen(userModel);
            if (checkUs != null)
            {
                // login user logic here.
                // redirect to home page.

                var noms = System.Runtime.Caching.MemoryCache.Default["names"];
                noms = checkUs.Name;
                System.Runtime.Caching.MemoryCache.Default["names"] = noms;
                return RedirectToAction("Dashboard");
            }
            else
            {
                // show login page again.
                return View();
            }
        }

        public ActionResult Dashboard(string cmd)
        {

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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

                dataReader.Close();
                command.Dispose();
                cnn.Close();

                return View(hv);


            }
        }

        public ActionResult Urban(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 1;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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
                    

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
                    };


                    return View(hv);
                }
            }
        }

        public ActionResult Khaosan(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 2;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
                    };


                    return View(hv);
                }
            }
        }

        public ActionResult UrbanTwo(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 3;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strBalanceNet = String.Format("{0:n0}", ((convert_tSales + convert_tOil + convert_tOtherS) - convert_tComs))
                    };


                    return View(hv);
                }
            }

        }

        public ActionResult UrbanThree(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 4;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    // Updated 11 October 2022
                    using (var context = new spasystemdbEntities())
                    {

                        listDiscount = context.DiscountRecords
                                        .Where(b => b.BranchId == branchIds && b.AccountId == accountIdInInteger)
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

                    tSaleMinusDiscount = convert_tSales - sumDiscount; // Updated 11 October 2022
                    tSaleMinusDiscountInString = String.Format("{0:n0}", tSaleMinusDiscount); // Updated 11 October 2022

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
                        strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString()
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString()
                    };


                    return View(hv);
                }
            }
        }

        public ActionResult Member(string accountId, string monthNo, string yearNo, string cmd)
        {
            
            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<Member> listMem = new List<Member>();

                using (var context = new spasystemdbEntities())
                {

                    listMem = context.Members
                                    .OrderBy(b => b.Id)
                                    .ToList();
                }

                List<MemberItem> listMemForView = new List<MemberItem>();

                foreach(Member mem in listMem)
                {
                    
                    string[] splitStart = getMemberDetail(mem.Id).StartDate.ToString().Split(' ');
                    string[] splitExpire = getMemberDetail(mem.Id).ExpireDate.ToString().Split(' ');

                    MemberItem memItem = new MemberItem() { Id=mem.Id.ToString(), MemberNo = mem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(mem.Id).MemberGroupId).ShowName, Title = mem.Title, FirstName = mem.FirstName, FamilyName = mem.FamilyName, AddressInTH = mem.AddressInTH, City = mem.City, TelephoneNo = mem.TelephoneNo, WhatsAppId = mem.WhatsAppId, LineId = mem.LineId, CreateDate = mem.CreateDateTime.ToString(), VipStart = splitStart[0], VipExpire= splitExpire[0] };
                    if(mem.ActiveStatus.Equals("true"))
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


                return View(hv);
            }
        }

        public ActionResult ManageMemberType(string accountId, string monthNo, string yearNo, string cmd)
        {
            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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

                    if(getMemberGroupPriviledge(memG.Id) != null && getMemberGroupPriviledge(memG.Id).Any())
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
                    MemberGroupList = listMemGForView
                };


                return View(hvg);
            }
        }

        public ActionResult ManagePriviledge(string accountId, string monthNo, string yearNo, string cmd)
        {
            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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
                    MemberPriviledgeList = listMemPrivForView
                };


                return View(hvp);
            }
        }

        public ActionResult MemberDetail(string accountId, string cmd, string MemberId)
        {
            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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
                    memItem.Birth = splitBirthInEach[1]+"/"+ splitBirthInEach[0]+"/"+ splitBirthInEach[2];
                }
                

                listMemForView.Add(memItem);

                HeaderValueVIP hv = new HeaderValueVIP()
                {
                    MemberList = listMemForView
                };


                return View(hv);
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
            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                if(Mode.Equals("Edit"))
                {
                    Member myMem = getMember(Int32.Parse(MemberId));
                    
                    string[] splitStart = getMemberDetail(myMem.Id).StartDate.ToString().Split(' ');
                    string[] splitExpire = getMemberDetail(myMem.Id).ExpireDate.ToString().Split(' ');

                    
                    string[] splitVipStartPart = splitStart[0].Split('/');
                    string[] splitVipExpirePart = splitExpire[0].Split('/');

                    List<MemberItem> listMemForView = new List<MemberItem>();

                    MemberItem memItem = new MemberItem() { Id = myMem.Id.ToString(), MemberNo = myMem.MemberNo, VipType = getMemberGroupDetail(getMemberDetail(myMem.Id).MemberGroupId).ShowName, Title = myMem.Title, FirstName = myMem.FirstName, FamilyName = myMem.FamilyName, AddressInTH = myMem.AddressInTH, City = myMem.City, TelephoneNo = myMem.TelephoneNo, WhatsAppId = myMem.WhatsAppId, LineId = myMem.LineId, CreateDate = myMem.CreateDateTime.ToString(),VipStart = splitStart[0], VipExpire = splitExpire[0],VipStartDay=splitVipStartPart[1],VipStartMonth=splitVipStartPart[0],VipStartYear=splitVipStartPart[2],VipExpireDay=splitVipExpirePart[1],VipExpireMonth=splitVipExpirePart[0],VipExpireYear=splitVipExpirePart[2] };
                    if (myMem.ActiveStatus.Equals("true"))
                    {
                        memItem.Status = "Active";
                    }
                    else
                    {
                        memItem.Status = "Inactive";
                    }

                    if(!string.IsNullOrEmpty(myMem.Birth.ToString()))
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
                        MemberList = listMemForView
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
                        MemberList = listMemForView
                    };


                    return View(hv);
                }
            }
        }

        //public ActionResult AccountChosen(string accountId)
        //{
        //    // if credentials are correct.
        //    return View();
        //}
        public ActionResult UrbanFour(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 5;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString()
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString()
                    };


                    return View(hv);
                }
            }
        }

        public ActionResult UrbanFive(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 6;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString()
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString()
                    };


                    return View(hv);
                }
            }
        }

        public ActionResult UrbanSix(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 7;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString()
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString()
                    };


                    return View(hv);
                }
            }
        }

        public ActionResult UrbanSeven(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 8;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString()
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString()
                    };


                    return View(hv);
                }
            }
        }

        public ActionResult UrbanEight(string accountId, string monthNo, string yearNo, string cmd)
        {
            int branchIds = 10;

            if (cmd != null)
            {
                foreach (var element in System.Runtime.Caching.MemoryCache.Default)
                {
                    System.Runtime.Caching.MemoryCache.Default.Remove(element.Key);
                }
            }

            var noms = System.Runtime.Caching.MemoryCache.Default["names"];
            if (noms == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
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


                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, Int32.Parse(accountId)).ToString()
                    };


                    return View(hv);
                }
                else if (monthNo != null)
                {
                    int selectedMonth = Int32.Parse(monthNo);
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, selectedMonth, 1);
                    List<Account> listAccountInMonth = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInMonth = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Month == dts.Month && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInMonth.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInMonth[p].Id);
                        tSales += getTotalSaleInMonth(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInMonth(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInMonth(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //System.Diagnostics.Debug.WriteLine("f");

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInMonth(branchIds, listAccountInMonth),
                        strPieTopAName = getTopATopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        strPieTopBName = getTopBTopicName(getBestSellerInMonth(branchIds, listAccountInMonth)),
                        arrPieTopAVal = getTopA(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInMonth(branchIds, listAccountInMonth), branchIds),
                        finalSaleForEach = getFinalSaleForEachInMonth(branchIds, listAccountInMonth, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
                    };

                    return View(hv);
                }
                else if (yearNo != null)
                {
                    int selectedYear = Int32.Parse(yearNo);
                    DateTime dts = new DateTime(selectedYear, 1, 1);
                    List<Account> listAccountInYear = new List<Account>();

                    using (var context = new spasystemdbEntities())
                    {

                        listAccountInYear = context.Accounts
                                        .Where(b => b.BranchId == branchIds && b.Date.Year == dts.Year)
                                        .OrderBy(b => b.Id)
                                        .ToList();
                    }

                    Account ac = new Account();
                    int tSales = 0;
                    int tPaxNum = 0;
                    int tComs = 0;
                    int tStaff = 0;
                    int tOtherS = 0;
                    int tInitMoney = 0;
                    int tOil = 0;
                    int tBalanceNet = 0;

                    for (int p = 0; p < listAccountInYear.Count(); p++)
                    {
                        ac = getAccountValueFromAccountId(branchIds, listAccountInYear[p].Id);
                        tSales += getTotalSaleInYear(branchIds, ac.Id);
                        tPaxNum += getPaxNum(branchIds, ac.Id);
                        tComs += getTotalCommissionInYear(branchIds, ac.Id);
                        tStaff += (int)ac.StaffAmount;
                        tOtherS += getTotalOtherSaleInYear(branchIds, ac.Id);
                        tInitMoney += (int)ac.StartMoney;
                        tOil += tStaff * getOilPrice(branchIds);
                        //tBalanceNet += ((tSales + tOil + tOtherS) - tComs);
                    }

                    tBalanceNet = ((tSales + tOil + tOtherS) - tComs);

                    float tSalesInFloat = (float)tSales;
                    float tPaxNumInFloat = (float)tPaxNum;
                    float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = String.Format("{0:n0}", tSales),
                        strPax = String.Format("{0:n0}", tPaxNum),
                        strStaff = String.Format("{0:n0}", tStaff),
                        strCommission = String.Format("{0:n0}", tComs),
                        arrGraphVal = getOrderRecordForGraphInYear(branchIds, listAccountInYear),
                        strPieTopAName = getTopATopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        strPieTopBName = getTopBTopicName(getBestSellerInYear(branchIds, listAccountInYear)),
                        arrPieTopAVal = getTopA(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        arrPieTopBVal = getTopB(getBestSellerInYear(branchIds, listAccountInYear), branchIds),
                        finalSaleForEach = getFinalSaleForEachInYear(branchIds, listAccountInYear, getMassageSetId(branchIds)),
                        listAllAccounts = getAllAccountInSelectionList(branchIds),
                        listAllMonths = getAllMonthList(),
                        listAllYears = getAllYearList(),
                        strAverage = tAvg.ToString(),
                        strOtherSale = String.Format("{0:n0}", tOtherS),
                        strInitMoney = String.Format("{0:n0}", tInitMoney),
                        strOilIncome = String.Format("{0:n0}", tOil),
                        strBalanceNet = String.Format("{0:n0}", tBalanceNet)
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

                    //int tPaxNum = getPaxNum(branchIds, ac.Id);
                    //string tComs = getTotalCommission(branchIds, ac.Id);
                    //int tSalesInInteger = getTotalSaleInInteger(branchIds, ac.Id);
                    //float tSalesInFloat = (float)tSalesInInteger;
                    //float tPaxNumInFloat = (float)tPaxNum;
                    //float tAvg = (float)Math.Round(tSalesInFloat / tPaxNumInFloat, MidpointRounding.AwayFromZero);
                    //string tOtherS = getTotalOtherSale(branchIds, ac.Id);

                    SqlCommand command;
                    SqlDataReader dataReader;
                    String sql = " ";
                    sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name as 'Top A' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name as 'Top B' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchIds + "' order by AccountID desc) and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B' , (select top 1 dbo.Account.StaffAmount from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select top 1 dbo.Account.StartMoney from dbo.Account where BranchId = '" + branchIds + "' order by dbo.Account.Id desc) as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = (select top 1 dbo.Account.Id from dbo.Account where dbo.Account.BranchId = '" + branchIds + "' order by dbo.Account.Id desc) and dbo.OrderRecord.CancelStatus = 'false';";
                    //sql = "select sum(dbo.OrderRecord.Price) as 'Total Sale', count(dbo.OrderRecord.Id) as 'Total Pax', sum(dbo.OrderRecord.Commission) as 'Total Commission', (sum(dbo.OrderRecord.Price) / count(dbo.OrderRecord.Id)) as 'Average', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Total Staff', (select sum(dbo.OtherSaleRecord.Price) from dbo.OtherSaleRecord where dbo.OtherSaleRecord.BranchId = '" + branchIds + "' and dbo.OtherSaleRecord.AccountId = '" + accountId + "' and dbo.OtherSaleRecord.CancelStatus = 'false') as 'Total Other Sale', (select top 1 dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc) as 'Top A', (select dbo.MassageTopic.Name from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '" + branchIds + "' and AccountId = '" + accountId + "' and CancelStatus = 'false' group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc OFFSET 1 ROW FETCH NEXT 1 ROW ONLY) as 'Top B', (select dbo.Account.StaffAmount from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') * (select dbo.SystemSetting.Value from dbo.SystemSetting where BranchId = '" + branchIds + "' and Name = 'OilPrice') as 'Total Oil Income',(select dbo.Account.StartMoney from dbo.Account where Id = '" + accountId + "' and BranchId = '" + branchIds + "') as 'Initial Money' from dbo.OrderRecord where dbo.OrderRecord.BranchId = '" + branchIds + "' and dbo.OrderRecord.AccountId = '" + accountId + "' and dbo.OrderRecord.CancelStatus = 'false';";

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

                    HeaderValue hv = new HeaderValue()
                    {
                        strSales = tSales,
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
                        strVipCount = getTotalVipAmount(branchIds, ac.Id).ToString()
                    };


                    return View(hv);
                }
            }
        }

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

                ac = context.Accounts
                                .Where(b => b.Id == accountId && b.BranchId == branchId)
                                .FirstOrDefault();
            }

            return ac;
        }
        public Account getAccountValue(int branchId)
        {
            Account ac = new Account();
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

                ac = context.Accounts
                                .Where(b => b.BranchId == branchId)
                                .OrderByDescending(b => b.Id)
                                .FirstOrDefault();
            }

            return ac;
        }

        public SelectList getAllAccountInSelectionList(int branchId)
        {
            
            var listAllAccounts = new List<Account>();
            using (var context = new spasystemdbEntities())
            {

                listAllAccounts = context.Accounts
                                .Where(b => b.BranchId == branchId)
                                .OrderByDescending(b => b.CreateDateTime)
                                .ToList();
            }

            SelectList getAllAccountToSelectItem = new SelectList(listAllAccounts,"Id","CreateDateTime");

            return getAllAccountToSelectItem;
        }

        public SelectList getAllMonthList()
        {

            var listAllMonths = new List<SelectListItem>();
            SelectListItem jan = new SelectListItem() { Text = "January", Value = "01" };
            SelectListItem feb = new SelectListItem() { Text = "February", Value = "02" };
            SelectListItem mar = new SelectListItem() { Text = "March", Value = "03" };
            SelectListItem apr = new SelectListItem() { Text = "April", Value = "04" };
            SelectListItem may = new SelectListItem() { Text = "May", Value = "05" };
            SelectListItem jun = new SelectListItem() { Text = "June", Value = "06" };
            SelectListItem jul = new SelectListItem() { Text = "July", Value = "07" };
            SelectListItem aug = new SelectListItem() { Text = "August", Value = "08" };
            SelectListItem sep = new SelectListItem() { Text = "September", Value = "09" };
            SelectListItem oct = new SelectListItem() { Text = "October", Value = "10" };
            SelectListItem nov = new SelectListItem() { Text = "November", Value = "11" };
            SelectListItem dec = new SelectListItem() { Text = "December", Value = "12" };
            listAllMonths.Add(jan);
            listAllMonths.Add(feb);
            listAllMonths.Add(mar);
            listAllMonths.Add(apr);
            listAllMonths.Add(may);
            listAllMonths.Add(jun);
            listAllMonths.Add(jul);
            listAllMonths.Add(aug);
            listAllMonths.Add(sep);
            listAllMonths.Add(oct);
            listAllMonths.Add(nov);
            listAllMonths.Add(dec);


            SelectList getAllMonths = new SelectList(listAllMonths, "Value", "Text");

            return getAllMonths;
        }

        public SelectList getAllYearList()
        {

            var listAllYears = new List<SelectListItem>();
            SelectListItem year2016 = new SelectListItem() { Text = "2016", Value = "2016" };
            SelectListItem year2017 = new SelectListItem() { Text = "2017", Value = "2017" };
            SelectListItem year2018 = new SelectListItem() { Text = "2018", Value = "2018" };
            SelectListItem year2019 = new SelectListItem() { Text = "2019", Value = "2019" };
            SelectListItem year2020 = new SelectListItem() { Text = "2020", Value = "2020" };
            SelectListItem year2021 = new SelectListItem() { Text = "2021", Value = "2021" };
            SelectListItem year2022 = new SelectListItem() { Text = "2022", Value = "2022" };
            SelectListItem year2023 = new SelectListItem() { Text = "2023", Value = "2023" };
            SelectListItem year2024 = new SelectListItem() { Text = "2024", Value = "2024" };
            
            listAllYears.Add(year2016);
            listAllYears.Add(year2017);
            listAllYears.Add(year2018);
            listAllYears.Add(year2019);
            listAllYears.Add(year2020);
            listAllYears.Add(year2021);
            listAllYears.Add(year2022);
            listAllYears.Add(year2023);
            listAllYears.Add(year2024);


            SelectList getAllYears = new SelectList(listAllYears, "Value", "Text");

            return getAllYears;
        }

        public JsonProp[] getOrderRecordForGraph(int branchId, int accountId)
        {
            List<OrderRecord> listOrderRecord = new List<OrderRecord>();
            JsonProp[] arrPrePareToJS = new JsonProp[24];
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
                    .Where(r => r.BranchId == branchId && r.AccountId == accountId && r.CancelStatus == "false")
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

            if(listOrderRecord.Count()==0)
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
                arrPrePareToJS[17] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.ToString("yyyy-MM-dd HH:mm:ss"), y = t00 };
                arrPrePareToJS[18] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), y = t01 };
                arrPrePareToJS[19] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(2).ToString("yyyy-MM-dd HH:mm:ss"), y = t02 };
                arrPrePareToJS[20] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(3).ToString("yyyy-MM-dd HH:mm:ss"), y = t03 };
                arrPrePareToJS[21] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(4).ToString("yyyy-MM-dd HH:mm:ss"), y = t04 };
                arrPrePareToJS[22] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
                arrPrePareToJS[23] = new JsonProp() { x = listOrderRecord[dataNum - 1].Date.AddHours(6).ToString("yyyy-MM-dd HH:mm:ss"), y = 0 };
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

        //public List<FinalSaleForEachTopic> getFinalSaleForEachTest(int branchId)
        //{
        //    //List<int> listMassageTopic;
        //    List<FinalSaleForEachTopic> listOfFinalSale = new List<FinalSaleForEachTopic>();

        //    SqlCommand command;
        //    SqlDataReader dataReader;
        //    String sql, Output = " ";
        //    sql = "select dbo.MassageTopic.Name, count(dbo.OrderRecord.MassageTopicId) as 'Total Pax', sum(Price) as 'Total Sale' from dbo.OrderRecord left join dbo.MassageTopic on dbo.OrderRecord.MassageTopicId = dbo.MassageTopic.Id where BranchId = '"+branchId+"' and AccountId in (select top 1 Id as AccountID from dbo.Account where BranchId = '" + branchId + "' order by AccountID desc) group by dbo.MassageTopic.Name order by count(dbo.OrderRecord.MassageTopicId) desc;";

        //    connetionString = ConfigurationManager.AppSettings["cString"];
        //    cnn = new SqlConnection(connetionString);
        //    cnn.Open();
        //    command = new SqlCommand(sql, cnn);

        //    dataReader = command.ExecuteReader();
        //    while (dataReader.Read())
        //    {
        //        Output = Output + dataReader.GetValue(0) + "-" + dataReader.GetValue(1) + "-" + dataReader.GetValue(2) + "</br>";
        //        listOfFinalSale.Add(new FinalSaleForEachTopic { MassageTopicName = dataReader.GetValue(0).ToString(), TotalPax = String.Format("{0:n0}", dataReader.GetValue(1)), TotalSale = String.Format("{0:n}", dataReader.GetValue(2)) });
        //    }

        //    dataReader.Close();
        //    command.Dispose();
        //    cnn.Close();

        //    return listOfFinalSale;

        //}

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
                // Query for all blogs with names starting with B 
                //var blogs = from b in context.Accounts
                //            where b.Date.Equals(2016-12-22)
                //            select b;

                //ac.Add((Account)blogs);
                // Query for the Blog named ADO.NET Blog

                totalVip = context.OrderRecords
                                .Where(b => b.BranchId == branchId && b.AccountId == accountId && b.CancelStatus == "false" && b.MemberId != 0)
                                .ToList().Count();
            }

            return totalVip;
        }

    }
    public static class ExtensionHelpers
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
    }

}