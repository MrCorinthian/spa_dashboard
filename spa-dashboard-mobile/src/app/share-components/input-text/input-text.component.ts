import { Component, Input, Output, EventEmitter } from '@angular/core';

import { Colors } from '../../const-value/colors';

@Component({
  selector: 'input-text',
  templateUrl: './input-text.component.html',
  styleUrls: ['./input-text.component.scss', '../../styles.scss'],
})
export class InputTextComponent {
  @Input() value?: string | number;
  @Input() type: string = 'text';
  @Input() requireMark: boolean = false;
  @Input() disabled: boolean = false;
  @Input() placeholder: string = '';
  @Output() onChange = new EventEmitter();
  @Output() onBlur = new EventEmitter();

  colors: any = Colors;

  constructor() {}

  inputOnChange(value: any) {
    this.onChange.emit(value.value);
  }

  inputBlur() {
    this.onBlur.emit();
  }
}
