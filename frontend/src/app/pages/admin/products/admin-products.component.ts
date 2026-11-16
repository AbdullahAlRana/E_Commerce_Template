import { Component, OnInit } from '@angular/core';
import { ProductService, CategoryService, BrandService } from '../../../core/services';
import { Product, Category, Brand } from '../../../core/models';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
  <h1 class="text-2xl font-bold mb-4">Products</h1>
  <div class="bg-white p-4 rounded shadow mb-4">
    <div class="grid md:grid-cols-2 gap-2">
      <input [(ngModel)]="form.name" placeholder="Name" class="border rounded px-3 py-2" />
      <input [(ngModel)]="form.sku" placeholder="SKU" class="border rounded px-3 py-2" />
      <input [(ngModel)]="form.price" type="number" placeholder="Price" class="border rounded px-3 py-2" />
      <input [(ngModel)]="form.discountPrice" type="number" placeholder="Discount Price" class="border rounded px-3 py-2" />
      <input [(ngModel)]="form.stockQuantity" type="number" placeholder="Stock" class="border rounded px-3 py-2" />
      <select [(ngModel)]="form.categoryId" class="border rounded px-3 py-2">
        <option *ngFor="let c of categories" [value]="c.id">{{c.name}}</option>
      </select>
      <select [(ngModel)]="form.brandId" class="border rounded px-3 py-2">
        <option value="">No brand</option>
        <option *ngFor="let b of brands" [value]="b.id">{{b.name}}</option>
      </select>
      <input [(ngModel)]="form.imageUrls" placeholder="Image URLs (comma separated)" class="border rounded px-3 py-2 md:col-span-2" />
      <textarea [(ngModel)]="form.description" placeholder="Description" class="border rounded px-3 py-2 md:col-span-2"></textarea>
    </div>
    <div class="mt-2">
      <label><input type="checkbox" [(ngModel)]="form.isActive" /> Active</label>
      <label class="ml-4"><input type="checkbox" [(ngModel)]="form.isFeatured" /> Featured</label>
    </div>
    <button (click)="save()" class="bg-blue-600 text-white px-4 py-2 rounded mt-3">{{editing?'Update':'Create'}}</button>
    <button *ngIf="editing" (click)="cancel()" class="ml-2 px-4 py-2 border rounded mt-3">Cancel</button>
  </div>
  <table class="w-full bg-white rounded shadow text-sm">
    <thead><tr class="border-b"><th class="p-2 text-left">Name</th><th class="p-2">Price</th><th class="p-2">Stock</th><th class="p-2">Actions</th></tr></thead>
    <tbody>
      <tr *ngFor="let p of items" class="border-b">
        <td class="p-2">{{p.name}}</td>
        <td class="p-2">{{p.effectivePrice}}</td>
        <td class="p-2">{{p.stockQuantity}}</td>
        <td class="p-2">
          <button (click)="edit(p)" class="text-blue-600 mr-2">Edit</button>
          <button (click)="del(p.id)" class="text-red-600">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>
  `
})
export class AdminProductsComponent implements OnInit {
  items: Product[] = [];
  categories: Category[] = [];
  brands: Brand[] = [];
  form: any = { name:'', sku:'', price:0, discountPrice:null, stockQuantity:0, categoryId:'', brandId:'', imageUrls:'', description:'', isActive:true, isFeatured:false };
  editing: string | null = null;
  constructor(private svc: ProductService, private cs: CategoryService, private bs: BrandService) {}
  ngOnInit() {
    this.load(); this.cs.getAll({ pageSize: 100 }).subscribe(r => this.categories = r.items); this.bs.getAll({ pageSize: 100 }).subscribe(r => this.brands = r.items);
  }
  load() { this.svc.getAll({ pageSize: 100 }).subscribe(r => this.items = r.items); }
  save() {
    const data: any = { ...this.form, imageUrls: (this.form.imageUrls || '').split(',').map((s: string) => s.trim()).filter(Boolean) };
    if (this.editing) this.svc.update(this.editing, data).subscribe(() => { this.load(); this.cancel(); });
    else this.svc.create(data).subscribe(() => { this.load(); this.form = { name:'', sku:'', price:0, discountPrice:null, stockQuantity:0, categoryId:'', brandId:'', imageUrls:'', description:'', isActive:true, isFeatured:false }; });
  }
  edit(p: Product) {
    this.editing = p.id;
    this.form = { name: p.name, sku: p.sku, price: p.price, discountPrice: p.discountPrice, stockQuantity: p.stockQuantity, categoryId: p.categoryId, brandId: p.brandId, imageUrls: (p.images || []).map(i => i.url).join(', '), description: p.description, isActive: p.isActive, isFeatured: p.isFeatured };
  }
  cancel() { this.editing = null; }
  del(id: string) { if (confirm('Delete?')) this.svc.delete(id).subscribe(() => this.load()); }
}