import { Component, EventEmitter, Output } from "@angular/core";

@Component({
    selector: "app-time-picker",
    templateUrl: "./time-picker.component.html",
    styleUrls: ["./time-picker.component.css"]
})
export class TimePickerComponent {

    @Output() private readonly timeSetEvent = new EventEmitter<number[]>();

    public hour = 0;
    public minute = 0;

    public inputChanged(): void {
        this.timeSetEvent.emit([this.hour, this.minute]);
    }
}