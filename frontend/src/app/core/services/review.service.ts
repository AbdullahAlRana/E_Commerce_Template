import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Review } from '../models';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private base = `${environment.apiUrl}/reviews`;
  constructor(private http: HttpClient) {}
  byProduct(productId: string): Observable<Review[]> { return this.http.get<Review[]>(`${this.base}/product/${productId}`); }
  create(data: any): Observable<Review> { return this.http.post<Review>(this.base, data); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}