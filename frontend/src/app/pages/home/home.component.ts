import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../core/models';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, NgFor, NgIf],
  template: `
  <section class="mb-8">
    <h1 class="text-3xl font-bold mb-2">Welcome to E-Shop</h1>
    <p class="text-gray-600">Latest products and deals</p>
  </section>
  <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
    <div *ngFor="let p of products" class="bg-white rounded shadow p-3">
      <a [routerLink]="['/products', p.id]">
        <img [src]="p.primaryImageUrl || 'https://via.placeholder.com/200'" class="w-full h-40 object-cover rounded" />
        <h3 class="font-semibold mt-2">{{p.name}}</h3>
        <p class="text-blue-600 font-bold">{{p.effectivePrice | currency}}</p>
      </a>
    </div>
  </div>
  `
})
export class HomeComponent implements OnInit {
  products: Product[] = [];
  constructor(private ps: ProductService) {}
  ngOnInit() {
    this.ps.getAll({ pageSize: 8, featured: true }).subscribe(r => this.products = r.items);
    if (!this.products.length) this.ps.getAll({ pageSize: 8 }).subscribe(r => this.products = r.items);
  }
}