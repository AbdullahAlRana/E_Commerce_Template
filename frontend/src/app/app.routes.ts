import { Routes } from '@angular/router';
import { authGuard, adminGuard } from './core/guards/auth.guard';
import { HomeComponent } from './pages/home/home.component';
import { ProductsComponent } from './pages/products/products.component';
import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { CartComponent } from './pages/cart/cart.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { OrdersComponent } from './pages/orders/orders.component';
import { OrderDetailComponent } from './pages/order-detail/order-detail.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { AddressesComponent } from './pages/addresses/addresses.component';
import { WishlistComponent } from './pages/wishlist/wishlist.component';
import { AdminComponent } from './pages/admin/admin.component';
import { AdminDashboardComponent } from './pages/admin/dashboard/admin-dashboard.component';
import { AdminProductsComponent } from './pages/admin/products/admin-products.component';
import { AdminCategoriesComponent } from './pages/admin/categories/admin-categories.component';
import { AdminBrandsComponent } from './pages/admin/brands/admin-brands.component';
import { AdminOrdersComponent } from './pages/admin/orders/admin-orders.component';
import { AdminCouponsComponent } from './pages/admin/coupons/admin-coupons.component';
import { AdminUsersComponent } from './pages/admin/users/admin-users.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'products', component: ProductsComponent },
  { path: 'products/:id', component: ProductDetailComponent },
  { path: 'cart', component: CartComponent },
  { path: 'checkout', component: CheckoutComponent, canActivate: [authGuard] },
  { path: 'orders', component: OrdersComponent, canActivate: [authGuard] },
  { path: 'orders/:id', component: OrderDetailComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
  { path: 'addresses', component: AddressesComponent, canActivate: [authGuard] },
  { path: 'wishlist', component: WishlistComponent, canActivate: [authGuard] },
  {
    path: 'admin', component: AdminComponent, canActivate: [adminGuard],
    children: [
      { path: '', component: AdminDashboardComponent },
      { path: 'products', component: AdminProductsComponent },
      { path: 'categories', component: AdminCategoriesComponent },
      { path: 'brands', component: AdminBrandsComponent },
      { path: 'orders', component: AdminOrdersComponent },
      { path: 'coupons', component: AdminCouponsComponent },
      { path: 'users', component: AdminUsersComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];