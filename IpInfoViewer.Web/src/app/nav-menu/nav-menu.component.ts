import { Component } from '@angular/core';
import {IpAddressInfoViewerService} from "../ip-address-info-viewer.service";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  swaggerUrl: string;
  constructor(service: IpAddressInfoViewerService) {
    this.swaggerUrl = service.baseUrl+'swagger';
  }
}
