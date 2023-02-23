import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {IpAdressesMapComponent} from "./ip-adresses-map/ip-adresses-map.component";
import {CountryPingMapComponent} from "./country-ping-map/country-ping-map.component";

const routes: Routes = [
  { path: '', component: IpAdressesMapComponent },
  { path: 'countryPingMap', component: CountryPingMapComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
