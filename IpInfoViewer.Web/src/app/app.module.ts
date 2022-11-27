import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';
import { IpAdressesMapComponent } from './ip-adresses-map/ip-adresses-map.component';

@NgModule({
  declarations: [
    AppComponent,
    IpAdressesMapComponent
  ],
  imports: [
    BrowserModule, HttpClientModule, LeafletModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
