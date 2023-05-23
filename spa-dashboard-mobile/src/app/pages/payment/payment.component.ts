import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseUrl } from '../../const-value/base-url';
import { Colors } from '../../const-value/colors';
import { DataIndex } from '../../models/data-index';
import { Format } from '../../share-functions/format-functions';
import {
  GenerateMonthList,
  GenerateYearList,
  GeneratePaymentStatusList,
} from '../../share-functions/generate-functions';

@Component({
  selector: 'payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss', '../../styles.scss'],
})
export class PaymentComponent {
  colors: any = Colors;
  now: Date = new Date();
  filter: any = { month: '', year: '', status: 'All' };
  months: Array<string> = GenerateMonthList();
  years: Array<string> = GenerateYearList();
  status: Array<string> = GeneratePaymentStatusList();

  dataTable: Array<any> = [];
  indexTable: Array<number> = [];
  currentIndex: number = 1;

  showPopup: boolean = false;
  paymentSelected: any = null;

  constructor(private http: HttpClient) {
    this.filter.month = this.now.toLocaleString('default', { month: 'long' });
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

  onChange(type: string, value: string) {
    if (value) {
      if (type === 'filterMonth') {
        this.filter.month = value;
      } else if (type === 'filterYear') {
        this.filter.year = value;
      } else if (type === 'filterStatus') {
        this.filter.status = value;
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
