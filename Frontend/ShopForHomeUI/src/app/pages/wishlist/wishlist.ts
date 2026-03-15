import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { WishlistService } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-wishlist',
  standalone: false,
  templateUrl: './wishlist.html',
  styleUrl: './wishlist.css'
})
export class Wishlist implements OnInit {
  wishlistItems: any[] = [];
  successMessage = '';
  errorMessage = '';

  constructor(
    private wishlistService: WishlistService,
    private cartService: CartService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadWishlistItems();
  }

  loadWishlistItems(): void {
    this.wishlistItems = this.wishlistService.getWishlistItems();
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

  removeItem(productId: number): void {
    this.wishlistService.removeFromWishlist(productId);
    this.loadWishlistItems();
  }

  clearWishlist(): void {
    this.wishlistService.clearWishlist();
    this.loadWishlistItems();
  }
 moveToCart(productId: number): void {
  const product = this.wishlistService.getItemById(productId);

  if (product) {
    this.cartService.addToCart(product);
    this.wishlistService.removeFromWishlist(productId);
    this.loadWishlistItems();
    this.showToast('success', product.name + ' moved to cart!');
  }
}
}