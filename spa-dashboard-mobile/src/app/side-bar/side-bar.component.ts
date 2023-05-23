import { Component, Input, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Colors } from '../const-value/colors';
import { MenuOption } from '../models/menu-option-model';

@Component({
  selector: 'side-bar',
  templateUrl: './side-bar.component.html',
  styleUrls: ['./side-bar.component.scss'],
})
export class SideBarComponent {
  http!: HttpClient;
  colors: any = Colors;
  @Input() options: Array<MenuOption> = [];
  @Input() selected: number = 0;
  @Output() changeSelect = new EventEmitter();

  onChangeSelect(value: number) {
    this.changeSelect.emit(value);
  }
}
