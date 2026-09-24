import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  private base = `${environment.apiUrl}/inventory`;
  constructor(private http: HttpClient) {}

  getQuantity(productId: number) {
    return this.http.get<{ productId: number; quantity: number }>(`${this.base}/${productId}`);
  }

  updateQuantity(productId: number, quantity: number) {
    return this.http.put(`${this.base}/${productId}`, quantity);
  }
}
