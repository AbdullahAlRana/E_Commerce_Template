import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Address } from '../models';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private base = `${environment.apiUrl}/addresses`;
  constructor(private http: HttpClient) {}
  getAll(): Observable<Address[]> { return this.http.get<Address[]>(this.base); }
  create(data: any): Observable<Address> { return this.http.post<Address>(this.base, data); }
  update(id: string, data: any): Observable<Address> { return this.http.put<Address>(`${this.base}/${id}`, data); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}