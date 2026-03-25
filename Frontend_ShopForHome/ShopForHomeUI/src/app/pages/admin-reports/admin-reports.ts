import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { Api } from './../../services/api';

@Component({
  selector: 'app-admin-reports',
  standalone: false,
  templateUrl: './admin-reports.html',
  styleUrl: './admin-reports.css'
})
export class AdminReports implements OnInit {
  fromDate: string = '';
  toDate: string = '';

  salesData: any = null;
  lowStockProducts: any[] = [];
  isLoading = false;

  constructor(private api: Api,private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadSalesReport();
    this.loadLowStockProducts();
  }

  loadSalesReport(): void {
    this.isLoading = true;

    this.api.getSalesReport(this.fromDate, this.toDate).subscribe({
      next: (data) => {
        this.salesData = data;
        this.isLoading = false;
         this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Sales report error:', err);
        this.isLoading = false;
      }
    });
  }

  loadLowStockProducts(): void {
    this.api.getLowStockProducts().subscribe({
      next: (data) => {
        this.lowStockProducts = data;
         this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Low stock error:', err);
      }
    });
  }

  applyDateFilter(): void {
    this.loadSalesReport();
     this.cdr.detectChanges();
  }

  resetFilter(): void {
    this.fromDate = '';
    this.toDate = '';
    this.loadSalesReport();
     this.cdr.detectChanges();
  }

  isAdmin(): boolean {
    return localStorage.getItem('role') === 'Admin';
  }
}