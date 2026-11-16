import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, UserInfo } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = environment.apiUrl;
  accessToken: string | null = null;
  refreshToken: string | null = null;
  user = signal<UserInfo | null>(null);

  constructor(private http: HttpClient, private router: Router) {
    const t = localStorage.getItem('accessToken');
    const r = localStorage.getItem('refreshToken');
    const u = localStorage.getItem('user');
    if (t) this.accessToken = t;
    if (r) this.refreshToken = r;
    if (u) this.user.set(JSON.parse(u));
  }

  register(data: any) {
    return this.http.post<AuthResponse>(`${this.base}/auth/register`, data).pipe(tap(res => this.store(res)));
  }
  login(data: { email: string; password: string }) {
    return this.http.post<AuthResponse>(`${this.base}/auth/login`, data).pipe(tap(res => this.store(res)));
  }
  private store(res: AuthResponse) {
    this.accessToken = res.accessToken;
    this.refreshToken = res.refreshToken;
    this.user.set(res.user);
    localStorage.setItem('accessToken', res.accessToken);
    localStorage.setItem('refreshToken', res.refreshToken);
    localStorage.setItem('user', JSON.stringify(res.user));
  }
  logout() {
    this.accessToken = null;
    this.refreshToken = null;
    this.user.set(null);
    localStorage.clear();
    this.router.navigate(['/login']);
  }
  get isLoggedIn() { return !!this.accessToken; }
  get isAdmin() { return this.user()?.roles?.includes('Admin') ?? false; }
}