using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication13.Models;
using SelectPdf;

namespace WebApplication13.DAL
{
    public class PdfDAL
    {
        private static string ConvertHtmlToPdf(string html, string outputPath)
        {
            var converter = new HtmlToPdf();
            PdfDocument doc = converter.ConvertHtmlString(html);
            doc.Save(outputPath);
            doc.Close();

            return outputPath;
        }

        public static string GenerateExportInvoice(int id, string docNo, DateTime comDate , string webRootPath, string outputPath)
        {
            string path = null;
            try
            {
                using (var db = new spasystemdbEntities())
                {
                    MobileSetting template = db.MobileSettings.FirstOrDefault(c => c.Code == "INVOICE_TEMPLATE");
                    if (template != null && !string.IsNullOrEmpty(template.Value))
                    {
                        DateTime docDate = comDate.AddMonths(1);

                        string replaceTemplate = template.Value ?? "";
                        string _name_1 = "";
                        string _name_2 = "";
                        string _address = "";
                        string _tel = "";
                        string _email = "";
                        string _tax_id = "";
                        string _doc_no = "";
                        string _doc_date = "";
                        string _customer_name = "";
                        string _customer_address = "";
                        string _customer_tax_id = "";
                        string _company_name = "";
                        string _signature_date = "";
                        string _img_path_id_card = "";
                        string _table_items = "";
                        string _table_sub_total = "";
                        string _table_vat_display = "none";
                        string _table_vat = "";
                        string _table_net_total = "";

                        MobileUser user = db.MobileUsers.FirstOrDefault(c => c.Id == id);
                        if (user != null)
                        {
                            string cusName = db.MobileSettings.FirstOrDefault(c => c.Code == "COMPANY_NAME").Value;
                            string cusAddress = db.MobileSettings.FirstOrDefault(c => c.Code == "COMPANY_ADDRESS").Value;
                            string cusTaxId = db.MobileSettings.FirstOrDefault(c => c.Code == "COMPANY_TAX_ID").Value;
                            if (user.CompanyTypeOfUsage.ToLower() == "company")
                            {
                                _name_1 = user.CompanyName;
                                _company_name = user.CompanyName;
                                _address = user.CompanyAddress;
                                _tax_id = user.CompanyTaxId;
                            }
                            else
                            {
                                _name_1 = $"{user.FirstName} {user.LastName}";
                                _address = user.Address;
                                _tax_id = user.IdCardNumber;
                            }
                            _name_2 = $"{user.FirstName} {user.LastName}";
                            _tel = user.PhoneNumber;
                            if (!string.IsNullOrEmpty(docNo)) _doc_no = docNo;
                            if (docDate != null) _doc_date = docDate.ToString("d MMMM yyyy");
                            if (!string.IsNullOrEmpty(cusName)) _customer_name = cusName;
                            if (!string.IsNullOrEmpty(cusAddress)) _customer_address = cusAddress;
                            if (!string.IsNullOrEmpty(cusTaxId)) _customer_tax_id = cusTaxId;
                            if (docDate != null) _signature_date = docDate.ToString("dd/MM/yyyy").Replace("-", "/");

                            MobileFileAttachment idCardImg = db.MobileFileAttachments.Where(c => c.MobileUserId == id && c.Type == 1 && c.Active == "Y").OrderByDescending(o => o.Created).FirstOrDefault();
                            _img_path_id_card = $"{webRootPath}{idCardImg.FileSubPath}/{idCardImg.FileName}";

                            string templateItem = db.MobileSettings.FirstOrDefault(c => c.Code == "INVOICE_TEMPLATE_TABLE_ITEM")?.Value;
                            List<MobileComTransaction> comTrans = db.MobileComTransactions.Where(c =>
                                c.MobileUserId == user.Id
                                && c.Created.Year == comDate.Year
                                && c.Created.Month == comDate.Month
                                ).ToList();
                            if (comTrans.Count > 0)
                            {
                                double sumCom = comTrans.Sum(s => s.TotalBaht);
                                _table_sub_total = sumCom.ToString("#,##0.00");

                                _table_items = templateItem
                                    .Replace("[table_item_no]", "1")
                                    .Replace("[table_item_detail]", $"Commission service ({comDate.ToString("MMMM yyyy")})")
                                    .Replace("[table_item_price]", $"{_table_sub_total}")
                                    ;
                                for(int i = 0; i < 9; i++)
                                {
                                    _table_items += templateItem
                                    .Replace("[table_item_no]", "")
                                    .Replace("[table_item_detail]", "")
                                    .Replace("[table_item_price]", "")
                                    ;
                                }

                                if (user.CompanyTypeOfUsage.ToLower() == "company")
                                {
                                    _table_vat_display = "block";
                                    _table_vat = (sumCom * 1.07).ToString("#,##0.00");
                                    _table_net_total = (sumCom * 1.07).ToString("#,##0.00");
                                }
                                else
                                {
                                    _table_net_total = sumCom.ToString("#,##0.00");
                                }
                            }
                        }

                        replaceTemplate = replaceTemplate
                            .Replace("[name_1]", _name_1)
                            .Replace("[address]", _address)
                            .Replace("[tel]", _tel)
                            .Replace("[email]", _email)
                            .Replace("[tax_id]", _tax_id)
                            .Replace("[doc_no]", _doc_no)
                            .Replace("[doc_date]", _doc_date)
                            .Replace("[customer_name]", _customer_name)
                            .Replace("[customer_address]", _customer_address)
                            .Replace("[customer_tax_id]", _customer_tax_id)
                            .Replace("[name_2]", _name_2)
                            .Replace("[company_name]", _company_name)
                            .Replace("[signature_date]", _signature_date)
                            .Replace("[img_path_id_card]", _img_path_id_card)
                            .Replace("[table_items]", _table_items)
                            .Replace("[table_sub_total]", _table_sub_total)
                            .Replace("[table_vat_display]", _table_vat_display)
                            .Replace("[table_vat]", _table_vat)
                            .Replace("[table_net_total]", _table_net_total)
                            ;
                        path = ConvertHtmlToPdf(replaceTemplate, outputPath);
                    }
                }
            }
            catch { }

            return path;
        }
    }
}