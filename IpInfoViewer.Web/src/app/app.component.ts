import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import * as Leaflet from 'leaflet';
import {BarChartOption, ChartData, ChartView} from 'ngx-chart';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(http: HttpClient) {
  }

  title = 'IpInfoViewer.Web';
  chartData: ChartData[] = [
    { name: "India", value: 132, color: "#61b15a" },
    { name: "Nepal", value: 772, color: "#adce74" },
    { name: "USA", value: 142, color: "#fff76a" },
    { name: "UK", value: 112, color: "#ffce89" },
    { name: "Brazil", value: 162, color: "#d8f8b7" }
  ];
  chartOptions: BarChartOption = {
    roundedCorners: false,
    showLegend: true,
    legendTitle: 'Total',
    isHorizontal: false
  }
  barView: ChartView = {
    height: 400,
    width: 400
  }
}

