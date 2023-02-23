import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountryPingMapComponent } from './country-ping-map.component';

describe('CountryPingMapComponent', () => {
  let component: CountryPingMapComponent;
  let fixture: ComponentFixture<CountryPingMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CountryPingMapComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CountryPingMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
