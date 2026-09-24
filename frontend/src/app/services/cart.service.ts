import { Injectable } from '@angular/core';
import { Product } from '../models/product';

@Injectable({ providedIn: 'root' })
export class CartService {
  private key = 'cart_items';

  getItems(): { product: Product, quantity: number }[] {
    const raw = localStorage.getItem(this.key);
    return raw ? JSON.parse(raw) : [];
  }

  save(items: { product: Product, quantity: number }[]) {
    localStorage.setItem(this.key, JSON.stringify(items));
  }

  add(product: Product, qty = 1) {
    const items = this.getItems();
    const existing = items.find(i => i.product.id === product.id);
    if (existing) existing.quantity += qty; else items.push({ product, quantity: qty });
    this.save(items);
  }

  clear() { localStorage.removeItem(this.key); }
}
