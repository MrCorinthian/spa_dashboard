import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseUrl } from '../../const-value/base-url';
import { Colors } from '../../const-value/colors';
import { GenerateStatusList } from '../../share-functions/generate-functions';
import { CloneObj } from '../../share-functions/clone-functions';
import { MobileComTier } from '../../models/data/MobileComTier';
import { MobileDropdown } from '../../models/data/MobileDropdown';

@Component({
  selector: 'commission-setting',
  templateUrl: './commission-setting.component.html',
  styleUrls: ['./commission-setting.component.scss', '../../styles.scss'],
})
export class CommissionSettingComponent {
  colors: any = Colors;

  filter: any = { tierName: '', status: '' };
  status: Array<MobileDropdown> = GenerateStatusList();

  dataTable: Array<any> = [];
  selected: MobileComTier = new MobileComTier();
  selectedEdit: MobileComTier = new MobileComTier();
  showPopup: boolean = false;
  isEdit: boolean = false;
  isCreate: boolean = false;
  isLoading: boolean = false;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getDataTable();
  }

  getDataTable() {
    this.http
      .post<Array<any>>(`${BaseUrl}Data/GetCommissionTierSetting`, this.filter)
      .subscribe((res) => {
        this.dataTable = [];
        if (res?.length > 0) {
          for (let item of res) {
            if (item.TierColor)
              item.TierColor = item.TierColor.replace('FF', '#').toLowerCase();
            this.dataTable.push(item);
          }
        }
      });
  }

  createTier() {
    let fromData = CloneObj(this.selectedEdit);
    this.http
      .post<MobileComTier>(`${BaseUrl}Data/CreateTier`, fromData)
      .subscribe((res) => {
        if (res?.Id) {
          this.getDataTable();
          this.closePopup();
        }
      });
  }

  updateTier() {
    this.http
      .post<any>(`${BaseUrl}Data/UpdateTierCommission`, this.selectedEdit)
      .subscribe((res) => {
        if (res?.Id) {
          let tier = this.dataTable.find((c) => c.Id === res.Id);
          if (tier) {
            res.TierColor = res.TierColor.replace('FF', '#').toLowerCase();
            this.getDataTable();
            this.selected = res;
            this.isEdit = false;
          }
        }
      });
  }

  deleteTier() {
    let fromData = CloneObj(this.selectedEdit);
    this.http
      .post<any>(`${BaseUrl}Data/DeleteTier`, fromData)
      .subscribe((res) => {
        if (res) {
          this.getDataTable();
          this.closePopup();
        }
      });
  }

  onChangeFilter(type: string, value: string) {
    if (type === 'filterTierName') {
      this.filter.tierName = value;
    } else if (type === 'filterStatus') {
      this.filter.status = value;
    }
  }

  onChange(type: string, value: any) {
    if (this.selected && (value || value === 0)) {
      if (type === 'TierName') {
        this.selectedEdit.TierName = value;
      } else if (type === 'TierColor') {
        this.selectedEdit.TierColor = value;
      } else if (type === 'ComPercentage') {
        this.selectedEdit.ComPercentage = Number.parseFloat(value);
      } else if (type === 'ComBahtFrom') {
        this.selectedEdit.ComBahtFrom = Number.parseFloat(value);
      } else if (type === 'ComBahtTo') {
        this.selectedEdit.ComBahtTo = Number.parseFloat(value);
      } else if (type === 'Active') {
        this.selectedEdit.Active = value.checked ? 'Y' : 'N';
      }
    }
  }

  openCreate() {
    this.showPopup = true;
    this.isEdit = true;
    this.isCreate = true;
    this.selectedEdit = new MobileComTier();
  }

  openEdit() {
    this.isEdit = true;
    this.isCreate = false;
    this.selectedEdit = CloneObj(this.selected);
  }

  closeEdit() {
    this.isEdit = false;
    this.isCreate = false;
    this.selected = new MobileComTier();
    this.selectedEdit = new MobileComTier();
  }

  openPopup(value: MobileComTier) {
    this.showPopup = true;
    this.isEdit = false;
    this.isCreate = false;
    this.selected = CloneObj(value);
  }

  closePopup() {
    this.showPopup = false;
    this.isEdit = false;
    this.isCreate = false;
  }
}
