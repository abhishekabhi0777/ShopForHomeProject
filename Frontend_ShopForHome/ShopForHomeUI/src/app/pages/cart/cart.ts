import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { CartService } from './../../services/cart.service';
import { Api } from './../../services/api';
import { Router } from '@angular/router'; 

@Component({
  selector: 'app-cart',
  standalone: false,
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class Cart implements OnInit {
  cartItems: any[] = [];

  totalAmount: number = 0;
  discountAmount: number = 0;
  finalAmount: number = 0;
  successMessage  : string ='';

  userCoupons: any[] = [];
  selectedCouponId: number | null = null;

  constructor(
    private cartService: CartService,
    private api: Api,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCartItems();

    const userId = Number(localStorage.getItem('userId'));
    if (userId) {
      this.loadUserCoupons(userId);
    }
  }

  loadCartItems(): void {
    this.cartItems = this.cartService.getCartItems();
    this.calculateTotals();
  }

  loadUserCoupons(userId: number): void {
    this.api.getUserCoupons(userId).subscribe({
      next: (data: any[]) => {
        this.userCoupons = data.filter(c => !c.isUsed);
      },
      error: (err) => {
        console.error('Failed to load user coupons', err);
      }
    });
  }

  calculateTotals(): void {
    this.totalAmount = this.cartItems.reduce(
      (sum, item) => sum + (item.price * item.quantity),
      0
    );

    this.discountAmount = 0;

    if (this.selectedCouponId) {
      const selectedCoupon = this.userCoupons.find(
        c => c.couponId === this.selectedCouponId
      );

      if (selectedCoupon && this.totalAmount >= selectedCoupon.minimumAmount) {
        this.discountAmount =
          this.totalAmount * (selectedCoupon.discountPercent / 100);
      }
    }

    this.finalAmount = this.totalAmount - this.discountAmount;
  }

  onCouponChange(): void {
    this.calculateTotals();
  }

  removeItem(productId: number): void {
    this.cartService.removeFromCart(productId);
    this.loadCartItems();
  }

  clearCart(): void {
    this.cartService.clearCart();
    this.cartItems = [];
    this.selectedCouponId = null;
    this.totalAmount = 0;
    this.discountAmount = 0;
    this.finalAmount = 0;
  }

  increaseQuantity(productId: number): void {
    this.cartService.increaseQuantity(productId);
    this.loadCartItems();
  }

  decreaseQuantity(productId: number): void {
    this.cartService.decreaseQuantity(productId);
    this.loadCartItems();
  }

  placeOrder(): void {
    const userId = Number(localStorage.getItem('userId'));

    if (!userId) {
      alert('Please login first');
      return;
    }

    if (this.cartItems.length === 0) {
      alert('Cart is empty');
      return;
    }

    const orderData = {
      userId: userId,
      couponId: this.selectedCouponId,
      items: this.cartItems.map(item => ({
        productId: item.productId,
        quantity: item.quantity,
        unitPrice: item.price
      }))
    };

    this.api.placeOrder(orderData).subscribe({
      next: (res: any) => {
  this.cartService.clearCart();
  this.cartItems = [];
  this.selectedCouponId = null;
  this.userCoupons = [];
  this.totalAmount = 0;
  this.discountAmount = 0;
  this.finalAmount = 0;

  this.loadCartItems();
  this.cdr.detectChanges();

  setTimeout(() => {
    alert(`Order placed successfully! Final Amount: ₹${res.finalAmount}`);
      setTimeout(() => {
        this.router.navigate(['/products']);
      }, 1500);
  }, 0);
},
      error: (err) => {
        console.error(err);
        alert(err?.error?.message || 'Order failed');
      }
    });
  }
}