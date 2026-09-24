import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private base = `${environment.apiUrl}/orders`;
  constructor(private http: HttpClient) {}

  create(order: any) {
    return this.http.post(this.base, order);
  }

  getById(id: number) {
    return this.http.get(`${this.base}/${id}`);
  }

  getByUser(userId: number) {
    return this.http.get<any[]>(`${this.base}/user/${userId}`);
  }

  checkout(id: number) {
    return this.http.post(`${this.base}/${id}/checkout`, {});
  }

  pay(id: number) {
    return this.http.post(`${this.base}/${id}/payment`, {});
  }

  getAll() {
    return this.http.get<any[]>(this.base);
  }

  approve(id: number) {
    return this.http.post(`${this.base}/${id}/approve`, {});
  }
}
