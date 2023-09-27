import { Component, OnInit } from "@angular/core";

@Component({
    selector: "app-time-picker",
    templateUrl: "./time-picker.component.html",
    styleUrls: ["./time-picker.component.css"]
})
export class TimePickerComponent implements OnInit {
    public hour = 0;
    public hours = [...Array(24).keys()]

    public minute = 0;
    public minutes = [...Array(60).keys()]
    
    public ngOnInit(): void {
        throw new Error("Method not implemented.");
    }
    
}