using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication13.Models;
using WebApplication13.Resolver;
//using SelectPdf;

namespace WebApplication13.DAL
{
    public class PdfDAL
    {
        private static string ConvertHtmlToPdf(string html, string outputPath)
        {
            //var converter = new HtmlToPdf();
            //PdfDocument doc = converter.ConvertHtmlString(html);
            //doc.Save(outputPath);
            //doc.Close();

            return outputPath;
        }

        public static string GenerateExportInvoice(int id, string docNo, DateTime comDate , string webRootPath, string directoryPath, string outputPath)
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

                            string companyTypeOfUsage = null;
                            if (user.CompanyTypeOfUsage != null)
                            {
                                MobileDropdown CompanyTypeOfUsage = db.MobileDropdowns.FirstOrDefault(c => c.GroupName == "COM_TYPE_OF_USAGE" && c.Id == user.CompanyTypeOfUsage);
                                if (CompanyTypeOfUsage != null) companyTypeOfUsage = CompanyTypeOfUsage.Value;
                            }
                            if (companyTypeOfUsage == "company")
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

                                if (companyTypeOfUsage == "Company")
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

        public static string GenerateExportInvoice2(int id, string docNo, DateTime comDate, string webRootPath, string directoryPath, string outputPath)
        {
            try
            {
                try
                {
                    GlobalFontSettings.FontResolver = new CustomFontResolver(webRootPath);
                }
                catch { }

                using (var db = new spasystemdbEntities())
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                    // Create a new PDF document
                    PdfDocument document = new PdfDocument();
                    // Add a page to the document
                    PdfPage page = document.AddPage();
                    page.Size = PageSize.A4;
                    page.Orientation = PageOrientation.Portrait;
                    // Create a graphics object for the page
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    DateTime docDate = comDate.AddMonths(1);

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
                        string companyTypeOfUsage = null;
                        if (user.CompanyTypeOfUsage != null)
                        {
                            MobileDropdown CompanyTypeOfUsage = db.MobileDropdowns.FirstOrDefault(c => c.GroupName == "COM_TYPE_OF_USAGE" && c.Id == user.CompanyTypeOfUsage);
                            if (CompanyTypeOfUsage != null) companyTypeOfUsage = CompanyTypeOfUsage.Value;
                        }

                        bool isCompany = companyTypeOfUsage == "Company";

                        #region query detail
                        string cusName = db.MobileSettings.FirstOrDefault(c => c.Code == "COMPANY_NAME").Value;
                        string cusAddress = db.MobileSettings.FirstOrDefault(c => c.Code == "COMPANY_ADDRESS").Value;
                        string cusTaxId = db.MobileSettings.FirstOrDefault(c => c.Code == "COMPANY_TAX_ID").Value;
                        if (isCompany)
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
                        _email = user.Email;
                        if (!string.IsNullOrEmpty(docNo)) _doc_no = docNo;
                        if (docDate != null) _doc_date = docDate.ToString("d MMMM yyyy");
                        if (!string.IsNullOrEmpty(cusName)) _customer_name = cusName;
                        if (!string.IsNullOrEmpty(cusAddress)) _customer_address = cusAddress;
                        if (!string.IsNullOrEmpty(cusTaxId)) _customer_tax_id = cusTaxId;
                        if (docDate != null) _signature_date = docDate.ToString("dd/MM/yyyy").Replace("-", "/");

                        MobileFileAttachment idCardImg = db.MobileFileAttachments.Where(c => c.MobileUserId == id && c.Type == 1 && c.Active == "Y").OrderByDescending(o => o.Created).FirstOrDefault();
                        _img_path_id_card = $"{webRootPath}{idCardImg.FileSubPath}/{idCardImg.FileName}";
;
                        List<MobileComTransaction> comTrans = db.MobileComTransactions.Where(c =>
                            c.MobileUserId == user.Id
                            && c.Created.Year == comDate.Year
                            && c.Created.Month == comDate.Month
                            ).ToList();
                        #endregion

                        if (comTrans.Count > 0)
                        {
                            XFont fontHeader = new XFont("ArialUnicodeMS", 14, XFontStyle.Bold);
                            XFont fontBold = new XFont("ArialUnicodeMS", 11, XFontStyle.Bold);
                            XFont font = new XFont("ArialUnicodeMS", 11, XFontStyle.Regular);

                            double currentLine = 0;
                            double container = page.Width - calPageWidth(20);

                            #region header
                            //name_1
                            currentLine += 10;
                            if (!string.IsNullOrEmpty(_name_1)) gfx.DrawString(_name_1, fontHeader, XBrushes.Black, calPageWidth(10), calPageHeight(currentLine));
                            gfx.DrawString("Billing Note/Invoice", fontHeader, XBrushes.Black, calPageWidth(80), calPageHeight(currentLine));
                            //address
                            currentLine += 4;
                            double addressWidth = gfx.MeasureString("Address", fontBold).Width;
                            gfx.DrawString("Address", fontBold, XBrushes.Black, calPageWidth(10), calPageHeight(currentLine));
                            gfx.DrawLine(XPens.Black, calPageWidth(10), calPageHeight(currentLine + 0.3), calPageWidth(10) + addressWidth, calPageHeight(currentLine + 0.3));
                            if (!string.IsNullOrEmpty(_address)) gfx.DrawString(_address, font, XBrushes.Black, calPageWidth(11) + addressWidth, calPageHeight(currentLine));
                            //doc_no
                            double noWidth = gfx.MeasureString("No.", fontBold).Width;
                            gfx.DrawString("No.", fontBold, XBrushes.Black, calPageWidth(80), calPageHeight(currentLine));
                            gfx.DrawLine(XPens.Black, calPageWidth(80), calPageHeight(currentLine + 0.3), calPageWidth(80) + noWidth, calPageHeight(currentLine + 0.3));
                            if (!string.IsNullOrEmpty(_doc_no)) gfx.DrawString(_doc_no, font, XBrushes.Black, calPageWidth(81) + noWidth, calPageHeight(currentLine));
                            //tel
                            currentLine += 2.5;
                            double telWidth = gfx.MeasureString("Tel", fontBold).Width;
                            gfx.DrawString("Tel", fontBold, XBrushes.Black, calPageWidth(10), calPageHeight(currentLine));
                            gfx.DrawLine(XPens.Black, calPageWidth(10), calPageHeight(currentLine + 0.3), calPageWidth(10) + telWidth, calPageHeight(currentLine + 0.3));
                            if (!string.IsNullOrEmpty(_tel)) gfx.DrawString(_tel, font, XBrushes.Black, calPageWidth(11) + telWidth, calPageHeight(currentLine));
                            //doc_date
                            double dateWidth = gfx.MeasureString("Date", fontBold).Width;
                            gfx.DrawString("Date", fontBold, XBrushes.Black, calPageWidth(80), calPageHeight(currentLine));
                            gfx.DrawLine(XPens.Black, calPageWidth(80), calPageHeight(currentLine + 0.3), calPageWidth(80) + dateWidth, calPageHeight(currentLine + 0.3));
                            if (!string.IsNullOrEmpty(_doc_date)) gfx.DrawString(_doc_date, font, XBrushes.Black, calPageWidth(81) + dateWidth, calPageHeight(currentLine));
                            //email
                            currentLine += 2.5;
                            double emailWidth = gfx.MeasureString("Email", fontBold).Width;
                            gfx.DrawString("Email", fontBold, XBrushes.Black, calPageWidth(10), calPageHeight(currentLine));
                            gfx.DrawLine(XPens.Black, calPageWidth(10), calPageHeight(currentLine + 0.3), calPageWidth(10) + emailWidth, calPageHeight(currentLine + 0.3));
                            if (!string.IsNullOrEmpty(_email)) gfx.DrawString(_email, font, XBrushes.Black, calPageWidth(11) + emailWidth, calPageHeight(currentLine));
                            //tax_id
                            currentLine += 2.5;
                            double taxIdWidth = gfx.MeasureString("Tax ID", fontBold).Width;
                            gfx.DrawString("Tax ID", fontBold, XBrushes.Black, calPageWidth(10), calPageHeight(currentLine));
                            gfx.DrawLine(XPens.Black, calPageWidth(10), calPageHeight(currentLine + 0.3), calPageWidth(10) + taxIdWidth, calPageHeight(currentLine + 0.3));
                            if (!string.IsNullOrEmpty(_tax_id)) gfx.DrawString(_tax_id, font, XBrushes.Black, calPageWidth(11) + taxIdWidth, calPageHeight(currentLine));
                            #endregion

                            #region customer
                            double rectangleX = calPageWidth(10);
                            double rectangleY = calPageHeight(currentLine + 4);
                            double rectangleH = 1;
                            currentLine += 8;
                            double customerWidth = gfx.MeasureString("Customer", fontBold).Width;
                            gfx.DrawString("Customer", fontBold, XBrushes.Black, calPageWidth(12), calPageHeight(currentLine));
                            gfx.DrawLine(XPens.Black, calPageWidth(12), calPageHeight(currentLine + 0.3), calPageWidth(12) + customerWidth, calPageHeight(currentLine + 0.3));

                            //customer_name
                            if (!string.IsNullOrEmpty(_customer_name))
                            {
                                rectangleH += 1;
                                currentLine += 2.5;
                                gfx.DrawString(_customer_name, font, XBrushes.Black, calPageWidth(12), calPageHeight(currentLine));
                            }
                            //customer_address
                            if (!string.IsNullOrEmpty(_customer_address))
                            {
                                rectangleH += 1;
                                currentLine += 2.5;
                                gfx.DrawString(_customer_address, font, XBrushes.Black, calPageWidth(12), calPageHeight(currentLine));
                            }
                            //customer_tax_id
                            if (!string.IsNullOrEmpty(_customer_tax_id))
                            {
                                rectangleH += 1;
                                currentLine += 2.5;
                                gfx.DrawString("Tax ID", font, XBrushes.Black, calPageWidth(12), calPageHeight(currentLine));
                                gfx.DrawString(_customer_tax_id, font, XBrushes.Black, calPageWidth(13) + gfx.MeasureString("Tax ID", font).Width, calPageHeight(currentLine));
                            }
                            currentLine += 8;
                            double cusHeight = gfx.MeasureString("Tax ID", fontBold).Height * rectangleH + calPageHeight(8);
                            XRect rectangle = new XRect(rectangleX, rectangleY, page.Width - calPageWidth(20), cusHeight);
                            gfx.DrawRectangle(XPens.Black, rectangle);
                            #endregion

                            #region Draw table
                            int rows =  12;
                            if (!isCompany) rows = 11;
                            const int cols = 3;
                            const int cellWidth = 100;
                            const int cellHeight = 20;
                            double startX = calPageWidth(10);
                            double startY = calPageHeight(currentLine);
                            XPen tablePen = new XPen(XColors.Black, 0.5);

                            //draw table header
                            for (int col = 0; col < cols; col++)
                            {
                                if (col == 0)
                                {
                                    //20%
                                    gfx.DrawRectangle(tablePen, startX, startY, container * 0.2, cellHeight);
                                    gfx.DrawString($"No.", font, XBrushes.Black, new XRect(startX, startY, container * 0.2, cellHeight), XStringFormats.Center);

                                }
                                else if (col == 1)
                                {
                                    //60%
                                    gfx.DrawRectangle(tablePen, startX + container * 0.2, startY, container * 0.6, cellHeight);
                                    gfx.DrawString($"Detail", font, XBrushes.Black, new XRect(startX + container * 0.2, startY, container * 0.6, cellHeight), XStringFormats.Center);
                                }
                                else if (col == 2)
                                {
                                    //20%
                                    gfx.DrawRectangle(tablePen, startX + container * 0.8, startY, container * 0.2, cellHeight);
                                    gfx.DrawString($"Price", font, XBrushes.Black, new XRect(startX + container * 0.8, startY, container * 0.2, cellHeight), XStringFormats.Center);
                                }
                            }

                            //draw table
                            double sumCom = comTrans.Sum(s => s.TotalBaht);
                            _table_sub_total = sumCom.ToString("#,##0.00");
                            if (companyTypeOfUsage == "Company")
                            {
                                _table_vat = (sumCom * 1.07).ToString("#,##0.00");
                                _table_net_total = (sumCom * 1.07).ToString("#,##0.00");
                            }
                            else
                            {
                                _table_net_total = sumCom.ToString("#,##0.00");
                            }

                            for (int row = 1; row <= rows; row++)
                            {
                                if (row == 1)
                                {
                                    for (int col = 0; col < cols; col++)
                                    {
                                        if (col == 0)
                                        {
                                            //20%
                                            //gfx.DrawRectangle(tablePen, startX, startY + row * cellHeight, container * 0.2, cellHeight);
                                            gfx.DrawString($"1", font, XBrushes.Black, new XRect(startX, startY + row * cellHeight, container * 0.2, cellHeight), XStringFormats.Center);

                                        }
                                        else if (col == 1)
                                        {
                                            //60%
                                            //gfx.DrawRectangle(tablePen, startX + container * 0.2, startY + row * cellHeight, container * 0.6, cellHeight);
                                            gfx.DrawString($"Commission service ({comDate.ToString("MMMM yyyy")})", font, XBrushes.Black, new XRect(startX + container * 0.2 + calPageWidth(2), startY + row * cellHeight, container * 0.6, cellHeight), XStringFormats.CenterLeft);
                                        }
                                        else if (col == 2)
                                        {
                                            //20%
                                            //gfx.DrawRectangle(tablePen, startX + container * 0.8, startY + row * cellHeight, container * 0.2, cellHeight);
                                            gfx.DrawString($"{_table_sub_total}", font, XBrushes.Black, new XRect(startX + container * 0.8 - calPageWidth(2), startY + row * cellHeight, container * 0.2, cellHeight), XStringFormats.CenterRight);
                                        }
                                    }
                                }
                                if ((isCompany && row == rows - 4) || (!isCompany && row == rows - 3))
                                {
                                    gfx.DrawLine(tablePen, startX, startY + row * cellHeight, startX + container, startY + row * cellHeight);
                                    //remark
                                    gfx.DrawString($"Remark", font, XBrushes.Black, new XRect(startX + calPageWidth(2), startY + row * cellHeight, container * 0.6, cellHeight), XStringFormats.CenterLeft);
                                    gfx.DrawString($"Sub Total", font, XBrushes.Black, new XRect(startX + container * 0.6 + calPageWidth(2), startY + row * cellHeight, container * 0.6, cellHeight), XStringFormats.CenterLeft);
                                    gfx.DrawString($"{_table_sub_total}", font, XBrushes.Black, new XRect(startX + container * 0.8 - calPageWidth(2), startY + row * cellHeight, container * 0.2, cellHeight), XStringFormats.CenterRight);
                                }
                                if (isCompany && row == rows - 3)
                                {
                                    //vat
                                    gfx.DrawString($"VAT 7%", font, XBrushes.Black, new XRect(startX + container * 0.6 + calPageWidth(2), startY + row * cellHeight, container * 0.6, cellHeight), XStringFormats.CenterLeft);
                                    gfx.DrawString($"{_table_vat}", font, XBrushes.Black, new XRect(startX + container * 0.8 - calPageWidth(2), startY + row * cellHeight, container * 0.2, cellHeight), XStringFormats.CenterRight);
                                }
                                if (row == rows - 2)
                                {
                                    //net total
                                    gfx.DrawString($"Net Total", font, XBrushes.Black, new XRect(startX + container * 0.6 + calPageWidth(2), startY + row * cellHeight, container * 0.6, cellHeight), XStringFormats.CenterLeft);
                                    gfx.DrawString($"{_table_net_total}", font, XBrushes.Black, new XRect(startX + container * 0.8 - calPageWidth(2), startY + row * cellHeight, container * 0.2, cellHeight), XStringFormats.CenterRight);
                                }
                                if (rows == row)
                                {
                                    gfx.DrawLine(tablePen, startX, startY + (row - 1) * cellHeight, startX + container, startY + (row - 1) * cellHeight);
                                }

                                if (row <= rows - 5)
                                {
                                    //item horizontal line
                                    gfx.DrawLine(tablePen, startX, startY + (row * cellHeight), startX, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                    gfx.DrawLine(tablePen, startX + container * 0.2, startY + (row * cellHeight), startX + container * 0.2, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                    gfx.DrawLine(tablePen, startX + container * 0.8, startY + (row * cellHeight), startX + container * 0.8, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                    gfx.DrawLine(tablePen, startX + container * 1, startY + (row * cellHeight), startX + container * 1, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                }
                                else if ((isCompany && row <= rows - 2) || (!isCompany && row <= rows - 3))
                                {
                                    //remark horizontal line
                                    gfx.DrawLine(tablePen, startX, startY + ((row + (isCompany ? 0 : 1)) * cellHeight), startX, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                    gfx.DrawLine(tablePen, startX + container * 0.6, startY + ((row + (isCompany ? 0 : 1)) * cellHeight), startX + container * 0.6, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                    gfx.DrawLine(tablePen, startX + container * 0.8, startY + ((row + (isCompany ? 0 : 1)) * cellHeight), startX + container * 0.8, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                    gfx.DrawLine(tablePen, startX + container * 1, startY + ((row + (isCompany ? 0 : 1)) * cellHeight), startX + container * 1, startY + ((row + (isCompany ? 1 : 2)) * cellHeight));
                                }
                            }
                            #endregion

                            #region footer
                            currentLine += 30;
                            if (!string.IsNullOrEmpty(_img_path_id_card))
                            {
                                XImage image = null;
                                try
                                {
                                    image = XImage.FromFile(_img_path_id_card);
                                    gfx.DrawImage(image, calPageWidth(10), calPageHeight(currentLine), calPageWidth(55), calPageHeight(22));
                                }
                                catch { }
                            }
                            currentLine += 8;
                            if (!string.IsNullOrEmpty(_name_2)) gfx.DrawString($"{_name_2}", fontBold, XBrushes.Black, calPageWidth(85), calPageHeight(currentLine), XStringFormats.Center);
                            currentLine += 2.5;
                            if (!string.IsNullOrEmpty(_signature_date)) gfx.DrawString($"{_signature_date}", font, XBrushes.Black, calPageWidth(85), calPageHeight(currentLine), XStringFormats.Center);
                            #endregion

                            // Save the PDF document to a file
                            document.Save(outputPath);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                DataDAL.ErrorLog($"GenerateExportInvoice2", ex.ToString(), "");
            }

            return outputPath;
        }

        private static double calPageWidth(double percentage)
        {
            double result = (595 / 100) * percentage;
            return result;
        }

        private static double calPageHeight(double percentage)
        {
            double result = (854 / 100) * percentage;
            return result;
        }
    }
}