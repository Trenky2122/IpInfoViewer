import {Component, OnInit} from '@angular/core';
import * as Leaflet from "leaflet";
import {circle, LeafletEvent, polygon, tileLayer} from "leaflet";
import {IpAddressInfoViewerService} from "../ip-address-info-viewer.service";
import {MapIpAddressRepresentation} from "../models";

@Component({
  selector: 'app-ip-adresses-map',
  templateUrl: './ip-adresses-map.component.html',
  styleUrls: ['./ip-adresses-map.component.css']
})
export class IpAdressesMapComponent implements OnInit{
  public week: string;
  public zoom: number = 3;
  public legendLink: string="";
  public currentWeekData: MapIpAddressRepresentation[] = [];
  constructor(private service: IpAddressInfoViewerService) {
    this.week = "2022-W45"
  }

  async ngOnInit(){
    this.setMapPointsForWeek();
    this.service.GetLastProcessedTimeForMapPoints().subscribe(
      value => this.week = value?.response
    )
  }

  options: Leaflet.MapOptions={
    layers: this.getLayers([], 3),
    zoom: 3,
    center: new Leaflet.LatLng(0, 0)
  };
  layers: Leaflet.Layer[] = [];

  getLayers(mapPoints: MapIpAddressRepresentation[], zoom: number): Leaflet.Layer[]{
    return [
      new Leaflet.TileLayer('https://{s}.tile.osm.org/{z}/{x}/{y}.png',
        {attribution: '&copy; <a href="https://osm.org/copyright">OpenStreetMap</a> contributors', minZoom: 3}),
      ...this.getMarkers(mapPoints, zoom)
    ] as Leaflet.Layer[]
  };

  getMarkers(mapPoints: MapIpAddressRepresentation[], zoom: number): Leaflet.CircleMarker[] {
    return mapPoints?.map(point => new Leaflet.CircleMarker(new Leaflet.LatLng(point.latitude, point.longitude), {
      fillColor:  this.pingToColor(point.averagePingRtT),
      color:  this.pingToColor(point.averagePingRtT),
      fillOpacity: 1,
      radius: this.ipCountToCircleRadius(point.ipAddressesCount, zoom),
    }));
  }

  setMapPointsForWeek(){
    this.service.GetMapPointsForWeek(this.week).subscribe(value => {
      this.currentWeekData = value;
      this.layers = this.getLayers(value, this.zoom);
    })
    this.legendLink = this.service.GetCountryPingInfoMapLink(this.week, true);
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
    return '#' + ('000000' + h.toString(16)).slice(-6);
  }

  zoomChanged(value: LeafletEvent){
    this.zoom = value.sourceTarget._zoom;
    this.layers = this.getLayers(this.currentWeekData, this.zoom);
  }

  ipCountToCircleRadius(ipCount: number, zoom: number): number{
    return 0.5 * Math.log(ipCount) * zoom;
  }
}
