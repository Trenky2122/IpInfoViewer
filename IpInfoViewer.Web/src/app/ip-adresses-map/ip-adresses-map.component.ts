import {Component, EventEmitter, Input, OnInit, Output, Pipe} from '@angular/core';
import * as Leaflet from "leaflet";
import {IpAddressInfoViewerService} from "../ip-address-info-viewer.service";
import {IpAdress, MapIpAddressRepresentation} from "../models";

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
      radius: 3 * Math.log(point.ipAddressesCount)
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
    let perc = Math.log(ping)/0.093;
    let r, g, b = 0;
    if(perc < 50) {
      r = 255;
      g = Math.round(5.1 * perc);
    }
    else {
      g = 255;
      r = Math.round(510 - 5.10 * perc);
    }
    let h = r * 0x10000 + g * 0x100 + b * 0x1;
    let result = '#' + ('000000' + h.toString(16)).slice(-6);
    console.log(ping, Math.log(ping), perc, result);
    return result;
  }
}
