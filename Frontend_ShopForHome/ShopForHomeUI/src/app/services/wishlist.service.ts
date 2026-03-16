import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WishlistService {

  private wishlistCountSubject = new BehaviorSubject<number>(this.getWishlistCount());
  wishlistCount$ = this.wishlistCountSubject.asObservable();

  constructor() {}

  private getWishlistKey(): string {
    const userId = localStorage.getItem('userId');
    return userId ? `wishlist_${userId}` : 'wishlist_guest';
  }

  getWishlistItems(): any[] {
    const items = localStorage.getItem(this.getWishlistKey());
    return items ? JSON.parse(items) : [];
  }
  getItemById(productId: number): any {
  const wishlist = this.getWishlistItems();
  return wishlist.find(item => item.productId === productId);
}

  addToWishlist(product: any): void {
    let wishlist = this.getWishlistItems();

    const exists = wishlist.find(item => item.productId === product.productId);

    if (!exists) {
      wishlist.push(product);
    }

    localStorage.setItem(this.getWishlistKey(), JSON.stringify(wishlist));
    this.updateWishlistCount();
  }

  removeFromWishlist(productId: number): void {
    const wishlist = this.getWishlistItems().filter(item => item.productId !== productId);
    localStorage.setItem(this.getWishlistKey(), JSON.stringify(wishlist));
    this.updateWishlistCount();
  }

  clearWishlist(): void {
    localStorage.removeItem(this.getWishlistKey());
    this.updateWishlistCount();
  }

  getWishlistCount(): number {
    return this.getWishlistItems().length;
  }

  private updateWishlistCount(): void {
    this.wishlistCountSubject.next(this.getWishlistCount());
  }
}