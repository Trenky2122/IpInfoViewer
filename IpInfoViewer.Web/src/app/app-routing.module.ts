import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {IpAdressesMapComponent} from "./ip-adresses-map/ip-adresses-map.component";

const routes: Routes = [
  { path: '', component: IpAdressesMapComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
