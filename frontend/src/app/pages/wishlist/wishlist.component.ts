import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { WishlistService } from '../../core/services';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [RouterLink, NgFor],
  template: `
  <h1 class="text-2xl font-bold mb-4">Wishlist</h1>
  <div *ngFor="let id of ids" class="bg-white p-3 rounded shadow mb-2 flex justify-between">
    <a [routerLink]="['/products', id]" class="text-blue-600">{{id}}</a>
    <button (click)="remove(id)" class="text-red-600">Remove</button>
  </div>
  `
})
export class WishlistComponent implements OnInit {
  ids: string[] = [];
  constructor(private svc: WishlistService) {}
  ngOnInit() { this.svc.get().subscribe((i: string[]) => this.ids = i); }
  remove(id: string) { this.svc.remove(id).subscribe(() => this.ids = this.ids.filter(x => x !== id)); }
}