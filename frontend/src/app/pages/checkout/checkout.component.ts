import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AddressService, CartService, OrderService, CouponService } from '../../core/services';
import { Address, Cart } from '../../core/models';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CurrencyPipe, NgFor, NgIf, FormsModule],
  template: `
  <h1 class="text-2xl font-bold mb-4">Checkout</h1>
  <div *ngIf="cart" class="grid md:grid-cols-2 gap-6">
    <div>
      <h2 class="font-bold mb-2">Shipping Address</h2>
      <select [(ngModel)]="addressId" class="border rounded px-3 py-2 w-full mb-3">
        <option *ngFor="let a of addresses" [value]="a.id">{{a.fullName}} - {{a.street}}, {{a.city}}</option>
      </select>
      <h2 class="font-bold mb-2">Payment Method</h2>
      <select [(ngModel)]="paymentMethod" class="border rounded px-3 py-2 w-full mb-3">
        <option [value]="0">Cash on Delivery</option>
        <option [value]="1">Credit Card</option>
        <option [value]="3">PayPal</option>
      </select>
      <h2 class="font-bold mb-2">Coupon</h2>
      <div class="flex gap-2 mb-3">
        <input [(ngModel)]="couponCode" class="border rounded px-3 py-2 flex-1" placeholder="Coupon code" />
        <button (click)="applyCoupon()" class="bg-gray-600 text-white px-3 py-2 rounded">Apply</button>
      </div>
      <p *ngIf="couponMsg" class="text-sm text-green-600">{{couponMsg}}</p>
      <textarea [(ngModel)]="notes" class="border rounded w-full p-2" placeholder="Notes"></textarea>
      <button (click)="placeOrder()" class="bg-blue-600 text-white px-4 py-2 rounded mt-4">Place Order</button>
    </div>
    <div class="bg-white p-4 rounded shadow">
      <h2 class="font-bold mb-2">Order Summary</h2>
      <div *ngFor="let i of cart.items" class="flex justify-between text-sm mb-1">
        <span>{{i.productName}} x{{i.quantity}}</span>
        <span>{{i.total | currency}}</span>
      </div>
      <hr class="my-2" />
      <p class="font-bold">Subtotal: {{cart.subTotal | currency}}</p>
      <p *ngIf="discount">Discount: -{{discount | currency}}</p>
    </div>
  </div>
  `
})
export class CheckoutComponent implements OnInit {
  cart: Cart | null = null;
  addresses: Address[] = [];
  addressId = '';
  paymentMethod = 0;
  couponCode = '';
  discount = 0;
  couponMsg = '';
  notes = '';
  constructor(private cartSvc: CartService, private addrSvc: AddressService, private orderSvc: OrderService, private couponSvc: CouponService, private router: Router) {}
  ngOnInit() {
    this.cartSvc.load().subscribe(c => this.cart = c);
    this.addrSvc.getAll().subscribe(a => { this.addresses = a; if (a.length) this.addressId = a[0].id; });
  }
  applyCoupon() {
    if (!this.cart) return;
    this.couponSvc.validate(this.couponCode, this.cart.subTotal).subscribe(r => {
      if (r.valid) { this.discount = r.discount; this.couponMsg = r.message; }
      else { this.couponMsg = r.message; this.discount = 0; }
    });
  }
  placeOrder() {
    const data: any = { paymentMethod: +this.paymentMethod, notes: this.notes };
    if (this.addressId) data.shippingAddressId = this.addressId;
    if (this.couponCode) data.couponCode = this.couponCode;
    this.orderSvc.create(data).subscribe(o => this.router.navigate(['/orders', o.id]));
  }
}