import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormControl } from '@angular/forms';
import { startWith, map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { Colors } from '../../const-value/colors';

@Component({
  selector: 'autocomplete',
  templateUrl: './autocomplete.component.html',
  styleUrls: ['./autocomplete.component.scss', '../../styles.scss'],
})
export class AutocompleteComponent {
  @Input() value?: string;
  @Input() placeholder: string = '';
  @Input() requireMark: boolean = false;
  @Input() disabled: boolean = false;
  @Input() options: Array<string> = new Array<string>();
  @Input() sortOption: boolean = true;
  @Output() onChange = new EventEmitter();
  @Output() onBlur = new EventEmitter();

  colors: any = Colors;

  filtered!: Observable<string[]>;
  control = new FormControl('');

  constructor() {}
  ngOnInit() {
    this.filtered = this.control.valueChanges.pipe(
      startWith(''),
      map((value) => this._filter(value || ''))
    );
    if (this.value) this.control.setValue(this.value);
  }

  private _filter(value: string): string[] {
    const filterValue = this._normalizeValue(value);
    if (this.sortOption) this.options.sort();
    let option_1: Array<string> = new Array<string>();
    let option_2: Array<string> = new Array<string>();
    for (let item of this.options) {
      if (this._normalizeValue(item).includes(filterValue)) {
        option_1.push(item);
      } else {
        option_2.push(item);
      }
    }
    return [...option_1, ...option_2];
  }

  private _normalizeValue(value: string): string {
    return value.toLowerCase().replace(/\s/g, '');
  }

  inputOnChange(value: any) {
    this.onChange.emit(value);
  }

  inputBlur() {
    this.onBlur.emit();
  }
}
