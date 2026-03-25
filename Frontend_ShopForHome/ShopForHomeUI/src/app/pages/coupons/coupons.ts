import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Api } from './../../services/api';

@Component({
  selector: 'app-coupons',
  standalone: false,
  templateUrl: './coupons.html',
  styleUrl: './coupons.css'
})
export class Coupons implements OnInit {
  coupons: any[] = [];
  userCoupons: any[] = [];

  couponForm = {
    code: '',
    discountPercent: 0,
    expiryDate: '',
    minimumAmount: 0
  };

  assignForm = {
    userId: 0,
    couponId: 0
  };

  message = '';
  error = '';

  constructor(private api: Api, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.refreshCouponsData();
  }

  isAdmin(): boolean {
    return localStorage.getItem('role') === 'Admin';
  }

  refreshCouponsData(): void {
    this.loadCoupons();

    const userId = Number(localStorage.getItem('userId'));
    if (userId) {
      this.loadUserCoupons(userId);
    }

    this.cdr.detectChanges();
  }

  loadCoupons(): void {
    this.api.getCoupons().subscribe({
      next: (data) => {
        this.coupons = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  loadUserCoupons(userId: number): void {
    this.api.getUserCoupons(userId).subscribe({
      next: (data) => {
        this.userCoupons = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  createCoupon(): void {
    this.message = '';
    this.error = '';

    this.api.createCoupon(this.couponForm).subscribe({
      next: () => {
        this.message = 'Coupon created successfully';
        this.couponForm = {
          code: '',
          discountPercent: 0,
          expiryDate: '',
          minimumAmount: 0
        };

        this.refreshCouponsData();
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to create coupon';
      }
    });
  }

  assignCoupon(): void {
    this.message = '';
    this.error = '';

    this.api.assignCoupon(this.assignForm).subscribe({
      next: () => {
        this.message = 'Coupon assigned successfully';
        this.assignForm = {
          userId: 0,
          couponId: 0
        };

        this.refreshCouponsData();
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to assign coupon';
      }
    });
  }
}