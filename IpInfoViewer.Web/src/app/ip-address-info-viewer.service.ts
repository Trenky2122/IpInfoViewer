import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {IpAdress, MapIpAddressRepresentation, StringResponse} from "./models";

@Injectable({
  providedIn: 'root'
})
export class IpAddressInfoViewerService {
  public baseUrl = "https://localhost:32768/";
  constructor(private http: HttpClient) { }

  public GetIpAddresses(): Observable<IpAdress[]>{
    return this.http.get<IpAdress[]>(this.baseUrl+"IpAddresses");
  }

  public GetMapPointsForWeek(week: string): Observable<MapIpAddressRepresentation[]>{
    return this.http.get<MapIpAddressRepresentation[]>(this.baseUrl + "Map/ForWeek/" + week);
  }

  public GetCountryPingInfoMapLink(week: string, fullScale: boolean): string{
    return this.baseUrl + "map/ColoredMap/" + week +"?fullScale="+fullScale;
  }

  public GetLastProcessedTimeForMapPoints(): Observable<StringResponse>{
    return this.http.get<StringResponse>(this.baseUrl + "map/lastProcessedDate/ipInfo");
  }

  public GetLastProcessedTimeForCountryPingInfo(): Observable<StringResponse>{
    return this.http.get<StringResponse>(this.baseUrl + "map/lastProcessedDate/countryPing");
  }
}
