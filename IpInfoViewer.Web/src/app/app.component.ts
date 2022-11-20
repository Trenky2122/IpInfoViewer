import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import * as Leaflet from 'leaflet';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(http: HttpClient) {
  }



  options: Leaflet.MapOptions = {
    layers: this.getLayers(),
    zoom: 1,
    center: new Leaflet.LatLng(0, 0)
  };

  getLayers(): Leaflet.Layer[]{
    let result = [
      new Leaflet.TileLayer('https://{s}.tile.osm.org/{z}/{x}/{y}.png',
        {attribution: '&copy; <a href="https://osm.org/copyright">OpenStreetMap</a> contributors'}),
      ...this.getMarkers()
    ] as Leaflet.Layer[];
    return result
  };

  getMarkers(): Leaflet.Marker[]{
    return [
      new Leaflet.Marker(new Leaflet.LatLng(43.5121264, 16.4700729), {
        icon: new Leaflet.Icon({
          iconSize: [50, 41],
          iconAnchor: [13, 41],
          iconUrl: 'assets/place.webp',
        }),
        title: 'Workspace'
      } as Leaflet.MarkerOptions),
      new Leaflet.Marker(new Leaflet.LatLng(43.5074826, 16.4390046), {
        icon: new Leaflet.Icon({
          iconSize: [50, 41],
          iconAnchor: [13, 41],
          iconUrl: 'assets/place.webp',
        }),
        title: 'Riva'
      } as Leaflet.MarkerOptions),
    ] as Leaflet.Marker[];
  };
  title = 'IpInfoViewer.Web';
}

