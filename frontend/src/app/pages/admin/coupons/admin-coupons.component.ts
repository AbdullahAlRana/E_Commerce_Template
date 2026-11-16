import { Component, OnInit } from '@angular/core';
import { CouponService } from '../../../core/services/coupon.service';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-coupons',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
  <h1 class="text-2xl font-bold mb-4">Coupons</h1>
  <div class="bg-white p-4 rounded shadow mb-4 max-w-md">
    <input [(ngModel)]="form.code" placeholder="Code" class="border rounded px-3 py-2 w-full mb-2" />
    <select [(ngModel)]="form.type" class="border rounded px-3 py-2 w-full mb-2">
      <option [value]="0">Percentage</option><option [value]="1">Fixed</option>
    </select>
    <input [(ngModel)]="form.value" type="number" placeholder="Value" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="form.minOrderAmount" type="number" placeholder="Min order" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="form.maxDiscount" type="number" placeholder="Max discount" class="border rounded px-3 py-2 w-full mb-2" />
    <button (click)="save()" class="bg-blue-600 text-white px-4 py-2 rounded">{{editing?'Update':'Create'}}</button>
    <button *ngIf="editing" (click)="cancel()" class="ml-2 px-4 py-2 border rounded">Cancel</button>
  </div>
  <table class="w-full bg-white rounded shadow text-sm">
    <thead><tr class="border-b"><th class="p-2">Code</th><th class="p-2">Type</th><th class="p-2">Value</th><th class="p-2">Actions</th></tr></thead>
    <tbody>
      <tr *ngFor="let c of items" class="border-b">
        <td class="p-2">{{c.code}}</td><td class="p-2">{{c.type}}</td><td class="p-2">{{c.value}}</td>
        <td class="p-2">
          <button (click)="edit(c)" class="text-blue-600 mr-2">Edit</button>
          <button (click)="del(c.id)" class="text-red-600">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>
  `
})
export class AdminCouponsComponent implements OnInit {
  items: any[] = [];
  form: any = { code: '', type: 0, value: 0, minOrderAmount: null, maxDiscount: null, startDate: new Date(), endDate: new Date(Date.now() + 30*24*3600*1000), isActive: true };
  editing: string | null = null;
  constructor(private svc: CouponService) {}
  ngOnInit() { this.load(); }
  load() { this.svc.getAll({ pageSize: 100 }).subscribe(r => this.items = r.items); }
  save() {
    if (this.editing) this.svc.update(this.editing, this.form).subscribe(() => { this.load(); this.cancel(); });
    else this.svc.create(this.form).subscribe(() => { this.load(); this.form.code=''; });
  }
  edit(c: any) { this.editing = c.id; this.form = { ...c }; }
  cancel() { this.editing = null; }
  del(id: string) { if (confirm('Delete?')) this.svc.delete(id).subscribe(() => this.load()); }
}