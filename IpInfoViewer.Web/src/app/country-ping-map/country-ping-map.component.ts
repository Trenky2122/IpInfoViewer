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
  constructor(private ipAddressInfoService: IpAddressInfoViewerService) {
  }

  ngOnInit(){
    this.changeWeek();
    this.ipAddressInfoService.GetLastProcessedTimeForMapPoints().subscribe(
      value => this.week = value
    )
  }
  changeWeek(){
    this.svgLink = this.ipAddressInfoService.GetCountryPingInfoMapLink(this.week);
  }
}
