import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseUrl } from '../../const-value/base-url';
import * as moment from 'moment';
import { Colors } from '../../const-value/colors';
import { Format } from '../../share-functions/format-functions';
import { DataIndex } from '../../models/data-index';

@Component({
  selector: 'report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss', '../../styles.scss'],
})
export class ReportComponent {
  colors: any = Colors;
  filter: any = {
    firstName: '',
    lastName: '',
    phoneNumber: '',
    periodFrom: null,
    periodTo: null,
    periodFromTo: null,
  };

  dataTable: Array<any> = [];
  indexTable: Array<number> = [];
  currentIndex: number = 1;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getDataTable(1);
  }

  getDataTable(page: number) {
    this.http
      .post<DataIndex>(`${BaseUrl}Commission/GetCommissionReportIndex`, {
        page: page,
        ...this.filter,
      })
      .subscribe((res) => {
        if (res && res?.Data?.length >= 0) {
          this.dataTable = [];
          this.indexTable = res.Indices;
          this.currentIndex = res.Index;
          for (let item of res.Data) {
            this.dataTable.push(item);
          }
          this.currentIndex = page;
        }
      });
  }

  onChangeFilter(type: string, value: string) {
    if (value) {
      if (type === 'filterFirstName') {
        this.filter.firstName = value;
      } else if (type === 'filterLastName') {
        this.filter.lastName = value;
      } else if (type === 'filterPhone') {
        this.filter.phoneNumber = value;
      } else if (type === 'filterPeriod') {
        let fromTo = value.split(' - ');
        if (fromTo.length === 2) {
          this.filter.periodFromTo = value;
          this.filter.periodFrom = fromTo[0];
          this.filter.periodTo = fromTo[1];
        }
      }
    } else if (type === 'filterPeriod') {
      this.filter.periodFromTo = null;
      this.filter.periodFrom = null;
      this.filter.periodTo = null;
    }
  }

  dateFormat(date: Date | null, format: string) {
    if (date) {
      return moment(date).format(format);
    } else {
      return '';
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
}
