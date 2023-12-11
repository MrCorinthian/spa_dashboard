import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { BaseUrl } from '../../const-value/base-url';
import { Colors } from '../../const-value/colors';
import { DataIndex } from '../../models/data-index';
import { MobileDropdown } from '../../models/data/MobileDropdown';
import { Format } from '../../share-functions/format-functions';
import {
  GenerateMonthList,
  GenerateYearList,
  GeneratePaymentStatusList,
  GenerateListWithAllOption,
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
  months: Array<MobileDropdown> = GenerateMonthList();
  years: Array<MobileDropdown> = GenerateYearList();
  status: Array<MobileDropdown> = GeneratePaymentStatusList();
  companyTypeOfUsages: Array<MobileDropdown> = [];

  dataTable: Array<any> = [];
  indexTable: Array<number> = [];
  currentIndex: number = 1;
  rowPerPage: number = 1;

  showPopup: boolean = false;
  paymentSelected: any = null;

  loading: number = 0;

  constructor(private http: HttpClient) {
    this.filter.month = this.now.toLocaleString('en-EN', { month: 'long' });
    this.filter.year = `${this.now.getFullYear()}`;
  }

  async ngOnInit() {
    this.getDataTable(1);
    this.companyTypeOfUsages = [
      ...GenerateListWithAllOption(),
      ...(await this.getDropdown('COM_TYPE_OF_USAGE')),
    ];
  }

  getDataTable(page: number) {
    this.dataTable = [];
    this.http
      .post<DataIndex>(`${BaseUrl}Commission/GetUserCommissionIndex`, {
        page: page,
        ...this.filter,
      })
      .subscribe((res) => {
        if (res) {
          this.indexTable = res.Indices;
          this.currentIndex = res.Index;
          if (res?.Data?.length >= 0) {
            this.rowPerPage = res.RowPerPage;
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
        }
      });
  }

  async getDropdown(code: string) {
    return await firstValueFrom(
      this.http.get<Array<MobileDropdown>>(
        `${BaseUrl}Data/GetDropdown?code=${code}`
      )
    );
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

  async exportInvoiceAll() {
    for (let item of this.dataTable) {
      await this.exportInvoice(item.Id, item.month, item.year);
    }
  }

  async exportInvoice(id: number, month: string, year: string) {
    this.loading++;
    const response = await firstValueFrom(
      this.http.post(
        `${BaseUrl}File/ExportInvoice`,
        { MobileUserId: id, Month: month, Year: year },
        { responseType: 'blob', observe: 'response' }
      )
    );
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
    this.loading--;
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
