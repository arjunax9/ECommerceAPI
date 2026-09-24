import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit{
  products: any[] = [];
  loading = false;
  error = '';

  constructor(private svc: ProductService, private cart: CartService) {}

  ngOnInit(): void {
    this.load();
  }

  load(){
    this.loading = true;
    this.svc.getAll().subscribe({
      next: p => { this.products = p; this.loading = false; },
      error: () => { this.error = 'Failed to load products'; this.loading = false; }
    });
  }

  addToCart(product: any){
    this.cart.add(product, 1);
    // simple feedback
    this.error = `${product.name} added to cart`;
    setTimeout(()=> this.error = '', 2000);
  }
}
