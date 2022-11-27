import {Component, EventEmitter, Input, Output} from '@angular/core';
import * as Leaflet from "leaflet";
import {IpAddressInfoViewerService} from "../ip-address-info-viewer.service";
import {IpAdress} from "../models";

@Component({
  selector: 'app-ip-adresses-map',
  templateUrl: './ip-adresses-map.component.html',
  styleUrls: ['./ip-adresses-map.component.css']
})
export class IpAdressesMapComponent {

  constructor(private service: IpAddressInfoViewerService) {
  }

  async ngOnInit(){
    this.service.GetIpAddresses().subscribe(value => {
      this.options = {
        layers: this.getLayers(value),
        zoom: 1,
        center: new Leaflet.LatLng(0, 0)
      };
      this.layers = this.getLayers(value);
      console.log(this.options);
    })
  }

  options: Leaflet.MapOptions={
    layers: this.getLayers([]),
    zoom: 1,
    center: new Leaflet.LatLng(0, 0)
  };
  layers: Leaflet.Layer[] = [];

  getLayers(addresses: IpAdress[]): Leaflet.Layer[]{
    let result = [
      new Leaflet.TileLayer('https://{s}.tile.osm.org/{z}/{x}/{y}.png',
        {attribution: '&copy; <a href="https://osm.org/copyright">OpenStreetMap</a> contributors'}),
      ...this.getMarkers(addresses)
    ] as Leaflet.Layer[];
    return result
  };

  getMarkers(addresses: IpAdress[]): Leaflet.Marker[]{
    return addresses?.map(address => new Leaflet.Marker(new Leaflet.LatLng(address.latitude, address.longitude), {
      icon: new Leaflet.Icon({
        iconSize: [50, 41],
        iconAnchor: [13, 41],
        iconUrl: 'assets/place.webp',
      }),
      title: address.ipValue
    } as Leaflet.MarkerOptions)) as Leaflet.Marker[];
    // return [
    //
    //   new Leaflet.Marker(new Leaflet.LatLng(43.5121264, 16.4700729), {
    //     icon: new Leaflet.Icon({
    //       iconSize: [50, 41],
    //       iconAnchor: [13, 41],
    //       iconUrl: 'assets/place.webp',
    //     }),
    //     title: 'Workspace'
    //   } as Leaflet.MarkerOptions),
    //   new Leaflet.Marker(new Leaflet.LatLng(43.5074826, 16.4390046), {
    //     icon: new Leaflet.Icon({
    //       iconSize: [50, 41],
    //       iconAnchor: [13, 41],
    //       iconUrl: 'assets/place.webp',
    //     }),
    //     title: 'Riva'
    //   } as Leaflet.MarkerOptions),
    // ] as Leaflet.Marker[];
  };
}
