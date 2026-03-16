import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';

import { Header } from './components/header/header';
import { Footer } from './components/footer/footer';
import { Home } from './pages/home/home';
import { Products } from './pages/products/products';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Cart } from './pages/cart/cart';
import { Wishlist } from './pages/wishlist/wishlist';
import { HttpClientModule } from '@angular/common/http';
import { AddProduct } from './pages/add-product/add-product';
import { UserComponent } from './pages/users/users';


@NgModule({
  declarations: [
    App,
    Header,
    Footer,
    Home,
    Products,
    Login,
    Register,
    Cart,
    Wishlist,
    AddProduct,
    UserComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, FormsModule, HttpClientModule],
  providers: [],
  bootstrap: [App],
})
export class AppModule {}
