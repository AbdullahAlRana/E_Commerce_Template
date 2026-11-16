import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
  <h1 class="text-2xl font-bold mb-4">Categories</h1>
  <div class="bg-white p-4 rounded shadow mb-4 max-w-md">
    <input [(ngModel)]="form.name" placeholder="Name" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="form.description" placeholder="Description" class="border rounded px-3 py-2 w-full mb-2" />
    <button (click)="save()" class="bg-blue-600 text-white px-4 py-2 rounded">{{editing?'Update':'Create'}}</button>
    <button *ngIf="editing" (click)="cancel()" class="ml-2 px-4 py-2 border rounded">Cancel</button>
  </div>
  <table class="w-full bg-white rounded shadow">
    <thead><tr class="border-b"><th class="p-2 text-left">Name</th><th class="p-2">Products</th><th class="p-2">Actions</th></tr></thead>
    <tbody>
      <tr *ngFor="let c of items" class="border-b">
        <td class="p-2">{{c.name}}</td>
        <td class="p-2 text-center">{{c.productCount}}</td>
        <td class="p-2 text-center">
          <button (click)="edit(c)" class="text-blue-600 mr-2">Edit</button>
          <button (click)="del(c.id)" class="text-red-600">Delete</button>
        </td>
      </tr>
    </tbody>
  </table>
  `
})
export class AdminCategoriesComponent implements OnInit {
  items: Category[] = [];
  form: any = { name: '', description: '' };
  editing: string | null = null;
  constructor(private svc: CategoryService) {}
  ngOnInit() { this.load(); }
  load() { this.svc.getAll({ pageSize: 100 }).subscribe(r => this.items = r.items); }
  save() {
    if (this.editing) this.svc.update(this.editing, this.form).subscribe(() => { this.load(); this.cancel(); });
    else this.svc.create(this.form).subscribe(() => { this.load(); this.form = { name:'', description:'' }; });
  }
  edit(c: Category) { this.editing = c.id; this.form = { name: c.name, description: c.description }; }
  cancel() { this.editing = null; this.form = { name:'', description:'' }; }
  del(id: string) { if (confirm('Delete?')) this.svc.delete(id).subscribe(() => this.load()); }
}