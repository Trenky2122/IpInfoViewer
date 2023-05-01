import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {IpAdress, MapIpAddressRepresentation, StringResponse} from "./models";

@Injectable({
  providedIn: 'root'
})
export class IpAddressInfoViewerService {
  public baseUrl = "https://localhost:32770/";
  constructor(private http: HttpClient) { }

  public GetIpAddresses(): Observable<IpAdress[]>{
    return this.http.get<IpAdress[]>(this.baseUrl+"IpAddresses");
  }

  public GetMapPointsForWeek(week: string): Observable<MapIpAddressRepresentation[]>{
    return this.http.get<MapIpAddressRepresentation[]>(this.baseUrl + "MapPoints/ForWeek/" + week);
  }

  public GetCountryPingInfoMapLink(week: string, fullScale: boolean): string{
    return this.baseUrl + "CountryPingInfo/ColoredMap/" + week +"?fullScale="+fullScale;
  }

  public GetMapPointsLegendLink(counts: number[], radii: number[], pingUpperBound: number): string{
    let queryParams = new URLSearchParams({
      pingUpperBound: pingUpperBound.toString()
    });
    counts.forEach(c => queryParams.append("counts", c.toString()));
    radii.forEach(r => queryParams.append("radii", r.toString()));
    return this.baseUrl + "MapPoints/MapPointsMapLegend?" + queryParams.toString();
  }

  public GetLastProcessedTimeForMapPoints(): Observable<StringResponse>{
    return this.http.get<StringResponse>(this.baseUrl + "MapPoints/LastProcessedDate");
  }

  public GetLastProcessedTimeForCountryPingInfo(): Observable<StringResponse>{
    return this.http.get<StringResponse>(this.baseUrl + "CountryPingInfo/LastProcessedDate");
  }
}
