import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private base = `${environment.apiUrl}/products`;
  constructor(private http: HttpClient) {}
  getAll(params: any = {}): Observable<PagedResult<Product>> {
    return this.http.get<PagedResult<Product>>(this.base, { params });
  }
  get(id: string): Observable<Product> { return this.http.get<Product>(`${this.base}/${id}`); }
  create(data: any): Observable<Product> { return this.http.post<Product>(this.base, data); }
  update(id: string, data: any): Observable<Product> { return this.http.put<Product>(`${this.base}/${id}`, data); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}