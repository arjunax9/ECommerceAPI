import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html'
})
export class CartComponent implements OnInit{
  items: any[] = [];
  total = 0;
  constructor(private cart: CartService, private router: Router) { }

  ngOnInit(): void{
    this.items = this.cart.getItems();
    this.recalc();
  }

  remove(i: number){
    this.items.splice(i,1);
    this.cart.save(this.items);
    this.recalc();
  }

  increase(i: number){
    this.items[i].quantity += 1;
    this.cart.save(this.items);
    this.recalc();
  }

  decrease(i: number){
    if (this.items[i].quantity>1) this.items[i].quantity -= 1;
    else this.remove(i);
    this.cart.save(this.items);
    this.recalc();
  }

  recalc(){
    this.total = this.items.reduce((s, it) => s + (it.product.price * it.quantity), 0);
  }

  checkout(){ this.router.navigate(['/checkout']); }
}
