import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { Api } from './../../services/api';
import { CartService } from './../../services/cart.service';
import { WishlistService } from './../../services/wishlist.service';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: any[] = [];
  isLoading: boolean = true;
  successMessage = '';
  errorMessage = '';

  constructor(
    private api: Api,
    private cdr: ChangeDetectorRef,
    private CartService: CartService,
    private WishlistService: WishlistService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  isAdmin(): boolean {
    return localStorage.getItem('role') === 'Admin';
  }

  loadProducts(): void {
    this.api.getProducts().subscribe({
      next: (data) => {
        this.products = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('API error:', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteProduct(productId: number): void {
    if (!confirm('Are you sure you want to delete this product?')) {
      return;
    }

    this.api.deleteProduct(productId).subscribe({
      next: () => {
        this.CartService.removeFromCart(productId);
        this.WishlistService.removeFromWishlist(productId);
        this.showToast('success', 'Product deleted successfully!');
        this.loadProducts();
      },
      error: () => {
        this.showToast('error', 'Failed to delete product');
      }
    });
  }

  showToast(type: 'success' | 'error', message: string): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (type === 'success') {
      this.successMessage = message;
    } else {
      this.errorMessage = message;
    }

    this.cdr.detectChanges();

    setTimeout(() => {
      this.successMessage = '';
      this.errorMessage = '';
      this.cdr.detectChanges();
    }, 2000);
  }

  addToCart(product: any): void {
    this.CartService.addToCart(product);
    this.showToast('success', product.name + ' added to cart!');
  }

  addToWishlist(product: any): void {
  this.WishlistService.addToWishlist(product);
  this.showToast('success', product.name + ' added to wishlist!');
}

  isInWishlist(productId: number): boolean {
    return this.WishlistService.getWishlistItems().some(
      item => item.productId === productId
    );
  }

  editProduct(product: any): void {
    localStorage.setItem('editProduct', JSON.stringify(product));
    this.router.navigate(['/add-product']);
  }
}