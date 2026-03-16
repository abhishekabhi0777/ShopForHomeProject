import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { AddProduct } from './add-product';

describe('AddProduct', () => {
  let component: AddProduct;
  let fixture: ComponentFixture<AddProduct>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddProduct],
      imports: [FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(AddProduct);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});