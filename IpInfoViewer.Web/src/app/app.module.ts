import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { IpAdressesMapComponent } from './ip-adresses-map/ip-adresses-map.component';
import {FormsModule} from "@angular/forms";
import {NgxChartModule} from "ngx-chart";
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { AppRoutingModule } from './app-routing.module';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";

@NgModule({
  declarations: [
    AppComponent,
    IpAdressesMapComponent,
    NavMenuComponent
  ],
    imports: [
      BrowserModule,
      HttpClientModule,
      LeafletModule,
      FormsModule,
      NgxChartModule,
      AppRoutingModule,
      NgbModule
    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
