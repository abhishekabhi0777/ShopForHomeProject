import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Api {
  private baseUrl = 'https://localhost:7025/api';

  constructor(private http: HttpClient) {}

  getProducts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/products`);
  }
  register(data: any): Observable<any> {
  return this.http.post<any>(`${this.baseUrl}/auth/register`, data);
}
  login(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/auth/login`, data);
  }
  deleteProduct(id: number) {

  const token = localStorage.getItem('token');

  const headers = {
    Authorization: `Bearer ${token}`
  };

  return this.http.delete(`${this.baseUrl}/products/${id}`, { headers });

}
addProduct(data: any) {
  const token = localStorage.getItem('token');

  const headers = {
    Authorization: `Bearer ${token}`
  };

  return this.http.post(`${this.baseUrl}/products`, data, { headers });
}
updateProduct(id: number, data: any) {
  const token = localStorage.getItem('token');

  const headers = {
    Authorization: `Bearer ${token}`
  };

  return this.http.put(`${this.baseUrl}/products/${id}`, data, { headers });
}
}