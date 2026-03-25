import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Coupons } from './coupons';

describe('Coupons', () => {
  let component: Coupons;
  let fixture: ComponentFixture<Coupons>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Coupons],
    }).compileComponents();

    fixture = TestBed.createComponent(Coupons);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
