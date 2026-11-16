import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UserService {
  private base = `${environment.apiUrl}/users`;
  constructor(private http: HttpClient) {}
  getAll(params: any = {}): Observable<any> { return this.http.get<any>(this.base, { params }); }
  get(id: string): Observable<any> { return this.http.get<any>(`${this.base}/${id}`); }
  setActive(id: string, active: boolean): Observable<void> { return this.http.put<void>(`${this.base}/${id}/active`, null, { params: { active } }); }
  assignRole(id: string, role: string): Observable<void> { return this.http.post<void>(`${this.base}/${id}/roles`, null, { params: { role } }); }
}