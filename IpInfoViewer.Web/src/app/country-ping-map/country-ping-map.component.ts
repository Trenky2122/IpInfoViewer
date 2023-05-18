import { Component } from '@angular/core';
import {IpAddressInfoViewerService} from "../ip-address-info-viewer.service";

@Component({
  selector: 'app-country-ping-map',
  templateUrl: './country-ping-map.component.html',
  styleUrls: ['./country-ping-map.component.css']
})
export class CountryPingMapComponent {
  protected week: string = "2022-W04";
  protected svgLink: string = "";
  protected scaleMode: string = "averageToAverage";
  protected requestedData: string = "average";
  constructor(private ipAddressInfoService: IpAddressInfoViewerService) {
  }

  ngOnInit(){
    this.ipAddressInfoService.GetLastProcessedTimeForCountryPingInfo().subscribe(
      value => {
        this.week = value?.response;
        this.changeWeek();
      }
    )
  }
  changeWeek(){
    console.log("change");
    this.svgLink = this.ipAddressInfoService.GetCountryPingInfoMapLink(this.week, this.scaleMode, this.requestedData);
  }
}
