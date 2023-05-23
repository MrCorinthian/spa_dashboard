import { Component, Input, Output, EventEmitter } from '@angular/core';
import * as moment from 'moment';

import { Colors } from '../../const-value/colors';

@Component({
  selector: 'daterangepicker',
  templateUrl: './daterangepicker.component.html',
  styleUrls: ['./daterangepicker.component.scss', '../../styles.scss'],
})
export class DaterangepickerComponent {
  @Input() value?: string | number;
  @Input() requireMark: boolean = false;
  @Input() disabled: boolean = false;
  @Input() placeholder: string = '';
  @Output() onChange = new EventEmitter();
  @Output() onBlur = new EventEmitter();

  colors: any = Colors;

  selected: { from: Date | null; to: Date | null } = { from: null, to: null };

  constructor() {}

  onChanceDate(type: string, event: any) {
    if (event.value) {
      if (type === 'from') {
        this.selected.from = event.value;
      } else if (type === 'to') {
        this.selected.to = event.value;
        let dateFrom: string = moment(this.selected.from).format('DD MMM YYYY');
        let dateTo: string = moment(this.selected.to).format('DD MMM YYYY');
        this.value = `${dateFrom} - ${dateTo}`;
        this.onChange.emit(this.value);
      }
    }
  }

  inputOnChange(value: any) {
    this.onChange.emit(value?.value);
  }

  // inputBlur() {
  //   this.onBlur.emit();
  // }
}
