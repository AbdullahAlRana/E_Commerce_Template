import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
  <h1 class="text-2xl font-bold mb-4">Users</h1>
  <table class="w-full bg-white rounded shadow text-sm">
    <thead><tr class="border-b"><th class="p-2">Email</th><th class="p-2">Name</th><th class="p-2">Roles</th><th class="p-2">Active</th><th class="p-2">Actions</th></tr></thead>
    <tbody>
      <tr *ngFor="let u of items" class="border-b">
        <td class="p-2">{{u.email}}</td>
        <td class="p-2">{{u.firstName}} {{u.lastName}}</td>
        <td class="p-2">{{u.roles?.join(', ')}}</td>
        <td class="p-2">{{u.isActive}}</td>
        <td class="p-2">
          <button (click)="toggle(u)" class="text-blue-600 mr-2">{{u.isActive?'Deactivate':'Activate'}}</button>
          <button (click)="makeAdmin(u)" class="text-green-600">Make Admin</button>
        </td>
      </tr>
    </tbody>
  </table>
  `
})
export class AdminUsersComponent implements OnInit {
  items: any[] = [];
  constructor(private svc: UserService) {}
  ngOnInit() { this.svc.getAll({ pageSize: 100 }).subscribe(r => this.items = r.items); }
  toggle(u: any) { this.svc.setActive(u.id, !u.isActive).subscribe(() => u.isActive = !u.isActive); }
  makeAdmin(u: any) { this.svc.assignRole(u.id, 'Admin').subscribe(() => { if (!u.roles.includes('Admin')) u.roles.push('Admin'); }); }
}