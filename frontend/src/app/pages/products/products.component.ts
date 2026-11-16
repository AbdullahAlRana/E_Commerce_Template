import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService, CategoryService } from '../../core/services';
import { Product, Category, PagedResult } from '../../core/models';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [RouterLink, FormsModule, CurrencyPipe, NgFor, NgIf],
  template: `
  <div class="flex gap-6">
    <aside class="w-64">
      <input [(ngModel)]="search" (keyup.enter)="load()" placeholder="Search" class="border rounded px-3 py-2 w-full mb-3" />
      <select [(ngModel)]="categoryId" (change)="load()" class="border rounded px-3 py-2 w-full mb-3">
        <option value="">All categories</option>
        <option *ngFor="let c of categories" [value]="c.id">{{c.name}}</option>
      </select>
      <button (click)="load()" class="bg-blue-600 text-white px-4 py-2 rounded w-full">Apply</button>
    </aside>
    <div class="flex-1">
      <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
        <div *ngFor="let p of result?.items" class="bg-white rounded shadow p-3">
          <a [routerLink]="['/products', p.id]">
            <img [src]="p.primaryImageUrl || 'https://via.placeholder.com/200'" class="w-full h-40 object-cover rounded" />
            <h3 class="font-semibold mt-2">{{p.name}}</h3>
            <p class="text-blue-600 font-bold">{{p.effectivePrice | currency}}</p>
          </a>
        </div>
      </div>
      <div class="mt-4 flex gap-2" *ngIf="result">
        <button (click)="prev()" [disabled]="page<=1" class="px-3 py-1 border rounded">Prev</button>
        <span>Page {{page}} / {{result.totalPages}}</span>
        <button (click)="next()" [disabled]="page>=result.totalPages" class="px-3 py-1 border rounded">Next</button>
      </div>
    </div>
  </div>
  `
})
export class ProductsComponent implements OnInit {
  result: PagedResult<Product> | null = null;
  categories: Category[] = [];
  search = ''; categoryId = ''; page = 1; pageSize = 12;
  constructor(private ps: ProductService, private cs: CategoryService) {}
  ngOnInit() {
    this.cs.getAll({ pageSize: 100 }).subscribe(r => this.categories = r.items);
    this.load();
  }
  load() {
    this.ps.getAll({ page: this.page, pageSize: this.pageSize, search: this.search || undefined, categoryId: this.categoryId || undefined }).subscribe(r => this.result = r);
  }
  prev() { if (this.page > 1) { this.page--; this.load(); } }
  next() { if (this.result && this.page < this.result.totalPages) { this.page++; this.load(); } }
}