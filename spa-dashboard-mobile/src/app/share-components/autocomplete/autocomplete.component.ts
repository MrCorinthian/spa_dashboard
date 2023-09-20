import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormControl } from '@angular/forms';
import { startWith, map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { Colors } from '../../const-value/colors';
import { MobileDropdown } from '../../models/data/MobileDropdown';

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
  @Input() options: Array<MobileDropdown> = new Array<MobileDropdown>();
  @Input() sortOption: boolean = true;
  @Output() onChange = new EventEmitter();
  @Output() onBlur = new EventEmitter();

  colors: any = Colors;

  filtered!: Observable<Array<MobileDropdown>>;
  control = new FormControl('');

  constructor() {}
  ngOnInit() {
    this.filtered = this.control.valueChanges.pipe(
      startWith(''),
      map((value) => this._filter(value || ''))
    );
    if (this.value) this.control.setValue(this.value);
  }

  private _filter(value: string): Array<MobileDropdown> {
    const filterValue = this._normalizeValue(value);
    if (this.sortOption)
      this.options.sort((a, b) => a.Value.localeCompare(b.Value));
    let option_1: Array<MobileDropdown> = new Array<MobileDropdown>();
    let option_2: Array<MobileDropdown> = new Array<MobileDropdown>();
    for (let item of this.options) {
      if (this._normalizeValue(item.Value).includes(filterValue)) {
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

  findDropdownValue(id: number): string | null {
    const find = this.options.find((c) => c.Id == id);
    if (find != null) return find.Value;
    else return null;
  }

  inputOnChange(option: number) {
    this.onChange.emit(option);
    if (option) this.control.setValue(this.findDropdownValue(option));
  }

  inputBlur() {
    this.onBlur.emit();
  }
}
