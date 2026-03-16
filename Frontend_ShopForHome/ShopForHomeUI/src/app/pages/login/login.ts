import { Component } from '@angular/core';
import { Api } from '../../services/api';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  loginData = {
    email: '',
    password: ''
  };

  successMessage = '';
  errorMessage = '';

  constructor(private api: Api, private router: Router) {}

  loginUser(): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.api.login(this.loginData).subscribe({
      next: (res) => {
        console.log('Login Success:', res);

        localStorage.setItem('token', res.token);
        localStorage.setItem('userId', res.userId);
        localStorage.setItem('fullName', res.fullName);
        localStorage.setItem('email', res.email);
        localStorage.setItem('role', res.role);
        localStorage.setItem('userId', res.userId.toString());

        this.successMessage = 'Login successful';
        this.errorMessage = '';

       setTimeout(() => {
  if (res.role === 'Admin') {
    this.router.navigate(['/products']).then(() => {
      window.location.reload();
    });
  } else {
    this.router.navigate(['/']).then(() => {
      window.location.reload();
    });
  }
}, 1500);
      },
      error: (err) => {
        console.error('Login error:', err);
        this.successMessage = '';
        this.errorMessage = 'Invalid email or password';
      }
    });
  }
}