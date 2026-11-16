import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, NgIf],
  template: `
  <div class="max-w-md mx-auto bg-white p-6 rounded shadow">
    <h1 class="text-2xl font-bold mb-4">Register</h1>
    <input [(ngModel)]="firstName" placeholder="First name" class="border rounded px-3 py-2 w-full mb-3" />
    <input [(ngModel)]="lastName" placeholder="Last name" class="border rounded px-3 py-2 w-full mb-3" />
    <input [(ngModel)]="email" type="email" placeholder="Email" class="border rounded px-3 py-2 w-full mb-3" />
    <input [(ngModel)]="phone" placeholder="Phone" class="border rounded px-3 py-2 w-full mb-3" />
    <input [(ngModel)]="password" type="password" placeholder="Password" class="border rounded px-3 py-2 w-full mb-3" />
    <button (click)="submit()" class="bg-blue-600 text-white px-4 py-2 rounded w-full">Register</button>
    <p class="mt-3 text-sm">Have account? <a routerLink="/login" class="text-blue-600">Login</a></p>
    <p *ngIf="error" class="text-red-600 mt-2">{{error}}</p>
  </div>
  `
})
export class RegisterComponent {
  firstName=''; lastName=''; email=''; phone=''; password=''; error='';
  constructor(private auth: AuthService, private router: Router) {}
  submit() {
    this.auth.register({ firstName:this.firstName, lastName:this.lastName, email:this.email, phone:this.phone, password:this.password }).subscribe({
      next: () => this.router.navigate(['/']),
      error: e => this.error = e?.error?.message || 'Registration failed'
    });
  }
}