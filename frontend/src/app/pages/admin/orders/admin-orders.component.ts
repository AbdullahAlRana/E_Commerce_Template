import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../core/models';
import { CurrencyPipe, NgFor, NgIf, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CurrencyPipe, NgFor, NgIf, FormsModule, DatePipe],
  template: `
  <h1 class="text-2xl font-bold mb-4">Orders</h1>
  <div *ngFor="let o of items" class="bg-white p-4 rounded shadow mb-3">
    <div class="flex justify-between items-center">
      <span class="font-bold">{{o.orderNumber}}</span>
      <span>{{o.total | currency}}</span>
    </div>
    <p class="text-sm text-gray-500">{{o.createdAt | date}} - {{o.customerEmail}}</p>
    <div class="mt-2 flex gap-2 items-center">
      <select [(ngModel)]="o.status" class="border rounded px-2 py-1">
        <option [value]="0">Pending</option><option [value]="1">Confirmed</option><option [value]="2">Processing</option>
        <option [value]="3">Shipped</option><option [value]="4">Delivered</option><option [value]="5">Cancelled</option>
      </select>
      <button (click)="update(o)" class="bg-blue-600 text-white px-3 py-1 rounded">Update</button>
    </div>
  </div>
  `
})
export class AdminOrdersComponent implements OnInit {
  items: Order[] = [];
  constructor(private svc: OrderService) {}
  ngOnInit() { this.svc.all(1, 50).subscribe(r => this.items = r.items); }
  update(o: Order) { this.svc.updateStatus(o.id, { status: +o.status }).subscribe(); }
}