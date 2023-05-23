import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { BaseUrl } from './const-value/base-url';
import { MenuOption } from './models/menu-option-model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'spa-dashboard-mobile'.toLowerCase();
  menuSelected: number = 0;
  menuOptions: Array<MenuOption> = [
    new MenuOption(0, 'Commission Dashboard'),
    new MenuOption(1, 'Payment'),
    new MenuOption(2, 'Report'),
    new MenuOption(3, 'User management'),
    new MenuOption(4, 'Commission setting'),
  ];

  constructor(private _route: ActivatedRoute, private _location: Location) {
    this._route.queryParams.subscribe((params) => {
      const _index = params['index'];
      if (_index) this.menuSelected = Number.parseInt(_index);
    });
  }

  onChangeMenu(value: number) {
    this._location.go(`?index=${value}`);
    this.menuSelected = value;
  }
}
