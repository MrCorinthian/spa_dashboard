import { Component, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import Chart from 'chart.js/auto';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { BaseUrl } from '../../const-value/base-url';
import { Colors } from '../../const-value/colors';
import { Format } from '../../share-functions/format-functions';
import {
  GenerateMonthList,
  GenerateYearList,
} from '../../share-functions/generate-functions';

import { ReportBranch } from '../../models/report-branch';

@Component({
  selector: 'commission-dashboard',
  templateUrl: './commission-dashboard.component.html',
  styleUrls: ['./commission-dashboard.component.scss', '../../styles.scss'],
})
export class CommissionDashboardComponent {
  @ViewChild('chartCanvas', { static: true }) chartCanvas!: ElementRef;

  now: Date = new Date();
  colors: any = Colors;
  dataTable: Array<any> = [];
  indexTable: Array<number> = [];
  currentIndex: number = 1;
  branchChart: any;

  filter: any = {
    month: this.now.toLocaleString('default', { month: 'long' }),
    year: `${this.now.getFullYear()}`,
  };
  months: Array<string> = GenerateMonthList();
  years: Array<string> = GenerateYearList();

  constructor(private http: HttpClient) {
    Chart.register(ChartDataLabels);
  }

  ngOnInit() {
    this.getData();
  }

  getData() {
    this.http
      .post<Array<ReportBranch>>(`${BaseUrl}Commission/GetCommission`, {
        month: this.filter.month,
        year: this.filter.year,
      })
      .subscribe((res) => {
        if (res) {
          this.generateChart(res);
        }
      });
  }

  generateChart(data: Array<ReportBranch>) {
    const canvas: HTMLCanvasElement = this.chartCanvas.nativeElement;
    const ctx = canvas.getContext('2d');
    if (ctx) {
      this.branchChart = new Chart(ctx, {
        type: 'pie',
        data: {
          labels: data.map(
            (m) =>
              `${m.BranchName}\n(${m.TotalPercentage.toFixed(2)}% ${Format(
                'number',
                m.TotalBaht.toString()
              )} Baht)`
          ),
          datasets: [
            {
              label: '',
              data: data.map((m) => m.TotalPercentage),
              backgroundColor: this.generateGreenPalette(data.length),
              borderWidth: 0.5,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          borderColor: '#3d3d3d',
          layout: {
            padding: {
              top: 50,
              right: 0,
              bottom: 50,
              left: 0,
            },
          },
          plugins: {
            tooltip: {
              callbacks: {
                label: (tooltipItem) => '',
              },
            },
            legend: {
              display: false,
            },
            datalabels: {
              color: 'black',
              font: {
                weight: 'bold',
              },
              formatter: (value, context: any) =>
                `${context.chart.data.labels[context.dataIndex]}`,
              anchor: 'end',
              align: 'end',
              offset: 10,
            },
          },
        },
      });
    }
  }

  getDataTable(page: number) {
    this.http
      .get<Array<any>>(`${BaseUrl}User/GetMoblieUserOverview?page=${page}`)
      .subscribe((res) => {
        if (res?.length > 0) {
          this.dataTable = [];
          for (let item of res) {
            if (item.ProfilePath)
              item.ProfilePath = `${BaseUrl}${item.ProfilePath}`;
            this.dataTable.push(item);
          }
          this.currentIndex = page;
        }
      });
  }

  onChangeFilter(type: string, value: string) {
    if (type === 'filterMonth') {
      this.filter.month = value;
    } else if (type === 'filterYear') {
      this.filter.year = value;
    }
  }

  generateGreenPalette(shades: number) {
    let greenColors = [];
    const baseHue = 100; // Green hue
    const saturation = 40;
    const lightnessStep = 5;

    for (let i = 0; i < shades; i++) {
      const lightness = (i + 4) * lightnessStep;
      const color = `hsl(${baseHue}, ${saturation}%, ${lightness}%)`;
      greenColors.push(color);
    }

    return greenColors;
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
