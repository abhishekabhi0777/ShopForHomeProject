import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Api } from './../../services/api';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-history',
  standalone: false,
  templateUrl: './order-history.html',
  styleUrl: './order-history.css'
})
export class OrderHistory implements OnInit {
  orders: any[] = [];
  isLoading: boolean = false;
  errorMessage: string = '';

  constructor(private api: Api,private cdr: ChangeDetectorRef,private router: Router) {}

  ngOnInit(): void {
  const role = localStorage.getItem('role');

  if (role !== 'User') {
    this.router.navigate(['/products']);
    return;
  }

  this.loadOrders();
}

  loadOrders(): void {
    const userId = Number(localStorage.getItem('userId'));
    console.log('UserId from localStorage:', userId);

    if (!userId) {
      this.errorMessage = 'User not logged in.';
      this.isLoading = false;
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.api.getOrdersByUser(userId).subscribe({
      next: (data: any[]) => {
        console.log('Orders API response:', data);
        this.orders = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load orders', err);
        this.errorMessage = 'Failed to load orders.';
        this.isLoading = false;
      }
    });
  }
}