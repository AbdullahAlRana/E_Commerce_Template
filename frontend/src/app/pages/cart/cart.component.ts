import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartService } from '../../core/services';
import { Cart } from '../../core/models';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, NgFor, NgIf],
  template: `
  <h1 class="text-2xl font-bold mb-4">Cart</h1>
  <div *ngIf="cart && cart.items.length; else empty">
    <div *ngFor="let i of cart.items" class="flex items-center gap-4 bg-white p-3 rounded shadow mb-2">
      <img [src]="i.productImage || 'https://via.placeholder.com/60'" class="w-16 h-16 object-cover rounded" />
      <div class="flex-1">
        <p class="font-semibold">{{i.productName}}</p>
        <p>{{i.unitPrice | currency}} x {{i.quantity}}</p>
      </div>
      <input type="number" [value]="i.quantity" (change)="update(i.id, $any($event.target).value)" class="border rounded w-16 px-2 py-1" />
      <p class="font-bold">{{i.total | currency}}</p>
      <button (click)="remove(i.id)" class="text-red-600">Remove</button>
    </div>
    <div class="text-right mt-4">
      <p class="text-xl font-bold">Subtotal: {{cart.subTotal | currency}}</p>
      <a routerLink="/checkout" class="bg-blue-600 text-white px-4 py-2 rounded inline-block mt-2">Checkout</a>
    </div>
  </div>
  <ng-template #empty><p>Your cart is empty.</p></ng-template>
  `
})
export class CartComponent implements OnInit {
  cart: Cart | null = null;
  constructor(private cartSvc: CartService) {}
  ngOnInit() { this.cartSvc.load().subscribe(c => this.cart = c); this.cartSvc.cart$.subscribe(c => this.cart = c); }
  update(id: string, q: string) { this.cartSvc.update(id, +q).subscribe(); }
  remove(id: string) { this.cartSvc.remove(id).subscribe(); }
}