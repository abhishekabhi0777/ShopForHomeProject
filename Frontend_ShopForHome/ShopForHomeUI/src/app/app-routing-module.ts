import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { Home } from './pages/home/home';
import { Products } from './pages/products/products';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Cart } from './pages/cart/cart';
import { Wishlist } from './pages/wishlist/wishlist';
import { AddProduct } from './pages/add-product/add-product';
import { UserComponent } from './pages/users/users';
import { AdminReports } from './pages/admin-reports/admin-reports';
import { Coupons } from './pages/coupons/coupons';
import { OrderHistory } from './pages/order-history/order-history';


const routes: Routes = [
  { path: '', component: Home },
  { path: 'home', component: Home },
  { path: 'products', component: Products },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'cart', component: Cart },
  { path: 'wishlist', component: Wishlist },
  { path: 'add-product', component: AddProduct },
  {path:'users', component: UserComponent},
  {path:'admin-reports', component: AdminReports},
  {path:'coupons', component: Coupons},
  {path:'order-history', component: OrderHistory},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }