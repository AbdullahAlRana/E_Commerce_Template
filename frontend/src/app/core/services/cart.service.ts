import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cart } from '../models';

@Injectable({ providedIn: 'root' })
export class CartService {
  private base = `${environment.apiUrl}/cart`;
  private cartSubject = new BehaviorSubject<Cart | null>(null);
  cart$ = this.cartSubject.asObservable();
  constructor(private http: HttpClient) {}
  load(): Observable<Cart> { return this.http.get<Cart>(this.base).pipe(tap(c => this.cartSubject.next(c))); }
  add(productId: string, quantity = 1): Observable<Cart> {
    return this.http.post<Cart>(`${this.base}/items`, { productId, quantity }).pipe(tap(c => this.cartSubject.next(c)));
  }
  update(itemId: string, quantity: number): Observable<Cart> {
    return this.http.put<Cart>(`${this.base}/items/${itemId}`, { quantity }).pipe(tap(c => this.cartSubject.next(c)));
  }
  remove(itemId: string): Observable<Cart> {
    return this.http.delete<Cart>(`${this.base}/items/${itemId}`).pipe(tap(c => this.cartSubject.next(c)));
  }
  clear(): Observable<void> { return this.http.delete<void>(this.base).pipe(tap(() => this.cartSubject.next(null))); }
  get current() { return this.cartSubject.value; }
}