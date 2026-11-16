import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CouponService {
  private base = `${environment.apiUrl}/coupons`;
  constructor(private http: HttpClient) {}
  validate(code: string, orderAmount: number): Observable<any> { return this.http.post<any>(`${this.base}/validate`, { code, orderAmount }); }
  getAll(params: any = {}): Observable<any> { return this.http.get<any>(this.base, { params }); }
  create(data: any): Observable<any> { return this.http.post<any>(this.base, data); }
  update(id: string, data: any): Observable<any> { return this.http.put<any>(`${this.base}/${id}`, data); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}