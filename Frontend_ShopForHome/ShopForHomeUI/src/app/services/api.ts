import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Coupons } from '../pages/coupons/coupons';

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
uploadProductsCsv(file: File) {
  const formData = new FormData();
  formData.append('file', file);

  const token = localStorage.getItem('token');

  const headers = {
    Authorization: `Bearer ${token}`
  };

  return this.http.post(`${this.baseUrl}/Products/upload-csv`, formData, { headers });
}
searchProducts(keyword: string) {
  return this.http.get<any[]>(`${this.baseUrl}/Search?keyword=${keyword}`);
}

filterProducts(categoryId?: number, minPrice?: number, maxPrice?: number, minRating?: number) {
  let url = `${this.baseUrl}/Search/filter?`;

  if (categoryId) url += `categoryId=${categoryId}&`;
  if (minPrice !== null && minPrice !== undefined) url += `minPrice=${minPrice}&`;
  if (maxPrice !== null && maxPrice !== undefined) url += `maxPrice=${maxPrice}&`;
  if (minRating !== null && minRating !== undefined) url += `minRating=${minRating}&`;

  return this.http.get<any[]>(url);
}

getCategories() {
  return this.http.get<any[]>(`${this.baseUrl}/Categories`);
}
getSalesReport(fromDate?: string, toDate?: string) {
  let url = `${this.baseUrl}/Reports/sales?`;

  if (fromDate) url += `fromDate=${fromDate}&`;
  if (toDate) url += `toDate=${toDate}&`;

  return this.http.get<any>(url);
}

getLowStockProducts() {
  return this.http.get<any[]>(`${this.baseUrl}/Reports/low-stock` );
}
getCoupons() {
  return this.http.get<any[]>(`${this.baseUrl}/Coupons`);
}

createCoupon(data: any) {
  return this.http.post(`${this.baseUrl}/Coupons`, data);
}

assignCoupon(data: any) {
  return this.http.post(`${this.baseUrl}/Coupons/assign`, data);
}

getUserCoupons(userId: number) {
  return this.http.get<any[]>(`${this.baseUrl}/Coupons/user/${userId}`);
}
placeOrder(data: any) {
  return this.http.post(`${this.baseUrl}/Orders`, data);
}
getOrdersByUser(userId: number) {
  return this.http.get<any[]>(`${this.baseUrl}/Orders/user/${userId}`);
}
}