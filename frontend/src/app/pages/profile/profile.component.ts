import { Component } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [NgIf],
  template: `
  <div class="bg-white p-6 rounded shadow max-w-md">
    <h1 class="text-2xl font-bold mb-4">Profile</h1>
    <p><strong>Name:</strong> {{auth.user()?.firstName}} {{auth.user()?.lastName}}</p>
    <p><strong>Email:</strong> {{auth.user()?.email}}</p>
    <p><strong>Roles:</strong> {{auth.user()?.roles?.join(', ')}}</p>
  </div>
  `
})
export class ProfileComponent {
  constructor(public auth: AuthService) {}
}