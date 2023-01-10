import {Component, EventEmitter, Input, OnInit, Output, Pipe} from '@angular/core';
import * as Leaflet from "leaflet";
import {IpAddressInfoViewerService} from "../ip-address-info-viewer.service";
import {IpAdress, MapIpAddressRepresentation} from "../models";
import {point} from "leaflet";

@Component({
  selector: 'app-ip-adresses-map',
  templateUrl: './ip-adresses-map.component.html',
  styleUrls: ['./ip-adresses-map.component.css']
})
export class IpAdressesMapComponent implements OnInit{
  public week: string;
  constructor(private service: IpAddressInfoViewerService) {
    this.week = "2022-W45"
  }

  async ngOnInit(){
    this.setMapPointsForWeek();
  }

  options: Leaflet.MapOptions={
    layers: this.getLayers([]),
    zoom: 1,
    center: new Leaflet.LatLng(0, 0)
  };
  layers: Leaflet.Layer[] = [];

  getLayers(mapPoints: MapIpAddressRepresentation[]): Leaflet.Layer[]{
    let result = [
      new Leaflet.TileLayer('https://{s}.tile.osm.org/{z}/{x}/{y}.png',
        {attribution: '&copy; <a href="https://osm.org/copyright">OpenStreetMap</a> contributors', minZoom: 3}),
      ...this.getMarkers(mapPoints)
    ] as Leaflet.Layer[];
    return result
  };

  getMarkers(mapPoints: MapIpAddressRepresentation[]): Leaflet.CircleMarker[] {
    return mapPoints?.map(point => new Leaflet.CircleMarker(new Leaflet.LatLng(point.latitude, point.longitude), {
      fillColor:  this.pingToColor(point.averagePingRtT),
      color:  this.pingToColor(point.averagePingRtT),
      fillOpacity: 1,
      radius: 2.5 * Math.log(point.ipAddressesCount),

    }));
  }

  setMapPointsForWeek(){
    this.service.GetMapPointsForWeek(this.week).subscribe(value => {
      this.layers = this.getLayers(value);
      console.log(value.length);
    })
    console.log("done")
  }

  pingToColor(ping: number) {
    let upperBound = 500;
    let lowerBound = 20;
    let pingInBounds = ping;
    if(pingInBounds < lowerBound)
      pingInBounds = lowerBound;
    if(pingInBounds > upperBound)
      pingInBounds = upperBound;
    let percent = (pingInBounds - lowerBound)/(upperBound - lowerBound);
    let r = Math.round(255*percent), g = Math.round((1-percent)*255), b = 0;
    let h = r * 0x10000 + g * 0x100;
    let result = '#' + ('000000' + h.toString(16)).slice(-6);
    console.log(percent, ping, result)
    return result;
  }
}
