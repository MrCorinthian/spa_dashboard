import { Component, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import * as moment from 'moment';
import { BaseUrl } from '../../const-value/base-url';
import { Colors } from '../../const-value/colors';
import {
  GenerateStatusList,
  GenerateCompanyTypeOfUsageList,
  GenerateCompanyTypeOfUsageListForEdit,
} from '../../share-functions/generate-functions';
import { Format } from '../../share-functions/format-functions';
import { CloneObj } from '../../share-functions/clone-functions';
import { MobileUser } from '../../models/data/MobileUser';

@Component({
  selector: 'user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss', '../../styles.scss'],
})
export class UserManagementComponent {
  colors: any = Colors;

  filter: any = {
    firstName: '',
    lastName: '',
    phone: '',
    status: 'All',
    IdCardNumber: '',
    companyTypeOfUsage: 'All',
    companyTaxId: '',
    companyName: '',
    tierName: 'All',
  };

  dataTable: Array<any> = [];
  indexTable: Array<number> = [];
  currentIndex: number = 1;
  selected: MobileUser = new MobileUser();
  selectedEdit: MobileUser = new MobileUser();
  showPopup: boolean = false;
  isEdit: boolean = false;
  isCreate: boolean = false;
  isLoading: boolean = false;

  bankAccounts: Array<string> = [];
  occupations: Array<string> = [];
  provinces: Array<string> = [];
  tiers: Array<string> = [];
  status: Array<string> = GenerateStatusList();
  companyTypeOfUsages: Array<string> = GenerateCompanyTypeOfUsageList();
  companyTypeOfUsagesForEdit: Array<string> =
    GenerateCompanyTypeOfUsageListForEdit();

  constructor(private ref: ChangeDetectorRef, private http: HttpClient) {}

  async ngOnInit() {
    this.getTableIndex();
    this.bankAccounts = await this.getDropdown('BANK_ACCOUNT');
    this.occupations = await this.getDropdown('OCCUPATION');
    this.provinces = await this.getDropdown('PROVINCE');
    this.tiers = await this.GetTierList();
  }

  search() {
    this.http
      .post<Array<number>>(`${BaseUrl}User/GetMoblieUserIndex`, this.filter)
      .subscribe((res) => {
        if (res && res.length > 0) {
          this.indexTable = res;
          this.getDataTable(1);
        }
      });
  }

  getTableIndex(sort: string = 'asc') {
    this.http
      .post<Array<number>>(`${BaseUrl}User/GetMoblieUserIndex`, this.filter)
      .subscribe((res) => {
        if (res && res.length > 0) {
          this.indexTable = res;
          this.getDataTable(sort === 'desc' ? res[res.length - 1] : res[0]);
        }
      });
  }

  getDataTable(page: number) {
    this.dataTable = [];
    this.http
      .post<Array<MobileUser>>(`${BaseUrl}User/GetMoblieUser`, {
        page: page,
        ...this.filter,
      })
      .subscribe((res) => {
        if (res?.length > 0) {
          for (let item of res) {
            if (item.ProfilePath) {
              item.ProfilePath = `${BaseUrl}${item.ProfilePath}`;
            }
            if (item.Birthday) {
              item.BirthdayString = moment(item.Birthday).format('DD MMM YYYY');
            }
            item.IdCardPath = `${BaseUrl}File/UserAttachmentImageWeb?id=${item.Id}`;
            this.dataTable.push(item);
          }
          this.currentIndex = page;
        }
      });
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      const formData = new FormData();
      formData.append('thumbnail', file);
      this.http
        .post<any>(`${BaseUrl}File/UploadImage`, formData)
        .subscribe((res) => {
          if (res || res?.Data) {
            this.selectedEdit.ProfilePath = `${BaseUrl}File/ProfileImageWebUpload?fileName=${res.Data}`;
            this.selectedEdit = CloneObj(this.selectedEdit);
          }
        });
    }
  }

  onIdCardFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      const formData = new FormData();
      formData.append('thumbnail', file);
      this.http
        .post<any>(`${BaseUrl}File/UploadAttachment`, formData)
        .subscribe((res) => {
          if (res || res?.Data) {
            this.selectedEdit.idCardFilename = res.Data;
            this.selectedEdit.IdCardPath = `${BaseUrl}File/AttachmentImageWeb?filename=${this.selectedEdit.idCardFilename}`;
            this.selectedEdit = CloneObj(this.selectedEdit);
          }
        });
    }
  }

  createUser() {
    let fromData = CloneObj(this.selectedEdit);
    this.http
      .post<MobileUser>(`${BaseUrl}User/CreateUser`, fromData)
      .subscribe((res) => {
        if (res?.Id) {
          this.getTableIndex('desc');
          this.closePopup();
        }
      });
  }

  updateUser() {
    let fromData = CloneObj(this.selectedEdit);
    const fileName = fromData?.ProfilePath?.split('fileName=');
    if (fileName) fromData.ProfilePath = fileName[fileName.length - 1];
    this.http
      .post<any>(`${BaseUrl}User/UpdateUserInformation`, fromData)
      .subscribe((res) => {
        if (res?.Id) {
          //update id card
          this.http
            .post<any>(`${BaseUrl}File/UpdateUserAttachment`, {
              MobileUserId: res.Id,
              Filename: this.selectedEdit.idCardFilename ?? '',
            })
            .subscribe((resUserAtt) => {
              if (resUserAtt != null && resUserAtt?.Data)
                this.selected.IdCardPath = `${BaseUrl}${resUserAtt.Data}`;
            });

          let tier = this.dataTable.find((c) => c.Id === res.Id);
          if (tier) {
            this.getDataTable(this.currentIndex);
            res.ProfilePath = `${BaseUrl}${res.ProfilePath}`;
            this.selected = res;
            this.isEdit = false;
            this.isCreate = false;
          }
        }
      });
  }

  deleteUser() {
    let fromData = CloneObj(this.selectedEdit);
    this.http
      .post<any>(`${BaseUrl}User/DeleteUser`, fromData)
      .subscribe((res) => {
        if (res) {
          this.getDataTable(1);
          this.closePopup();
        }
      });
  }

  onChangeFilter(type: string, value: string) {
    if (type === 'filterFirstName') {
      this.filter.firstName = value;
    } else if (type === 'filterLastName') {
      this.filter.lastName = value;
    } else if (type === 'filterStatus') {
      this.filter.status = value;
    } else if (type === 'filterPhone') {
      this.filter.phone = value;
    } else if (type === 'filterIdCardNo') {
      this.filter.IdCardNumber = value;
    } else if (type === 'filterCompanyTypeOfUsage') {
      this.filter.companyTypeOfUsage = value;
    } else if (type === 'filterCompanyName') {
      this.filter.companyName = value;
    } else if (type === 'filterCompanyTaxId') {
      this.filter.companyTaxId = value;
    } else if (type === 'filterTierName') {
      this.filter.tierName = value;
    }
  }

  onChange(type: string, value: any) {
    if (this.selected && (value || value === 0)) {
      if (type === 'Username') {
        this.selectedEdit.Username = value;
      } else if (type === 'Password') {
        this.selectedEdit.Password = value;
      } else if (type === 'ConfirmPassword') {
        this.selectedEdit.ConfirmPassword = value;
      } else if (type === 'IdCardNumber') {
        this.selectedEdit.IdCardNumber = value;
      } else if (type === 'FirstName') {
        this.selectedEdit.FirstName = value;
      } else if (type === 'LastName') {
        this.selectedEdit.LastName = value;
      } else if (type === 'Occupation') {
        this.selectedEdit.Occupation = value;
      } else if (type === 'PhoneNumber') {
        this.selectedEdit.PhoneNumber = value;
      } else if (type === 'LineId') {
        this.selectedEdit.LineId = value;
      } else if (type === 'WhatsAppId') {
        this.selectedEdit.WhatsAppId = value;
      } else if (type === 'Email') {
        this.selectedEdit.Email = value;
      } else if (type === 'BankAccount') {
        this.selectedEdit.BankAccount = value;
      } else if (type === 'BankAccountNumber') {
        this.selectedEdit.BankAccountNumber = value;
      } else if (type === 'CompanyName') {
        this.selectedEdit.CompanyName = value;
      } else if (type === 'CompanyTaxId') {
        this.selectedEdit.CompanyTaxId = value;
      } else if (type === 'CompanyAddress') {
        this.selectedEdit.CompanyAddress = value;
      } else if (type === 'Nationality') {
        this.selectedEdit.Nationality = value;
      } else if (type === 'Address') {
        this.selectedEdit.Address = value;
      } else if (type === 'Province') {
        this.selectedEdit.Province = value;
      } else if (type === 'Active') {
        this.selectedEdit.Active = value.checked ? 'Y' : 'N';
      } else if (type === 'CompanyTypeOfUsage') {
        this.selectedEdit.CompanyTypeOfUsage = value;
      } else if (type === 'Birthday') {
        this.selectedEdit.Birthday = moment(value, 'DD MMM YYYY').toDate();
        this.selectedEdit.BirthdayString = value;
      }
    }
  }

  async getDropdown(code: string) {
    return await firstValueFrom(
      this.http.get<Array<string>>(`${BaseUrl}Data/GetDropdown?code=${code}`)
    );
  }

  async GetTierList() {
    return await firstValueFrom(
      this.http.get<Array<string>>(`${BaseUrl}Data/GetTierList`)
    );
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

  openCreate() {
    this.isEdit = true;
    this.isCreate = true;
    this.showPopup = true;
    this.selectedEdit = new MobileUser();
  }

  openEdit() {
    this.isEdit = true;
    this.isCreate = false;
    this.selectedEdit = CloneObj(this.selected);
  }

  closeEdit() {
    this.isEdit = false;
    this.isCreate = false;
    this.selectedEdit = new MobileUser();
  }

  openPopup(value: MobileUser) {
    this.showPopup = true;
    this.selected = CloneObj(value);
  }

  closePopup() {
    this.showPopup = false;
    this.isEdit = false;
    this.isCreate = false;
    this.selected = new MobileUser();
    this.selectedEdit = new MobileUser();
  }
}
