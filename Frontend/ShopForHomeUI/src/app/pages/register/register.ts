import { Component } from '@angular/core';
import { Api } from '../../services/api';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  registerData = {
    fullName: '',
    email: '',
    password: '',
    phone: '',
    address: '',
    role: 'User'
  };

  successMessage = '';
  errorMessage = '';

  constructor(private api: Api, private router: Router) {}

  registerUser(): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.api.register(this.registerData).subscribe({
      next: (res) => {
        this.successMessage = 'Registration successful. Please login.';
        this.errorMessage = '';

        this.registerData = {
          fullName: '',
          email: '',
          password: '',
          phone: '',
          address: '',
          role: 'User'
        };

        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1500);
      },
      error: (err) => {
        console.error('Register error:', err);
        this.successMessage = '';
        this.errorMessage = err?.error?.message || 'Registration failed.';
      }
    });
  }
}