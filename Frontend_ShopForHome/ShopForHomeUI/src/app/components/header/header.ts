import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header implements OnInit {
  isLoggedIn = false;
  fullName = '';
  role = '';
  cartCount: number = 0;
  wishlistCount: number =0;

  constructor(
    private router: Router,
    private cartService: CartService,
    private wishlistService: WishlistService
  ) {}

  ngOnInit(): void {
    this.loadUserData();

    this.cartService.cartCounts.subscribe(count => {
      this.cartCount = count;
    });
    this.wishlistService.wishlistCount$.subscribe(count => {
  this.wishlistCount = count;
});
  }

  loadUserData(): void {
    const token = localStorage.getItem('token');
    const fullName = localStorage.getItem('fullName');
    const role = localStorage.getItem('role');

    this.isLoggedIn = !!token;
    this.fullName = fullName ? fullName : '';
    this.role = role ? role : '';
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('fullName');
    localStorage.removeItem('email');
    localStorage.removeItem('role');

    this.isLoggedIn = false;
    this.fullName = '';
    this.role = '';

    this.router.navigate(['/login']);
  }
}