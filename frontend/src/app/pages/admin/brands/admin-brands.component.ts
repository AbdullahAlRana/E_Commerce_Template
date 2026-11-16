import { Component, OnInit } from '@angular/core';
import { BrandService } from '../../../core/services/brand.service';
import { Brand } from '../../../core/models';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-brands',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
  <h1 class="text-2xl font-bold mb-4">Brands</h1>
  <div class="bg-white p-4 rounded shadow mb-4 max-w-md">
    <input [(ngModel)]="form.name" placeholder="Name" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="form.description" placeholder="Description" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="form.logoUrl" placeholder="Logo URL" class="border rounded px-3 py-2 w-full mb-2" />
    <button (click)="save()" class="bg-blue-600 text-white px-4 py-2 rounded">{{editing?'Update':'Create'}}</button>
    <button *ngIf="editing" (click)="cancel()" class="ml-2 px-4 py-2 border rounded">Cancel</button>
  </div>
  <table class="w-full bg-white rounded shadow">
    <thead><tr class="border-b"><th class="p-2 text-left">Name</th><th class="p-2">Actions</th></tr></thead>
    <tbody>
      <tr *ngFor="let b of items" class="border-b">
        <td class="p-2">{{b.name}}</td>
        <td class="p-2 text-center">
          <button (click)="edit(b)" class="text-blue-600 mr-2">Edit</button>
          <button (click)="del(b.id)" class="text-red-600">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>
  `
})
export class AdminBrandsComponent implements OnInit {
  items: Brand[] = [];
  form: any = { name: '', description: '', logoUrl: '' };
  editing: string | null = null;
  constructor(private svc: BrandService) {}
  ngOnInit() { this.load(); }
  load() { this.svc.getAll({ pageSize: 100 }).subscribe(r => this.items = r.items); }
  save() {
    if (this.editing) this.svc.update(this.editing, this.form).subscribe(() => { this.load(); this.cancel(); });
    else this.svc.create(this.form).subscribe(() => { this.load(); this.form = { name:'', description:'', logoUrl:'' }; });
  }
  edit(b: Brand) { this.editing = b.id; this.form = { name: b.name, description: b.description, logoUrl: b.logoUrl }; }
  cancel() { this.editing = null; this.form = { name:'', description:'', logoUrl:'' }; }
  del(id: string) { if (confirm('Delete?')) this.svc.delete(id).subscribe(() => this.load()); }
}