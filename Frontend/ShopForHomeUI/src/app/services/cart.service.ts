import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  private cartCountSubject = new BehaviorSubject<number>(this.getCartCount());
  cartCounts = this.cartCountSubject.asObservable();

  constructor() {}

  private getCartKey(): string {
    const userId = localStorage.getItem('userId');
    return userId ? `cart_${userId}` : 'cart_guest';
  }

  getCartItems(): any[] {
    const items = localStorage.getItem(this.getCartKey());
    return items ? JSON.parse(items) : [];
  }

  addToCart(product: any): void {
    let cart = this.getCartItems();

    const existing = cart.find(item => item.productId === product.productId);

    if (existing) {
      existing.quantity += 1;
    } else {
      cart.push({ ...product, quantity: 1 });
    }

    localStorage.setItem(this.getCartKey(), JSON.stringify(cart));
    this.updateCartCount();
  }

  removeFromCart(productId: number): void {
    const cart = this.getCartItems().filter(item => item.productId !== productId);
    localStorage.setItem(this.getCartKey(), JSON.stringify(cart));
    this.updateCartCount();
  }

  increaseQuantity(productId: number): void {
    const cart = this.getCartItems();
    const item = cart.find((p: any) => p.productId === productId);

    if (item) {
      item.quantity += 1;
      localStorage.setItem(this.getCartKey(), JSON.stringify(cart));
      this.updateCartCount();
    }
  }

  decreaseQuantity(productId: number): void {
    let cart = this.getCartItems();
    const item = cart.find((p: any) => p.productId === productId);

    if (item) {
      item.quantity -= 1;

      if (item.quantity <= 0) {
        cart = cart.filter((p: any) => p.productId !== productId);
      }

      localStorage.setItem(this.getCartKey(), JSON.stringify(cart));
      this.updateCartCount();
    }
  }

  clearCart(): void {
    localStorage.removeItem(this.getCartKey());
    this.updateCartCount();
  }

  getCartCount(): number {
    const cart = this.getCartItems();
    return cart.reduce((count, item) => count + item.quantity, 0);
  }

  private updateCartCount(): void {
    this.cartCountSubject.next(this.getCartCount());
  }
}