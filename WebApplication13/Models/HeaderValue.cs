using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication13.Models
{
    public class HeaderValue
    {
        public string SellStartDate { get; set; }
        public string strSales { get; set; }
        public string strPax { get; set; }
        public string strStaff { get; set; }
        public string strCommission { get; set; }
        public JsonProp[] arrGraphVal { get; set; }
        public string strPieTopAName { get; set; }
        public string strPieTopBName { get; set; }
        public JsonPropForTopAB[] arrPieTopAVal { get; set; }
        public JsonPropForTopAB[] arrPieTopBVal { get; set; }
        public List<ListOfEachTopicVal> listTopicAndPlanVals { get; set; }
        public List<OrderRecordWithName> allRecordsWithName { get; set; }
        public List<FinalSaleForEachTopic> finalSaleForEach { get; set; }
        public SelectList listAllAccounts { get; set; }
        public SelectList listAllMonths { get; set; }
        public SelectList listAllYears { get; set; }
        public string strAverage { get; set; }
        public string strOtherSale { get; set; }
        public string strInitMoney { get; set; }
        public string strOilIncome { get; set; }
        public string strBalanceNet { get; set; }
        public string strVipCount { get; set; }
        public string strLoginName { get; set; }
        public string strCash { get; set; }
        public string strCredit { get; set; }
        public string strVoucher { get; set; }
        public int bid { get; set; }
        public string bName { get; set; }
    }
}