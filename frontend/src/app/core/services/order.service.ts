import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Order, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private base = `${environment.apiUrl}/orders`;
  constructor(private http: HttpClient) {}
  myOrders(page = 1, pageSize = 20): Observable<PagedResult<Order>> { return this.http.get<PagedResult<Order>>(`${this.base}/my`, { params: { page, pageSize } }); }
  get(id: string): Observable<Order> { return this.http.get<Order>(`${this.base}/${id}`); }
  create(data: any): Observable<Order> { return this.http.post<Order>(this.base, data); }
  all(page = 1, pageSize = 20): Observable<PagedResult<Order>> { return this.http.get<PagedResult<Order>>(this.base, { params: { page, pageSize } }); }
  updateStatus(id: string, data: any): Observable<Order> { return this.http.put<Order>(`${this.base}/${id}/status`, data); }
}