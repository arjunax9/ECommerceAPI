import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-details',
  template: `
  <div style="max-width: 800px; margin: 0 auto; padding: 20px;">
    <div style="display: flex; align-items: center; justify-content: space-between; border-bottom: 2px solid #e2e8f0; padding-bottom: 12px; margin-bottom: 20px;">
      <h2 style="margin: 0;">Order Details</h2>
      <a routerLink="/orders" style="color: #4f46e5; text-decoration: none; font-weight: 500;">← Back to My Orders</a>
    </div>

    <div *ngIf="loading" style="padding: 15px; color: #4a5568;">Loading order details...</div>
    <div *ngIf="error" style="padding: 15px; color: #e53e3e; background: #fff5f5; border-radius: 6px; margin-bottom: 15px;">{{error}}</div>
    <div *ngIf="actionMessage" style="padding: 12px 16px; background-color: #def7ec; color: #03543f; border: 1px solid #bcf0da; border-radius: 6px; margin-bottom: 15px; font-weight: 500;">
      ✓ {{ actionMessage }}
    </div>

    <div *ngIf="order" style="border: 1px solid #e2e8f0; border-radius: 8px; padding: 24px; background: #ffffff; box-shadow: 0 1px 3px rgba(0,0,0,0.05);">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px;">
        <h3 style="margin: 0; font-size: 1.4rem;">Order #{{order.id}}</h3>
        <span [style.backgroundColor]="order.status === 'Paid' ? '#d1fae5' : (order.status === 'Pending' ? '#fef3c7' : '#e0f2fe')"
              [style.color]="order.status === 'Paid' ? '#065f46' : (order.status === 'Pending' ? '#92400e' : '#0369a1')"
              style="padding: 6px 16px; border-radius: 9999px; font-size: 0.9rem; font-weight: 600; text-transform: uppercase;">
          {{order.status}}
        </span>
      </div>

      <div style="margin-bottom: 20px; font-size: 1.2rem; font-weight: 600; color: #1e293b;">
        Total Amount: {{order.totalAmount | currency}}
      </div>

      <div style="margin-top: 20px;">
        <h4 style="margin: 0 0 10px 0; color: #475569; text-transform: uppercase; font-size: 0.85rem; letter-spacing: 0.05em;">Order Items</h4>
        <table style="width: 100%; border-collapse: collapse; margin-top: 8px;">
          <thead>
            <tr style="background: #f8fafc; border-bottom: 1px solid #e2e8f0; text-align: left;">
              <th style="padding: 10px 12px; font-size: 0.85rem; color: #64748b;">Product ID</th>
              <th style="padding: 10px 12px; font-size: 0.85rem; color: #64748b;">Quantity</th>
              <th style="padding: 10px 12px; font-size: 0.85rem; color: #64748b;">Price</th>
              <th style="padding: 10px 12px; font-size: 0.85rem; color: #64748b;">Subtotal</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let it of order.orderItems" style="border-bottom: 1px solid #f1f5f9;">
              <td style="padding: 10px 12px; color: #334155;">#{{it.productId}}</td>
              <td style="padding: 10px 12px; color: #334155;">{{it.quantity}}</td>
              <td style="padding: 10px 12px; color: #334155;">{{it.price | currency}}</td>
              <td style="padding: 10px 12px; font-weight: 500; color: #1e293b;">{{(it.price * it.quantity) | currency}}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div *ngIf="order.status === 'Pending'" style="margin-top: 24px; padding-top: 18px; border-top: 1px solid #e2e8f0;">
        <button (click)="approveOrder()"
                [disabled]="approving"
                style="background-color: #10b981; color: white; border: none; padding: 10px 22px; border-radius: 6px; font-size: 1rem; font-weight: 600; cursor: pointer;">
          {{ approving ? 'Approving Order...' : '✓ Approve & Pay Order Now' }}
        </button>
      </div>
    </div>
  </div>
  `
})
export class OrderDetailsComponent implements OnInit {
  order: any = null;
  loading = false;
  error = '';
  actionMessage = '';
  approving = false;

  constructor(private route: ActivatedRoute, private svc: OrderService) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.error = 'No order id provided';
      return;
    }
    this.loadOrder(id);
  }

  loadOrder(id: number): void {
    this.loading = true;
    this.svc.getById(id).subscribe({
      next: (o) => {
        this.order = o;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load order details';
        this.loading = false;
      }
    });
  }

  approveOrder(): void {
    if (!this.order?.id) return;
    this.approving = true;
    this.actionMessage = '';
    this.svc.approve(this.order.id).subscribe({
      next: () => {
        this.order.status = 'Paid';
        this.approving = false;
        this.actionMessage = `Order #${this.order.id} was approved and marked as Paid successfully!`;
      },
      error: (err) => {
        this.approving = false;
        alert('Failed to approve order: ' + (err?.error?.error || err?.message || 'Error occurred'));
      }
    });
  }
}
