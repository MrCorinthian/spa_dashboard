using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication13.Models
{
    public class HeaderWithBeauty
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
        public string strAverage { get; set; }
        public string strOtherSale { get; set; }
        public string strOilIncome { get; set; }
        public string strBalanceNet { get; set; }
        public string strVipCount { get; set; }
        public string strCash { get; set; }
        public string strCredit { get; set; }
        public string strVoucher { get; set; }

        public string SellStartDate_B { get; set; }
        public string strSales_B { get; set; }
        public string strPax_B { get; set; }
        public string strStaff_B { get; set; }
        public string strCommission_B { get; set; }
        public JsonProp[] arrGraphVal_B { get; set; }
        public string strPieTopAName_B { get; set; }
        public string strPieTopBName_B { get; set; }
        public JsonPropForTopAB[] arrPieTopAVal_B { get; set; }
        public JsonPropForTopAB[] arrPieTopBVal_B { get; set; }
        public List<ListOfEachTopicVal> listTopicAndPlanVals_B { get; set; }
        public List<OrderRecordWithName> allRecordsWithName_B { get; set; }
        public List<FinalSaleForEachTopic> finalSaleForEach_B { get; set; }
        public string strAverage_B { get; set; }
        public string strOtherSale_B { get; set; }
        public string strOilIncome_B { get; set; }
        public string strBalanceNet_B { get; set; }
        public string strVipCount_B { get; set; }
        public string strCash_B { get; set; }
        public string strCredit_B { get; set; }
        public string strVoucher_B { get; set; }

        public string strInitMoney { get; set; }
        public string strLoginName { get; set; }
        public SelectList listAllAccounts { get; set; }
        public SelectList listAllMonths { get; set; }
        public SelectList listAllYears { get; set; }
        public int bid { get; set; }
        public string bName { get; set; }
        public string accountId { get; set; }
    }
}