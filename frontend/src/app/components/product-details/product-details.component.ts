import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit{
  product: any = null;
  loading = false;
  error = '';

  constructor(private route: ActivatedRoute, private svc: ProductService){}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) this.load(id);
  }

  load(id: number){
    this.loading = true;
    this.svc.getById(id).subscribe({
      next: p => { this.product = p; this.loading = false; },
      error: () => { this.error = 'Failed to load product'; this.loading = false; }
    });
  }
}
