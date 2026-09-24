import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Product } from '../models/product';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private base = `${environment.apiUrl}/products`;
  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<Product[]>(this.base);
  }

  getById(id: number) {
    return this.http.get<Product>(`${this.base}/${id}`);
  }

  create(payload: any) {
    return this.http.post(this.base, payload);
  }

  update(id: number, payload: any) {
    return this.http.put(`${this.base}/${id}`, payload);
  }

  delete(id: number) {
    return this.http.delete(`${this.base}/${id}`);
  }

  importProducts(file: File) {
    const fd = new FormData();
    fd.append('file', file, file.name);
    return this.http.post(`${this.base}/import`, fd);
  }
}
