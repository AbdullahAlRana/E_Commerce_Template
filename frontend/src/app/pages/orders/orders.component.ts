import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../core/services';
import { Order, PagedResult } from '../../core/models';
import { CurrencyPipe, NgFor, NgIf, DatePipe } from '@angular/common';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, NgFor, NgIf, DatePipe],
  template: `
  <h1 class="text-2xl font-bold mb-4">My Orders</h1>
  <div *ngFor="let o of result?.items" class="bg-white p-4 rounded shadow mb-3">
    <div class="flex justify-between">
      <a [routerLink]="['/orders', o.id]" class="font-bold text-blue-600">{{o.orderNumber}}</a>
      <span>{{o.total | currency}}</span>
    </div>
    <p class="text-sm text-gray-500">{{o.createdAt | date}} - Status: {{statusLabel(o.status)}}</p>
  </div>
  `
})
export class OrdersComponent implements OnInit {
  result: PagedResult<Order> | null = null;
  constructor(private os: OrderService) {}
  ngOnInit() { this.os.myOrders().subscribe(r => this.result = r); }
  statusLabel(s: number) { return ['Pending','Confirmed','Processing','Shipped','Delivered','Cancelled','Refunded'][s] || s; }
}