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
                Member = db.Members.ToList(),
                MemberGroup = db.MemberGroups.ToList(),
                MemberPriviledge = db.MemberPriviledges.ToList(),
                PriviledgeType = db.PriviledgeTypes.ToList(),
                MemberGroupPriviledge = db.MemberGroupPriviledges.ToList(),
                MemberDetail = db.MemberDetails.ToList(),
                SellItemType = getAllSellItemType(getBranchId),
                //EmployeeType = getAllEmployeeType(getBranchId),
                SystemConfig = getAllSetting(getBranchId),
                Password = getBranchPasswordDetail(getBranchId).Value,
                Password_Version = getBranchPasswordDetail(getBranchId).Version,
                Password_Status = getBranchPasswordDetail(getBranchId).Status,
                System_Version = getBranchSystemVersion(getBranchId).Value,
                System_URL = getBranchDownloadURL(getBranchId).Value,
                Master_DB = "https://drive.google.com/u/0/uc?id=1TP8xez3QP2WqJ_PYjLZT_03sUatEk1Ws&export=download"

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

        [HttpPost]
        public ActionResult VerifyMember(string memberNo)
        {
            //int getMemNo = Int32.Parse(memberNo);
            Member getCurMem = getMemberFromMemberNo(memberNo);
            if(getCurMem != null)
            {
                var send = new
                {
                    Status = "true",
                    Id = getCurMem.Id,
                    MemberNo = getCurMem.MemberNo,
                    Title = getCurMem.Title,
                    FirstName = getCurMem.FirstName,
                    FamilyName = getCurMem.FamilyName,
                    //BirthDay = getAllMassageSet(getMassageSetId(getBranchId)),
                    //BirthMonth = db.OtherSales.ToList(),
                    //BirthYear = db.DiscountMasters.ToList(),
                    Address = getCurMem.AddressInTH,
                    City = getCurMem.City,
                    TelephoneNo = getCurMem.TelephoneNo,
                    WhatsappId = getCurMem.WhatsAppId,
                    LineId = getCurMem.LineId,
                    MemberDetails = getMemberDetail(getCurMem.Id)
                    //MemberGroupId = db.MemberGroupPriviledges.ToList(),
                    //PriviledgeTypeId = db.MemberDetails.ToList(),
                    //Value = getAllSetting(getBranchId)

                };
                var json = Json(send, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }
            else
            {
                var send = new
                {
                    Status = "false",
                };
                var json = Json(send, JsonRequestBehavior.AllowGet);
                json.MaxJsonLength = int.MaxValue;
                return json;
            }

        }

        [HttpPost]
        public ActionResult SendReceiptData(string data)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleReceipt rootObj = JsonConvert.DeserializeObject<RootSingleReceipt>(data);

                //Insert Receipt
                Receipt getReceiptRecord = rootObj.ReceiptData;
                db.Receipts.Add(getReceiptRecord);

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            //int latestReceiptId = getLatestReceipt().Id;

            var response = new
            {
                Status = status,
                Error_Message = error
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SendOrderReceiptData(string data)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleOrderReceipt rootObj = JsonConvert.DeserializeObject<RootSingleOrderReceipt>(data);

                //Insert Receipt
                OrderReceipt getOrderReceiptRecord = rootObj.OrderReceiptData;
                db.OrderReceipts.Add(getOrderReceiptRecord);

                db.SaveChanges();
                status = "true";
            }
            catch (Exception ie)
            {
                status = "false";
                error = ie.ToString();
            }

            //int latestReceiptId = getLatestReceipt().Id;

            var response = new
            {
                Status = status,
                Error_Message = error
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateOrderReceipt(string orderReceiptData)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleOrderReceipt rootObj = JsonConvert.DeserializeObject<RootSingleOrderReceipt>(orderReceiptData);
                OrderReceipt getOrderReceipt = rootObj.OrderReceiptData;
                int receiptId = getOrderReceipt.Id;
                int accountId = getOrderReceipt.AccountId;
                int branchId = getOrderReceipt.BranchId;

                OrderReceipt orderReceiptUpdated = db.OrderReceipts.Where(a => a.Id == receiptId && a.BranchId == branchId && a.AccountId == accountId).FirstOrDefault();
                orderReceiptUpdated.CancelStatus = getOrderReceipt.CancelStatus;
                orderReceiptUpdated.UpdateDateTime = getOrderReceipt.UpdateDateTime;

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
        public ActionResult UpdateDiscountRecord(string discountRecordData)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleDiscountRecord rootObj = JsonConvert.DeserializeObject<RootSingleDiscountRecord>(discountRecordData);
                DiscountRecord getDiscountRecord = rootObj.DiscountRecordData;
                int discountId = getDiscountRecord.Id;
                int accountId = getDiscountRecord.AccountId;
                int branchId = getDiscountRecord.BranchId;

                DiscountRecord discountRecordUpdated = db.DiscountRecords.Where(a => a.Id == discountId && a.BranchId == branchId && a.AccountId == accountId).FirstOrDefault();
                discountRecordUpdated.CancelStatus = getDiscountRecord.CancelStatus;
                discountRecordUpdated.UpdateDateTime = getDiscountRecord.UpdateDateTime;

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
        public ActionResult UpdateOtherSaleRecord(string otherSaleRecordData)
        {
            string status = "false";
            string error = "-";
            try
            {
                RootSingleOtherSaleRecord rootObj = JsonConvert.DeserializeObject<RootSingleOtherSaleRecord>(otherSaleRecordData);
                OtherSaleRecord getOtherSaleRecord = rootObj.OtherSaleRecordData;
                int otherSaleRecordId = getOtherSaleRecord.Id;
                int accountId = getOtherSaleRecord.AccountId;
                int branchId = getOtherSaleRecord.BranchId;

                OtherSaleRecord otherSaleRecordUpdated = db.OtherSaleRecords.Where(a => a.Id == otherSaleRecordId && a.BranchId == branchId && a.AccountId == accountId).FirstOrDefault();
                otherSaleRecordUpdated.CancelStatus = getOtherSaleRecord.CancelStatus;
                otherSaleRecordUpdated.UpdateDateTime = getOtherSaleRecord.UpdateDateTime;

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

        public Member getMemberFromMemberNo(string memberNo)
        {
            Member mem = new Member();

            using (var contexts = new spasystemdbEntities())
            {

                mem = contexts.Members
                                .Where(b => b.MemberNo == memberNo && b.ActiveStatus == "true")
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

        public Receipt getLatestReceipt()
        {
            Receipt latestReceipt = new Receipt();

            using (var contexts = new spasystemdbEntities())
            {

                latestReceipt = contexts.Receipts
                                .OrderByDescending(b => b.Created)
                                .FirstOrDefault();
            }


            return latestReceipt;

        }
        public List<SellItemType> getAllSellItemType(int branchId)
        {
            List<SellItemType> listSellItemType;

            using (var context = new spasystemdbEntities())
            {

                listSellItemType = context.SellItemTypes
                                .Where(b => b.BranchId == branchId)
                                .ToList();
            }

            return listSellItemType;
        }
        public List<EmployeeType> getAllEmployeeType(int branchId)
        {
            List<EmployeeType> listEmployeeType;

            using (var context = new spasystemdbEntities())
            {

                listEmployeeType = context.EmployeeTypes
                                .Where(b => b.BranchId == branchId)
                                .ToList();
            }

            return listEmployeeType;
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

        public class RootSingleReceipt
        {
            public Receipt ReceiptData { get; set; }

        }

        public class RootSingleOrderReceipt
        {
            public OrderReceipt OrderReceiptData { get; set; }

        }
    }
}