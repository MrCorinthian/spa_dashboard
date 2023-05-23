import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseUrl } from '../const-value/base-url';

import { Colors } from '../const-value/colors';

@Component({
  selector: 'nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss'],
})
export class NavMenuComponent {
  colors: any = Colors;

  constructor(private http: HttpClient) {}

  logout() {
    this.http.post<any>(`${BaseUrl}User/WebLogout`, null).subscribe((res) => {
      window.location.href = `${BaseUrl.replace('api/', '')}`;
    });
  }
}
