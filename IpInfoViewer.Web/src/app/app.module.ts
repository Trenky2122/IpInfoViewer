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
import { CountryPingMapComponent } from './country-ping-map/country-ping-map.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatSlideToggleModule} from "@angular/material/slide-toggle";
import {NgOptimizedImage} from "@angular/common";
import {SafePipeModule} from "safe-pipe";
import {MatSelectModule} from "@angular/material/select";

@NgModule({
  declarations: [
    AppComponent,
    IpAdressesMapComponent,
    NavMenuComponent,
    CountryPingMapComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    LeafletModule,
    FormsModule,
    NgxChartModule,
    AppRoutingModule,
    NgbModule,
    BrowserAnimationsModule,
    MatSlideToggleModule,
    NgOptimizedImage,
    SafePipeModule,
    MatSelectModule
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule { }
