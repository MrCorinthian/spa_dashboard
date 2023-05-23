import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.scss'],
})
export class PopupComponent {
  @Input() openPopup!: boolean;
  @Output() closePopup: EventEmitter<any> = new EventEmitter();
}
