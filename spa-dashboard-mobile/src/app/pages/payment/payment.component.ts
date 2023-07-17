import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { BaseUrl } from '../../const-value/base-url';
import { Colors } from '../../const-value/colors';
import { DataIndex } from '../../models/data-index';
import { Format } from '../../share-functions/format-functions';
import {
  GenerateMonthList,
  GenerateYearList,
  GeneratePaymentStatusList,
  GenerateCompanyTypeOfUsageList,
} from '../../share-functions/generate-functions';

@Component({
  selector: 'payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss', '../../styles.scss'],
})
export class PaymentComponent {
  colors: any = Colors;
  now: Date = new Date();
  filter: any = {
    month: this.now.toLocaleString('en-EN', { month: 'long' }),
    year: `${this.now.getFullYear()}`,
    status: 'All',
    companyTypeOfUsage: 'All',
  };
  months: Array<string> = GenerateMonthList();
  years: Array<string> = GenerateYearList();
  status: Array<string> = GeneratePaymentStatusList();
  companyTypeOfUsages: Array<string> = GenerateCompanyTypeOfUsageList();

  dataTable: Array<any> = [];
  indexTable: Array<number> = [];
  currentIndex: number = 1;

  showPopup: boolean = false;
  paymentSelected: any = null;

  loading = false;

  constructor(private http: HttpClient) {
    this.filter.month = this.now.toLocaleString('en-EN', { month: 'long' });
    this.filter.year = `${this.now.getFullYear()}`;
  }

  ngOnInit() {
    this.getDataTable(1);
  }

  getDataTable(page: number) {
    this.http
      .post<DataIndex>(`${BaseUrl}Commission/GetUserCommissionIndex`, {
        page: page,
        ...this.filter,
      })
      .subscribe((res) => {
        if (res && res?.Data?.length >= 0) {
          this.dataTable = [];
          this.indexTable = res.Indices;
          this.currentIndex = res.Index;
          for (let item of res.Data) {
            if (item.ProfilePath) {
              item.ProfilePath = `${BaseUrl}${item.ProfilePath}`;
              item.month = `${this.filter.month}`;
              item.year = `${this.filter.year}`;
            }
            this.dataTable.push(item);
          }
          this.currentIndex = page;
        }
      });
  }

  payment(id: number) {
    this.http
      .post<DataIndex>(`${BaseUrl}Commission/Payment`, {
        userId: id,
        ...this.filter,
      })
      .subscribe((res) => {
        this.getDataTable(this.currentIndex);
      });
  }

  exportInvoiceAll() {
    for (let item of this.dataTable) {
      this.exportInvoice(item.Id, item.month, item.year);
    }
  }

  exportInvoice(id: number, month: string, year: string) {
    this.loading = true;
    this.http
      .post(
        `${BaseUrl}File/ExportInvoice`,
        { MobileUserId: id, Month: month, Year: year },
        { responseType: 'blob', observe: 'response' }
      )
      .subscribe((response: any) => {
        if (response && response?.body) {
          // Extract the filename from the response headers
          const contentDispositionHeader = response.headers.get(
            'content-disposition'
          );
          let filename = contentDispositionHeader?.split('filename=')[1];
          filename = filename?.split(';')[0];

          // Create a URL object from the response blob
          const blob = new Blob([response.body], {
            type: 'application/octet-stream',
          });
          const url = window.URL.createObjectURL(blob);

          // Create an anchor element and set its properties
          const link = document.createElement('a');
          link.href = url;
          link.download = filename ? filename : `INVOICE_00000000000.pdf`;

          // Programmatically click the anchor element to trigger the file download
          link.click();

          // Clean up the URL object
          window.URL.revokeObjectURL(url);
        }
        this.loading = false;
      });
  }

  onChange(type: string, value: string) {
    if (value) {
      if (type === 'filterMonth') {
        this.filter.month = value;
      } else if (type === 'filterYear') {
        this.filter.year = value;
      } else if (type === 'filterStatus') {
        this.filter.status = value;
      } else if (type === 'filterCompanyTypeOfUsage') {
        this.filter.companyTypeOfUsage = value;
      }
    }
  }

  format(type: string, value?: string) {
    return Format(type, value);
  }

  nextPage() {
    if (this.currentIndex + 1 <= this.indexTable.length) {
      this.getDataTable(this.currentIndex + 1);
    }
  }

  previousPage() {
    if (this.currentIndex - 1 > 0) {
      this.getDataTable(this.currentIndex - 1);
    }
  }

  openPopup(payment: any) {
    this.paymentSelected = payment;
    this.showPopup = true;
  }

  closePopup() {
    this.showPopup = false;
    this.paymentSelected = null;
  }
}
