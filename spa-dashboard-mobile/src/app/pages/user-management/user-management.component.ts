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
  GenerateListWithAllOption,
} from '../../share-functions/generate-functions';
import { Format } from '../../share-functions/format-functions';
import { CloneObj } from '../../share-functions/clone-functions';
import { MobileUser } from '../../models/data/MobileUser';
import { MobileDropdown } from '../../models/data/MobileDropdown';
import { DataIndex } from '../../models/data-index';
import { validateHorizontalPosition } from '@angular/cdk/overlay';

@Component({
  selector: 'user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss', '../../styles.scss'],
})
export class UserManagementComponent {
  colors: any = Colors;
  baseUrl: string = BaseUrl;
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
  rowPerPage: number = 1;
  selected: MobileUser = new MobileUser();
  selectedEdit: MobileUser = new MobileUser();
  showPopup: boolean = false;
  isEdit: boolean = false;
  isCreate: boolean = false;
  isLoading: boolean = false;

  showPopupRqf: boolean = false;
  popupMessage: string = '';
  message: Array<string> = [];

  dropdowns: Array<MobileDropdown> = [];
  banks: Array<MobileDropdown> = [];
  occupations: Array<MobileDropdown> = [];
  provinces: Array<MobileDropdown> = [];
  tiers: Array<MobileDropdown> = [];
  status: Array<MobileDropdown> = GenerateStatusList();
  companyTypeOfUsages: Array<MobileDropdown> = [];
  companyTypeOfUsagesForEdit: Array<MobileDropdown> = [];

  constructor(private ref: ChangeDetectorRef, private http: HttpClient) {}

  async ngOnInit() {
    this.getTableIndex();
    this.banks = await this.getDropdown('BANK_NAME');
    this.occupations = await this.getDropdown('OCCUPATION');
    this.provinces = await this.getDropdown('PROVINCE');
    this.tiers = await this.GetTierList();
    this.companyTypeOfUsages = [
      ...GenerateListWithAllOption(),
      ...(await this.getDropdown('COM_TYPE_OF_USAGE')),
    ];
    this.companyTypeOfUsagesForEdit = await this.getDropdown(
      'COM_TYPE_OF_USAGE'
    );
  }

  search() {
    this.dataTable = [];
    this.http
      .post<DataIndex>(`${BaseUrl}User/GetMoblieUserIndex`, this.filter)
      .subscribe((res) => {
        if (res) {
          this.indexTable = res.Indices;
          this.rowPerPage = res.RowPerPage;
          if (res.Indices.length > 0) this.getDataTable(1);
        }
      });
  }

  getTableIndex(sort: string = 'asc') {
    this.dataTable = [];
    this.http
      .post<DataIndex>(`${BaseUrl}User/GetMoblieUserIndex`, this.filter)
      .subscribe((res) => {
        if (res) {
          this.indexTable = res.Indices;
          this.rowPerPage = res.RowPerPage;
          if (res.Indices.length > 0)
            this.getDataTable(
              sort === 'desc'
                ? res.Indices[res.Indices.length - 1]
                : res.Indices[0]
            );
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
              item.ProfilePath = `${item.ProfilePath}`;
            }
            if (item.Birthday) {
              item.BirthdayString = moment(item.Birthday).format('DD MMM YYYY');
            }
            item.IdCardPath = `${BaseUrl}File/UserAttachmentImageWeb?id=${
              item.Id
            }&time=${Date.now()}`;
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
            this.selectedEdit.ProfilePath = `${res.Data}`;
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
            this.selectedEdit.IdCardPath = `${BaseUrl}File/AttachmentImageWeb?filename=${res.Data}`;
            this.selectedEdit = CloneObj(this.selectedEdit);
          }
        });
    }
  }

  async checkRequiredField() {
    let validate = true;
    this.popupMessage = 'Required fields is missing';
    this.message = new Array<string>();
    if (!this.selectedEdit.ProfilePath) {
      validate = false;
      this.message.push('- Profile photo');
    }
    if (!this.selectedEdit.FirstName) {
      validate = false;
      this.message.push('- First name');
    }
    if (!this.selectedEdit.LastName) {
      validate = false;
      this.message.push('- Family name');
    }
    if (!this.selectedEdit.IdCardNumber) {
      validate = false;
      this.message.push('- ID card no.');
    }
    if (!this.selectedEdit.IdCardPath) {
      validate = false;
      this.message.push('- ID card photo');
    }
    if (!this.selectedEdit.Province) {
      validate = false;
      this.message.push('- Province');
    }
    if (!this.selectedEdit.Occupation) {
      validate = false;
      this.message.push('- Occupation');
    }
    if (this.selectedEdit.PhoneNumber) {
      const res = await firstValueFrom(
        this.http.post<any>(`${BaseUrl}User/TelephoneWeb`, {
          Id: this.selectedEdit.Id ? this.selectedEdit.Id : 0,
          PhoneNumber: this.selectedEdit.PhoneNumber,
        })
      );
      console.log(res);
      if (res) {
        if (res.Success) {
          validate = false;
          this.message.push('- Telephone no. already exists');
        }
      }
    } else {
      validate = false;
      this.message.push('- Telephone no.');
    }
    if (!this.selectedEdit.CompanyTypeOfUsage) {
      validate = false;
      this.message.push('- Type of usage');
    }
    if (this.selectedEdit.CompanyTypeOfUsage == '99') {
      if (!this.selectedEdit.CompanyName) {
        validate = false;
        this.message.push('- Company name');
      }
      if (!this.selectedEdit.CompanyTaxId) {
        validate = false;
        this.message.push('- Company tax ID');
      }
      if (!this.selectedEdit.CompanyAddress) {
        validate = false;
        this.message.push('- Company address');
      }
    }
    if (!this.selectedEdit.Bank) {
      validate = false;
      this.message.push('- Bank name');
    }
    if (!this.selectedEdit.BankAccountNumber) {
      validate = false;
      this.message.push('- Bank account no.');
    }
    if (this.isCreate) {
      if (!this.selectedEdit.Password) {
        validate = false;
        this.message.push('- Password');
      }
      if (!this.selectedEdit.ConfirmPassword) {
        validate = false;
        this.message.push('- Confirm password');
      }
      if (
        this.selectedEdit.Password &&
        this.selectedEdit.ConfirmPassword &&
        (this.selectedEdit.Password !== this.selectedEdit.ConfirmPassword ||
          this.selectedEdit.Password.length < 6)
      ) {
        this.message = new Array<string>();
        validate = false;
        this.popupMessage = 'Passwords do not match';
        this.message.push(
          'Your password and confirmation password do not match, please enter again'
        );
      }
    }
    if (!validate) this.openPopupRqf();
    return validate;
  }

  async createUser() {
    if (await this.checkRequiredField()) {
      let fromData = CloneObj(this.selectedEdit);
      this.http
        .post<MobileUser>(`${BaseUrl}User/CreateUser`, fromData)
        .subscribe((res) => {
          if (res?.Id) {
            //update id card
            this.http
              .post<any>(`${BaseUrl}File/UpdateUserAttachment`, {
                MobileUserId: res.Id,
                Filename: fromData.idCardFilename ?? '',
              })
              .subscribe((resUserAtt) => {
                console.log(resUserAtt);
                if (resUserAtt != null && resUserAtt?.Data)
                  this.selected.IdCardPath = `${BaseUrl}${resUserAtt.Data}`;
              });
            this.getTableIndex('desc');
            this.closePopup();
          }
        });
    }
  }

  async updateUser() {
    if (await this.checkRequiredField()) {
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

            if (res.Birthday) {
              res.BirthdayString = moment(res.Birthday).format('DD MMM YYYY');
            }

            let tier = this.dataTable.find((c) => c.Id === res.Id);
            if (tier) {
              this.getDataTable(this.currentIndex);
              res.ProfilePath = `${res.ProfilePath}`;
              this.selected = res;
              this.isEdit = false;
              this.isCreate = false;
            }
          }
        });
    }
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
    if (this.selected) {
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
      } else if (type === 'BankName') {
        this.selectedEdit.Bank = value;
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
        console.log(value);
        console.log(this.selectedEdit.Birthday);
        this.selectedEdit.BirthdayString = value;
      }
    }
    this.selectedEdit = CloneObj(this.selectedEdit);
  }

  findDropdownValue(dropdowns: Array<MobileDropdown>, id: number | String) {
    const find = dropdowns.find((c) => c.Id == id);
    if (find != null) return find.Value;
    else return id;
  }

  async getDropdown(code: string) {
    return await firstValueFrom(
      this.http.get<Array<MobileDropdown>>(
        `${BaseUrl}Data/GetDropdown?code=${code}`
      )
    );
  }

  async GetTierList() {
    return await firstValueFrom(
      this.http.get<Array<MobileDropdown>>(`${BaseUrl}Data/GetTierList`)
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

  openPopupRqf() {
    this.showPopupRqf = true;
  }

  closePopupRqf() {
    this.showPopupRqf = false;
    this.popupMessage = '';
    this.message = new Array<string>();
  }
}
