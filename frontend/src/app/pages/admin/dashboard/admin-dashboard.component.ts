import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../../core/services/dashboard.service';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CurrencyPipe, NgFor, NgIf],
  template: `
  <h1 class="text-2xl font-bold mb-4">Dashboard</h1>
  <div *ngIf="stats" class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
    <div class="bg-white p-4 rounded shadow"><p class="text-gray-500 text-sm">Users</p><p class="text-2xl font-bold">{{stats.totalUsers}}</p></div>
    <div class="bg-white p-4 rounded shadow"><p class="text-gray-500 text-sm">Products</p><p class="text-2xl font-bold">{{stats.totalProducts}}</p></div>
    <div class="bg-white p-4 rounded shadow"><p class="text-gray-500 text-sm">Orders</p><p class="text-2xl font-bold">{{stats.totalOrders}}</p></div>
    <div class="bg-white p-4 rounded shadow"><p class="text-gray-500 text-sm">Revenue</p><p class="text-2xl font-bold">{{stats.totalRevenue | currency}}</p></div>
    <div class="bg-white p-4 rounded shadow"><p class="text-gray-500 text-sm">Pending Orders</p><p class="text-2xl font-bold">{{stats.pendingOrders}}</p></div>
    <div class="bg-white p-4 rounded shadow"><p class="text-gray-500 text-sm">Low Stock</p><p class="text-2xl font-bold">{{stats.lowStock}}</p></div>
  </div>
  <div class="bg-white p-4 rounded shadow">
    <h2 class="font-bold mb-2">Recent Orders</h2>
    <div *ngFor="let o of stats?.recentOrders" class="flex justify-between py-1 border-b">
      <span>{{o.orderNumber}}</span><span>{{o.total | currency}}</span>
    </div>
  </div>
  `
})
export class AdminDashboardComponent implements OnInit {
  stats: any = null;
  constructor(private ds: DashboardService) {}
  ngOnInit() { this.ds.get().subscribe(s => this.stats = s); }
}