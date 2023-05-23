import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { SideBarComponent } from './side-bar/side-bar.component';
import { PopupComponent } from './popup/popup.component';
import { CommissionDashboardComponent } from './pages/commission-dashborad/commission-dashboard.component';
import { PaymentComponent } from './pages/payment/payment.component';
import { ReportComponent } from './pages/report/report.component';
import { UserManagementComponent } from './pages/user-management/user-management.component';
import { CommissionSettingComponent } from './pages/commission-setting/commission-setting.component';

import { InputTextComponent } from './share-components/input-text/input-text.component';
import { AutocompleteComponent } from './share-components/autocomplete/autocomplete.component';
import { DaterangepickerComponent } from './share-components/daterangepicker/daterangepicker.component';

const routes: Routes = [
  // {
  //   path: 'Dashboard',
  //   component: CommissionDashboardComponent,
  //   pathMatch: 'full',
  // },
];

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    SideBarComponent,
    PopupComponent,
    CommissionDashboardComponent,
    PaymentComponent,
    ReportComponent,
    UserManagementComponent,
    CommissionSettingComponent,

    InputTextComponent,
    AutocompleteComponent,
    DaterangepickerComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RouterModule.forRoot(routes),
    HttpClientModule,
    ReactiveFormsModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatSlideToggleModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatInputModule,
    MatDatepickerModule,
  ],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
