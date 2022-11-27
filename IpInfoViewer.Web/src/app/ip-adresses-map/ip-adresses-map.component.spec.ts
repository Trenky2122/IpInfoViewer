import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IpAdressesMapComponent } from './ip-adresses-map.component';

describe('IpAdressesMapComponent', () => {
  let component: IpAdressesMapComponent;
  let fixture: ComponentFixture<IpAdressesMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IpAdressesMapComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IpAdressesMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
