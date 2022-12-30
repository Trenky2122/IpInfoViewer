import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import * as Leaflet from "leaflet";
import {IpAddressInfoViewerService} from "../ip-address-info-viewer.service";
import {IpAdress, MapIpAddressRepresentation} from "../models";

@Component({
  selector: 'app-ip-adresses-map',
  templateUrl: './ip-adresses-map.component.html',
  styleUrls: ['./ip-adresses-map.component.css']
})
export class IpAdressesMapComponent implements OnInit{

  constructor(private service: IpAddressInfoViewerService) {
  }

  async ngOnInit(){
    this.service.GetMapPoints().subscribe(value => {
      this.layers = this.getLayers(value);
    })
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

  getMarkers(mapPoints: MapIpAddressRepresentation[]): Leaflet.Marker[] {
    return mapPoints?.map(point => new Leaflet.Marker(new Leaflet.LatLng(point.latitude, point.longitude), {
      icon: new Leaflet.Icon({
        iconSize: [15 * Math.log(point.ipAddressesCount), 12* Math.log(point.ipAddressesCount)],
        iconAnchor: [13, 41],
        iconUrl: 'assets/place.webp',
      }),
      title: point.latitude +" " + point.longitude +" " + point.ipAddressesCount.toString()
    } as Leaflet.MarkerOptions)) as Leaflet.Marker[];
  }
}
