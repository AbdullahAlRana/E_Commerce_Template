import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private base = `${environment.apiUrl}/categories`;
  constructor(private http: HttpClient) {}
  getAll(params: any = {}): Observable<PagedResult<Category>> { return this.http.get<PagedResult<Category>>(this.base, { params }); }
  get(id: string): Observable<Category> { return this.http.get<Category>(`${this.base}/${id}`); }
  create(data: any): Observable<Category> { return this.http.post<Category>(this.base, data); }
  update(id: string, data: any): Observable<Category> { return this.http.put<Category>(`${this.base}/${id}`, data); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}