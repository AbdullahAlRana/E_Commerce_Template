import { Component } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [RouterLink, RouterOutlet, RouterLinkActive, NgIf],
  template: `
  <div class="flex gap-6">
    <aside class="w-56 bg-white p-4 rounded shadow h-fit">
      <h2 class="font-bold mb-3">Admin</h2>
      <nav class="flex flex-col gap-2 text-sm">
        <a routerLink="/admin" routerLinkActive="font-bold" [routerLinkActiveOptions]="{exact:true}">Dashboard</a>
        <a routerLink="/admin/products">Products</a>
        <a routerLink="/admin/categories">Categories</a>
        <a routerLink="/admin/brands">Brands</a>
        <a routerLink="/admin/orders">Orders</a>
        <a routerLink="/admin/coupons">Coupons</a>
        <a routerLink="/admin/users">Users</a>
      </nav>
    </aside>
    <div class="flex-1"><router-outlet /></div>
  </div>
  `
})
export class AdminComponent {
  constructor(public auth: AuthService) {}
}