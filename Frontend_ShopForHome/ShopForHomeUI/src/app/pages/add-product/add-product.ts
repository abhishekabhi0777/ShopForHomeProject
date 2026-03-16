import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Api } from './../../services/api';

@Component({
  selector: 'app-add-product',
  standalone: false,
  templateUrl: './add-product.html',
  styleUrl: './add-product.css'
})
export class AddProduct implements OnInit {

  product: any = {
    productId: 0,
    name: '',
    description: '',
    price: 0,
    rating: 0,
    stock: 0,
    imageUrl: '',
    categoryId: 0
  };

  successMessage = '';
  errorMessage = '';
  isEditMode = false;

  constructor(
    private api: Api,
    private router: Router
  ) {}

  ngOnInit(): void {
    const editData = localStorage.getItem('editProduct');

    if (editData) {
      this.product = JSON.parse(editData);
      this.isEditMode = true;
    }
  }

  saveProduct(): void {
    if (this.isEditMode) {
      this.api.updateProduct(this.product.productId, this.product).subscribe({
        next: () => {
          this.successMessage = 'Product updated successfully!';
          this.errorMessage = '';

          localStorage.removeItem('editProduct');

          setTimeout(() => {
            this.router.navigate(['/products']);
          }, 1000);
        },
        error: () => {
          this.errorMessage = 'Failed to update product';
          this.successMessage = '';
        }
      });
    } else {
      this.api.addProduct(this.product).subscribe({
        next: () => {
          this.successMessage = 'Product added successfully!';
          this.errorMessage = '';

          setTimeout(() => {
            this.router.navigate(['/products']);
          }, 1000);
        },
        error: () => {
          this.errorMessage = 'Failed to add product';
          this.successMessage = '';
        }
      });
    }
  }
}