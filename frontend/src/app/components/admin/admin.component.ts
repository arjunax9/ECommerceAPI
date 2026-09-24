import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { InventoryService } from '../../services/inventory.service';
import { AuthService } from '../../services/auth.service';
import { OrderService } from '../../services/order.service';
import { Product } from '../../models/product';

@Component({
  selector: 'app-admin',
  template: `
  <div style="max-width: 1000px; margin: 0 auto; padding: 20px;">
    <h2>Admin Panel</h2>

    <div *ngIf="!auth.isAdmin()">
      <p style="color: #e53e3e;">You must be an admin to access this page.</p>
    </div>

    <div *ngIf="auth.isAdmin()">
      <section style="background: #fff; padding: 20px; border-radius: 8px; border: 1px solid #e2e8f0; margin-bottom: 24px;">
        <h3>Products</h3>
        <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
          <thead>
            <tr style="background: #f8fafc; text-align: left; border-bottom: 2px solid #e2e8f0;">
              <th style="padding: 8px;">Name</th>
              <th style="padding: 8px;">Price</th>
              <th style="padding: 8px;">Category</th>
              <th style="padding: 8px;">Quantity</th>
              <th style="padding: 8px;">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let p of products" style="border-bottom: 1px solid #f1f5f9;">
              <td style="padding: 8px;">{{p.name}}</td>
              <td style="padding: 8px;">{{p.price | number:'1.2-2'}}</td>
              <td style="padding: 8px;">{{p.category}}</td>
              <td style="padding: 8px;">{{p.quantity ?? '—'}}</td>
              <td style="padding: 8px;">
                <button (click)="startEdit(p)" style="margin-right: 6px;">Edit</button>
                <button (click)="remove(p.id)" style="color: #e53e3e;">Delete</button>
              </td>
            </tr>
          </tbody>
        </table>

        <h4 style="margin-top: 20px;">{{ editing ? 'Edit product' : 'Add product' }}</h4>
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 10px; max-width: 600px;">
          <div>
            <label>Name: <input [(ngModel)]="form.name" style="width: 100%;" /></label>
          </div>
          <div>
            <label>Description: <input [(ngModel)]="form.description" style="width: 100%;" /></label>
          </div>
          <div>
            <label>Price: <input type="number" [(ngModel)]="form.price" style="width: 100%;" /></label>
          </div>
          <div>
            <label>Category: <input [(ngModel)]="form.category" style="width: 100%;" /></label>
          </div>
          <div>
            <label>Quantity: <input type="number" [(ngModel)]="form.quantity" style="width: 100%;" /></label>
          </div>
        </div>
        <div style="margin-top: 12px;">
          <button (click)="save()" style="padding: 6px 16px; background: #4f46e5; color: white; border: none; border-radius: 4px; cursor: pointer;">{{ editing ? 'Update' : 'Create' }}</button>
          <button (click)="cancel()" *ngIf="editing" style="margin-left: 8px; padding: 6px 16px;">Cancel</button>
        </div>
      </section>

      <section style="background: #fff; padding: 20px; border-radius: 8px; border: 1px solid #e2e8f0; margin-bottom: 24px;">
        <h3>Order Management & Approval</h3>

        <div *ngIf="orderActionMessage" style="padding: 10px 14px; background-color: #def7ec; color: #03543f; border: 1px solid #bcf0da; border-radius: 6px; margin-bottom: 14px; font-weight: 500;">
          ✓ {{ orderActionMessage }}
        </div>

        <button (click)="loadAllOrders()" style="padding: 6px 14px; margin-bottom: 12px; cursor: pointer;">
          {{ loadingOrders ? 'Loading Orders...' : '↻ Refresh Orders List' }}
        </button>

        <div *ngIf="allOrders.length > 0" style="overflow-x: auto; margin-top: 8px;">
          <table style="width: 100%; border-collapse: collapse;">
            <thead>
              <tr style="background: #f8fafc; text-align: left; border-bottom: 2px solid #e2e8f0;">
                <th style="padding: 8px;">Order ID</th>
                <th style="padding: 8px;">User ID</th>
                <th style="padding: 8px;">Total</th>
                <th style="padding: 8px;">Status</th>
                <th style="padding: 8px;">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let ord of allOrders" style="border-bottom: 1px solid #f1f5f9;">
                <td style="padding: 8px; font-weight: 600;">#{{ord.id}}</td>
                <td style="padding: 8px;">User #{{ord.userId}}</td>
                <td style="padding: 8px; font-weight: 500;">{{ord.totalAmount | currency}}</td>
                <td style="padding: 8px;">
                  <span [style.backgroundColor]="ord.status === 'Paid' ? '#d1fae5' : (ord.status === 'Pending' ? '#fef3c7' : '#e0f2fe')"
                        [style.color]="ord.status === 'Paid' ? '#065f46' : (ord.status === 'Pending' ? '#92400e' : '#0369a1')"
                        style="padding: 3px 10px; border-radius: 9999px; font-size: 0.8rem; font-weight: 600;">
                    {{ord.status}}
                  </span>
                </td>
                <td style="padding: 8px;">
                  <button *ngIf="ord.status === 'Pending'"
                          (click)="approveOrder(ord)"
                          [disabled]="approvingOrderId === ord.id"
                          style="background-color: #10b981; color: white; border: none; padding: 5px 12px; border-radius: 4px; font-weight: 600; cursor: pointer;">
                    {{ approvingOrderId === ord.id ? 'Approving...' : 'Approve' }}
                  </button>
                  <span *ngIf="ord.status !== 'Pending'" style="color: #64748b; font-size: 0.9rem;">—</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div style="margin-top:20px; border-top: 1px solid #e2e8f0; padding-top: 16px;">
          <h4>Lookup Order By ID</h4>
          <div style="display: flex; gap: 8px; align-items: center;">
            <input type="number" [(ngModel)]="orderId" placeholder="Enter Order ID" style="padding: 6px; width: 150px;" />
            <button (click)="findOrder()" style="padding: 6px 14px; cursor: pointer;">Find</button>
          </div>

          <div *ngIf="orderResult" style="margin-top: 12px; padding: 14px; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px;">
            <div *ngIf="orderResult.id">
              <p><strong>Order ID:</strong> #{{orderResult.id}} | <strong>Status:</strong> {{orderResult.status}} | <strong>Total:</strong> {{orderResult.totalAmount | currency}}</p>
              <button *ngIf="orderResult.status === 'Pending'"
                      (click)="approveOrder(orderResult)"
                      [disabled]="approvingOrderId === orderResult.id"
                      style="background-color: #10b981; color: white; border: none; padding: 6px 14px; border-radius: 4px; font-weight: 600; cursor: pointer;">
                {{ approvingOrderId === orderResult.id ? 'Approving...' : '✓ Approve Order' }}
              </button>
            </div>
            <pre *ngIf="!orderResult.id">{{ orderResult | json }}</pre>
          </div>
        </div>
      </section>

      <section style="background: #fff; padding: 20px; border-radius: 8px; border: 1px solid #e2e8f0;">
        <h3>Import Products (Excel / CSV)</h3>
        <input type="file" (change)="onFile($event)" accept=".xlsx,.xls,.csv" />
        <button (click)="upload()" [disabled]="!importFile" style="margin-left: 8px;">Upload</button>
        <div *ngIf="importResult" style="margin-top: 10px;">
          <pre>{{ importResult | json }}</pre>
        </div>
      </section>
    </div>
  </div>
  `
})
export class AdminComponent implements OnInit {
  products: Product[] = [];
  allOrders: any[] = [];
  loadingOrders = false;
  editing: Product | null = null;
  form: any = { name: '', description: '', price: 0, category: '', quantity: 0 };
  orderId: number | null = null;
  orderResult: any = null;
  orderActionMessage = '';
  approvingOrderId: number | null = null;
  importFile: File | null = null;
  importResult: any = null;

  constructor(
    public auth: AuthService,
    private productSvc: ProductService,
    private inventorySvc: InventoryService,
    private orderSvc: OrderService
  ) {}

  ngOnInit(): void {
    this.load();
    if (this.auth.isAdmin()) {
      this.loadAllOrders();
    }
  }

  load() {
    this.productSvc.getAll().subscribe((res: any) => {
      this.products = (res || []).map((r: any) => ({
        id: r.id,
        name: r.name,
        description: r.description,
        price: r.price,
        category: r.category,
        quantity: r.quantity
      }));
    });
  }

  loadAllOrders() {
    this.loadingOrders = true;
    this.orderSvc.getAll().subscribe({
      next: (orders) => {
        this.allOrders = orders || [];
        this.loadingOrders = false;
      },
      error: () => {
        this.loadingOrders = false;
      }
    });
  }

  approveOrder(order: any) {
    this.approvingOrderId = order.id;
    this.orderActionMessage = '';
    this.orderSvc.approve(order.id).subscribe({
      next: () => {
        order.status = 'Paid';
        if (this.orderResult && this.orderResult.id === order.id) {
          this.orderResult.status = 'Paid';
        }
        this.approvingOrderId = null;
        this.orderActionMessage = `Order #${order.id} has been approved successfully!`;
      },
      error: (err) => {
        this.approvingOrderId = null;
        alert('Failed to approve order: ' + (err?.error?.error || err?.message || 'Error'));
      }
    });
  }

  startEdit(p: Product) {
    this.editing = { ...p } as Product;
    this.form = { ...p };
  }

  cancel() {
    this.editing = null;
    this.form = { name: '', description: '', price: 0, category: '', quantity: 0 };
  }

  save() {
    const payload: any = {
      name: this.form.name,
      description: this.form.description,
      price: Number(this.form.price) || 0,
      category: this.form.category,
      quantity: this.form.quantity == null ? undefined : Number(this.form.quantity)
    };

    if (this.editing) {
      this.productSvc.update(this.editing.id, payload).subscribe(() => {
        this.cancel();
        this.load();
      });
    } else {
      this.productSvc.create(payload).subscribe(() => {
        this.cancel();
        this.load();
      });
    }
  }

  onFile(ev: Event) {
    const input = ev.target as HTMLInputElement;
    const f = input?.files?.[0] || null;
    this.importFile = f;
  }

  upload() {
    if (!this.importFile) return;
    this.productSvc.importProducts(this.importFile).subscribe({
      next: (res) => { this.importResult = res; this.load(); },
      error: (err) => { this.importResult = { error: err?.message || err }; }
    });
  }

  remove(id: number) {
    if (!confirm('Delete product?')) return;
    this.productSvc.delete(id).subscribe(() => this.load());
  }

  findOrder() {
    this.orderResult = null;
    if (!this.orderId) return;
    this.orderSvc.getById(this.orderId).subscribe({
      next: (res) => this.orderResult = res,
      error: () => this.orderResult = { error: 'Not found or access denied' }
    });
  }
}
