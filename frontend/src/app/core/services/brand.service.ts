import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Brand, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class BrandService {
  private base = `${environment.apiUrl}/brands`;
  constructor(private http: HttpClient) {}
  getAll(params: any = {}): Observable<PagedResult<Brand>> { return this.http.get<PagedResult<Brand>>(this.base, { params }); }
  get(id: string): Observable<Brand> { return this.http.get<Brand>(`${this.base}/${id}`); }
  create(data: any): Observable<Brand> { return this.http.post<Brand>(this.base, data); }
  update(id: string, data: any): Observable<Brand> { return this.http.put<Brand>(`${this.base}/${id}`, data); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}