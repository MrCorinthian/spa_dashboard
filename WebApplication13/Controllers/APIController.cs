using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
    public class APIController : Controller
    {
        private spasystemdbEntities db = new spasystemdbEntities();

        // GET: API
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetBranchData(string branchId)
        {
            int getBranchId = Int32.Parse(branchId);
            var send = new
            {
                Status = "true",
                Branch_Name = getBranchName(getBranchId),
                Version_Number = getVersionNo(getBranchId),
                MassageSet_Id = getMassageSetId(getBranchId),
                MassageTopic = db.MassageTopics.ToList(),
                MassagePlan = db.MassagePlans.ToList(),
                MassageSet = getAllMassageSet(getMassageSetId(getBranchId)),
                OtherSale = db.OtherSales.ToList(),
                DiscountMaster = db.DiscountMasters.ToList(),
                DiscountMasterDetail = getAllDiscountMasterDetail(getBranchId),
                SystemConfig = getAllSetting(getBranchId),
                Password = getBranchPasswordDetail(getBranchId).Value,
                Password_Version = getBranchPasswordDetail(getBranchId).Version,
                Password_Status = getBranchPasswordDetail(getBranchId).Status,
                System_Version = getBranchSystemVersion(getBranchId).Value,
                System_URL = getBranchDownloadURL(getBranchId).Value

            };
            var json = Json(send, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;


        }

        [HttpPost]
        public ActionResult SendData(string data)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(data);

                //Insert Account
                Account getAccount = rootObj.AccountData;
                if (getAccount == null || getAccount.Equals(null))
                {

                }
                else
                {
                    db.Accounts.Add(getAccount);
                }

                //Insert OrderRecord
                List<OrderRecord> getOrderRecordList = rootObj.OrderRecordList;
                if (getOrderRecordList == null || getOrderRecordList.Equals(null))
                {

                }
                else
                {
                    foreach (OrderRecord a in getOrderRecordList)
                    {
                        db.OrderRecords.Add(a);
                    }
                }

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            var response = new
            {
                Status = status,
                Error_Message = error
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendOtherSaleData(string data)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleOtherSaleRecord rootObj = JsonConvert.DeserializeObject<RootSingleOtherSaleRecord>(data);

                //Insert OtherSaleRecord
                OtherSaleRecord getOtherSaleRecord = rootObj.OtherSaleRecordData;
                db.OtherSaleRecords.Add(getOtherSaleRecord);

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            var response = new
            {
                Status = status,
                Error_Message = error
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendDiscountData(string data)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleDiscountRecord rootObj = JsonConvert.DeserializeObject<RootSingleDiscountRecord>(data);

                //Insert OtherSaleRecord
                DiscountRecord getDiscountRecord = rootObj.DiscountRecordData;
                db.DiscountRecords.Add(getDiscountRecord);

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            var response = new
            {
                Status = status,
                Error_Message = error
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendOrderRecordWithDiscountData(string data)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleOrderRecordWithDiscount rootObj = JsonConvert.DeserializeObject<RootSingleOrderRecordWithDiscount>(data);

                //Insert OtherSaleRecord
                OrderRecordWithDiscount getOrderRecordWithDiscount = rootObj.OrderRecordWithDiscountData;
                db.OrderRecordWithDiscounts.Add(getOrderRecordWithDiscount);

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            var response = new
            {
                Status = status,
                Error_Message = error
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendMassageSetData(string data)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleMassageSetRecord rootObj = JsonConvert.DeserializeObject<RootSingleMassageSetRecord>(data);

                //Insert MassageSet
                MassageSet getMassageSet = rootObj.MassageSetRecordData;
                db.MassageSets.Add(getMassageSet);

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            var response = new
            {
                Status = status,
                Error_Message = error
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateAccount(string accountData)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(accountData);
                Account getAccount = rootObj.AccountData;
                int accountId = getAccount.Id;
                int branchId = getAccount.BranchId;

                Account accountUpdated = db.Accounts.Where(a => a.Id == accountId && a.BranchId == branchId).FirstOrDefault();
                accountUpdated.StaffAmount = getAccount.StaffAmount;
                accountUpdated.UpdateDateTime = getAccount.UpdateDateTime;

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            var response = new
            {
                Status = status,
                Error_Message = error
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateOrderRecord(string orderRecordData)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleOrderRecord rootObj = JsonConvert.DeserializeObject<RootSingleOrderRecord>(orderRecordData);
                OrderRecord getOrderRecord = rootObj.OrderRecordData;
                int orderId = getOrderRecord.Id;
                int accountId = getOrderRecord.AccountId;
                int branchId = getOrderRecord.BranchId;

                OrderRecord orderRecordUpdated = db.OrderRecords.Where(a => a.Id == orderId && a.BranchId == branchId && a.AccountId == accountId).FirstOrDefault();
                orderRecordUpdated.CancelStatus = getOrderRecord.CancelStatus;
                orderRecordUpdated.UpdateDateTime = getOrderRecord.UpdateDateTime;

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            var response = new
            {
                Status = status,
                Error_Message = error
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public int getVersionNo(int branchId)
        {
            //List<OrderRecord> orderList = new List<OrderRecord>();
            Models.Version versionData;

            using (var context = new spasystemdbEntities())
            {
                versionData = context.Versions
                                .Where(b => b.BranchId == branchId)
                                .FirstOrDefault();
            }

            int versionNo = (int)versionData.VersionNo;

            return versionNo;
        }

        public string getBranchName(int branchId)
        {
            //List<OrderRecord> orderList = new List<OrderRecord>();
            Branch branchData;

            using (var context = new spasystemdbEntities())
            {
                branchData = context.Branches
                                .Where(b => b.Id == branchId)
                                .FirstOrDefault();
            }

            return branchData.Name;
        }

        public SystemSetting getBranchPasswordDetail(int branchId)
        {
            //List<OrderRecord> orderList = new List<OrderRecord>();
            SystemSetting settingData;

            using (var context = new spasystemdbEntities())
            {
                settingData = context.SystemSettings
                                .Where(b => b.BranchId == branchId && b.Name == "Password")
                                .FirstOrDefault();
            }

            return settingData;
        }

        public SystemSetting getBranchSystemVersion(int branchId)
        {
            //List<OrderRecord> orderList = new List<OrderRecord>();
            SystemSetting settingData;

            using (var context = new spasystemdbEntities())
            {
                settingData = context.SystemSettings
                                .Where(b => b.BranchId == branchId && b.Name == "SystemVersion")
                                .FirstOrDefault();
            }

            return settingData;
        }

        public SystemSetting getBranchDownloadURL(int branchId)
        {
            //List<OrderRecord> orderList = new List<OrderRecord>();
            SystemSetting settingData;

            using (var context = new spasystemdbEntities())
            {
                settingData = context.SystemSettings
                                .Where(b => b.BranchId == branchId && b.Name == "DownloadURL")
                                .FirstOrDefault();
            }

            return settingData;
        }

        public int getMassageSetId(int branchId)
        {
            //List<OrderRecord> orderList = new List<OrderRecord>();
            Branch branchData;

            using (var context = new spasystemdbEntities())
            {
                branchData = context.Branches
                                .Where(b => b.Id == branchId)
                                .FirstOrDefault();
            }

            return branchData.MassageSetId;
        }

        public List<MassageSet> getAllMassageSet(int massageSetId)
        {
            List<MassageSet> listMassageSet;

            using (var context = new spasystemdbEntities())
            {

                listMassageSet = context.MassageSets
                                .Where(b => b.Id == massageSetId)
                                .ToList();
            }

            return listMassageSet;
        }

        public List<DiscountMasterDetail> getAllDiscountMasterDetail(int branchId)
        {
            List<DiscountMasterDetail> listDiscountMasterDetail;

            using (var context = new spasystemdbEntities())
            {

                listDiscountMasterDetail = context.DiscountMasterDetails
                                .Where(b => b.BranchId == branchId)
                                .ToList();
            }

            return listDiscountMasterDetail;
        }

        public List<SystemSetting> getAllSetting(int branchId)
        {
            List<SystemSetting> listSetting;

            using (var context = new spasystemdbEntities())
            {

                listSetting = context.SystemSettings
                                .Where(b => b.BranchId == branchId)
                                .ToList();
            }

            return listSetting;
        }

        public class RootObject
        {
            public Account AccountData { get; set; }
            public List<OrderRecord> OrderRecordList { get; set; }

        }

        public class RootSingleOrderRecord
        {
            public OrderRecord OrderRecordData { get; set; }

        }

        public class RootSingleOtherSaleRecord
        {
            public OtherSaleRecord OtherSaleRecordData { get; set; }

        }

        public class RootSingleDiscountRecord
        {
            public DiscountRecord DiscountRecordData { get; set; }

        }
        public class RootSingleOrderRecordWithDiscount
        {
            public OrderRecordWithDiscount OrderRecordWithDiscountData { get; set; }

        }

        public class RootSingleMassageSetRecord
        {
            public MassageSet MassageSetRecordData { get; set; }

        }
    }
}