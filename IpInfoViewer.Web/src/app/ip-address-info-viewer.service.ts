import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";
import {IpAdress, MapIpAddressRepresentation, SvgWrapper} from "./models";

@Injectable({
  providedIn: 'root'
})
export class IpAddressInfoViewerService {
  private baseUrl = "https://localhost:32768/";
  constructor(private http: HttpClient) { }

  public GetIpAddresses(): Observable<IpAdress[]>{
    return this.http.get<IpAdress[]>(this.baseUrl+"IpAddresses");
  }

  public GetMapPointsForWeek(week: string): Observable<MapIpAddressRepresentation[]>{
    return this.http.get<MapIpAddressRepresentation[]>(this.baseUrl + "Map/ForWeek/" + week);
  }

  public GetCountryPingInfoMapLink(week: string): string{
    return this.baseUrl + "Map/ColoredMap/" + week;
  }
}
