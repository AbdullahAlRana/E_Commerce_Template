import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../../core/services';
import { Order } from '../../core/models';
import { CurrencyPipe, NgFor, NgIf, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CurrencyPipe, NgFor, NgIf, DatePipe],
  template: `
  <div *ngIf="order">
    <h1 class="text-2xl font-bold mb-2">Order {{order.orderNumber}}</h1>
    <p>Status: {{statusLabel(order.status)}} | Payment: {{order.paymentStatus}}</p>
    <p>Placed: {{order.createdAt | date}}</p>
    <div class="bg-white p-4 rounded shadow mt-4">
      <div *ngFor="let i of order.items" class="flex justify-between py-1 border-b">
        <span>{{i.productName}} x{{i.quantity}}</span>
        <span>{{i.total | currency}}</span>
      </div>
      <div class="mt-2 text-right">
        <p>Subtotal: {{order.subTotal | currency}}</p>
        <p>Shipping: {{order.shippingCost | currency}}</p>
        <p>Tax: {{order.tax | currency}}</p>
        <p>Discount: -{{order.discount | currency}}</p>
        <p class="font-bold">Total: {{order.total | currency}}</p>
      </div>
    </div>
    <div *ngIf="order.shippingAddress" class="mt-4 bg-white p-4 rounded shadow">
      <h3 class="font-bold">Shipping Address</h3>
      <p>{{order.shippingAddress.fullName}}, {{order.shippingAddress.street}}, {{order.shippingAddress.city}}, {{order.shippingAddress.country}}</p>
    </div>
  </div>
  `
})
export class OrderDetailComponent implements OnInit {
  order: Order | null = null;
  constructor(private route: ActivatedRoute, private os: OrderService) {}
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.os.get(id).subscribe(o => this.order = o);
  }
  statusLabel(s: number) { return ['Pending','Confirmed','Processing','Shipped','Delivered','Cancelled','Refunded'][s] || s; }
}