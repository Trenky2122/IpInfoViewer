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
  public radii: number[] = [0, 0, 0, 0, 0];
  public counts: number[] = [0, 0, 0, 0, 0];
  private osmAddress: string = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
  protected scaleMode: string = "averageToAverage";
  protected requestedData = "average";
  constructor(private service: IpAddressInfoViewerService) {
    this.week = "2022-W45"
  }

  async ngOnInit(){
    this.service.GetLastProcessedTimeForMapPoints().subscribe(
      value => {
        this.week = value?.response;
        this.setMapPointsForWeek();
      }
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
      new Leaflet.TileLayer(this.osmAddress,
        {attribution: '&copy; <a href="https://osm.org/copyright">OpenStreetMap</a> contributors', minZoom: 3, maxZoom: 6}),
      ...this.getMarkers(mapPoints, zoom),
      ...(this.zoom >= 6 ? this.getTooltips(mapPoints, zoom) : [])
    ] as Leaflet.Layer[]
  };

  getMarkers(mapPoints: MapIpAddressRepresentation[], zoom: number): Leaflet.CircleMarker[] {
    let upperBound = this.getUpperBound(mapPoints);
    return mapPoints?.map(point => {
      let color = this.pingToColor(this.getRequestedPingValuesForRequestedData(point, this.requestedData), upperBound);
      return new Leaflet.CircleMarker(new Leaflet.LatLng(point.latitude, point.longitude), {
        fillColor:  color,
        color: color,
        fillOpacity: 1,
        radius: this.ipCountToCircleRadius(point.ipAddressesCount, zoom),
        className: "mapPoint " + point.id,
        interactive: true
    })});
  }

  getTooltips(mapPoints: MapIpAddressRepresentation[], zoom: number): Leaflet.Tooltip[] {
    return mapPoints?.map(point => new Leaflet.Tooltip(new Leaflet.LatLng(point.latitude, point.longitude), {
      content: "<strong>lat</strong>: " + point.latitude.toFixed(4)
        + "<br><strong>long</strong>: " + point.longitude.toFixed(4)
        +"<br><strong>average</strong>: " + point.averagePingRtT
        +"<br><strong>minimum</strong>: " + point.minimumPingRtT
        +"<br><strong>maximum</strong>: " + point.maximumPingRtT
        +"<br><strong>total</strong>: " + point.ipAddressesCount,
      permanent: true,
      className: "mapPointLabel " + point.id,
      interactive: true,
      direction: "auto",
      sticky: true
    }));
  }

  setMapPointsForWeek(){
    this.service.GetMapPointsForWeek(this.week).subscribe(value => {
      this.currentWeekData = value;
      this.redrawLayers();
    })
  }

  setLegendValuesForCurrentZoom(){
    this.counts = [5, 50, 500, 5000, 50000];
    this.counts.forEach((c, i) => this.radii[i] = Math.round(this.ipCountToCircleRadius(c, this.zoom)));
  }

  pingToColor(ping: number, upperBound: number) {
    const lowerBound = 5;
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

  getUpperBound(data: MapIpAddressRepresentation[]):number{
    switch (this.scaleMode){
      case "constantMaximum":
        return 500;
      case "maximumToMaximum":
        return this.getMaximumForRequestedData(data);
      case "averageToAverage":
        return this.getAverageForRequestedData(data);
    }
    return 0;
  }

  getMaximumForRequestedData(data: MapIpAddressRepresentation[]): number{
    switch (this.requestedData){
      case "average":
        return Math.max(...data.map(x=>x.averagePingRtT));
      case "maximum":
        return Math.max(...data.map(x=>x.maximumPingRtT));
      case "minimum":
        return Math.max(...data.map(x=>x.minimumPingRtT));
    }
    return 0;
  }

  getAverageForRequestedData(data: MapIpAddressRepresentation[]): number{
    switch (this.requestedData){
      case "average":
        return this.avg(data.map(x=>x.averagePingRtT));
      case "maximum":
        return this.avg(data.map(x=>x.maximumPingRtT));
      case "minimum":
        return this.avg(data.map(x=>x.minimumPingRtT));
    }
    return 0;
  }

  zoomChanged(value: LeafletEvent){
    this.zoom = value.sourceTarget._zoom;
    this.redrawLayers();
  }

  avg(data: number[]):number{
    if(data.length === 0)
      return 0;
    let sum = 0;
    for(let i = 0; i<data.length; i++ ){
      sum += data[i];
    }
    return sum/data.length;
  }

  redrawLayers(){
    this.layers = this.getLayers(this.currentWeekData, this.zoom);
    this.setLegendValuesForCurrentZoom();
    this.legendLink = this.service.GetMapPointsLegendLink(this.counts, this.radii, 500);
  }

  ipCountToCircleRadius(ipCount: number, zoom: number): number{
    return 0.5 * Math.log(ipCount) * zoom;
  }

  getRequestedPingValuesForRequestedData(mapPoint: MapIpAddressRepresentation, requestedData: string): number{
    switch (requestedData){
      case "average":
        return mapPoint.averagePingRtT;
      case "minimum":
        return mapPoint.minimumPingRtT;
      case "maximum":
        return  mapPoint.maximumPingRtT;
    }
    return 0;
  }
}
