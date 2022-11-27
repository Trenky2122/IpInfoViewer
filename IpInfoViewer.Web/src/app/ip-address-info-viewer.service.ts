import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {IpAdress} from "./models";

@Injectable({
  providedIn: 'root'
})
export class IpAddressInfoViewerService {
  private baseUrl = "https://localhost:7024/";
  constructor(private http: HttpClient) { }

  public GetIpAddresses(): Observable<IpAdress[]>{
    return this.http.get<IpAdress[]>(this.baseUrl+"IpAddresses");
  }
}
