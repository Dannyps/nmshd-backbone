import { Component, OnInit } from "@angular/core";

@Component({
    selector: "app-time-picker",
    templateUrl: "./time-picker.component.html",
    styleUrls: ["./time-picker.component.css"]
})
export class TimePickerComponent implements OnInit {
    public hour = 0;
    public minute = 0;
    
    public ngOnInit(): void {
        throw new Error("Method not implemented.");
    }
    
}