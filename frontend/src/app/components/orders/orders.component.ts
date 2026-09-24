import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';

@Component({ selector: 'app-orders', templateUrl: './orders.component.html' })
export class OrdersComponent implements OnInit {
  orders: any[] = [];
  loading = false;
  error = '';
  actionMessage = '';
  approvingId: number | null = null;

  constructor(private svc: OrderService, private auth: AuthService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    const uid = this.auth.getUserId();
    if (!uid) {
      this.error = 'Please login to see your orders';
      return;
    }
    this.loading = true;
    this.svc.getByUser(uid).subscribe({
      next: (o) => {
        this.orders = o;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load orders';
        this.loading = false;
      }
    });
  }

  approveOrder(order: any): void {
    this.approvingId = order.id;
    this.actionMessage = '';
    this.svc.approve(order.id).subscribe({
      next: () => {
        order.status = 'Paid';
        this.approvingId = null;
        this.actionMessage = `Order #${order.id} has been approved and marked as Paid successfully!`;
      },
      error: (err) => {
        this.approvingId = null;
        alert('Failed to approve order: ' + (err?.error?.error || err?.message || 'Error occurred'));
      }
    });
  }
}
