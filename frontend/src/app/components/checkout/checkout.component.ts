import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';

@Component({ selector: 'app-checkout', templateUrl: './checkout.component.html' })
export class CheckoutComponent implements OnInit{
  items:any[] = [];
  message='';
  total = 0;
  paymentMethod: 'card'|'paypal' = 'card';

  constructor(private cart: CartService, private orders: OrderService, private auth: AuthService){ }

  ngOnInit(): void{
    this.items = this.cart.getItems();
    this.recalc();
  }

  recalc(){ this.total = this.items.reduce((s, it) => s + (it.product.price * it.quantity), 0); }

  place(){
    const userId = this.getUserId();
    if (!userId) { this.message = 'Please login to place order'; return; }
    const req = { userId: userId, items: this.items.map(i=>({ productId: i.product.id, quantity: i.quantity })), paymentMethod: this.paymentMethod };
    this.orders.create(req).subscribe({
      next: (r:any) => {
        const id = r?.id;
        if (!id) { this.message = 'Order created but no id returned'; return; }
        // perform checkout then payment sequentially
        this.orders.checkout(id).subscribe({
          next: () => {
            this.orders.pay(id).subscribe({
              next: () => {
                this.message = 'Payment successful';
                this.cart.clear();
                this.items = [];
                this.recalc();
              },
              error: () => { this.message = 'Payment failed'; }
            });
          },
          error: () => { this.message = 'Checkout failed'; }
        });
      },
      error: (e) => this.message='Failed to create order'
    });
  }

  getUserId(): number | null{
    const token = this.auth.getToken();
    if (!token) return null;
    try{ const payload = JSON.parse(atob(token.split('.')[1])); return Number(payload.sub); } catch { return null; }
  }
}
