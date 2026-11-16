import { Component, OnInit } from '@angular/core';
import { AddressService } from '../../core/services';
import { Address } from '../../core/models';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-addresses',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  template: `
  <h1 class="text-2xl font-bold mb-4">Addresses</h1>
  <div class="grid md:grid-cols-2 gap-4">
    <div *ngFor="let a of addresses" class="bg-white p-4 rounded shadow">
      <p class="font-bold">{{a.fullName}}</p>
      <p>{{a.street}}, {{a.city}}, {{a.state}} {{a.zipCode}}, {{a.country}}</p>
      <p class="text-sm">{{a.phone}}</p>
      <button (click)="del(a.id)" class="text-red-600 mt-2">Delete</button>
    </div>
  </div>
  <div class="bg-white p-4 rounded shadow mt-4 max-w-md">
    <h2 class="font-bold mb-2">Add Address</h2>
    <input [(ngModel)]="m.fullName" placeholder="Full name" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="m.phone" placeholder="Phone" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="m.street" placeholder="Street" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="m.city" placeholder="City" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="m.state" placeholder="State" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="m.zipCode" placeholder="Zip" class="border rounded px-3 py-2 w-full mb-2" />
    <input [(ngModel)]="m.country" placeholder="Country" class="border rounded px-3 py-2 w-full mb-2" />
    <button (click)="save()" class="bg-blue-600 text-white px-4 py-2 rounded">Save</button>
  </div>
  `
})
export class AddressesComponent implements OnInit {
  addresses: Address[] = [];
  m: any = { fullName:'', phone:'', street:'', city:'', state:'', zipCode:'', country:'' };
  constructor(private svc: AddressService) {}
  ngOnInit() { this.load(); }
  load() { this.svc.getAll().subscribe(a => this.addresses = a); }
  save() { this.svc.create(this.m).subscribe(() => { this.load(); this.m = { fullName:'', phone:'', street:'', city:'', state:'', zipCode:'', country:'' }; }); }
  del(id: string) { this.svc.delete(id).subscribe(() => this.load()); }
}