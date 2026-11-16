import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService, CartService, ReviewService, WishlistService } from '../../core/services';
import { Product, Review } from '../../core/models';
import { CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, NgFor, NgIf, FormsModule],
  template: `
  <div *ngIf="product" class="grid md:grid-cols-2 gap-6">
    <div>
      <img [src]="product.primaryImageUrl || 'https://via.placeholder.com/400'" class="w-full rounded shadow" />
    </div>
    <div>
      <h1 class="text-3xl font-bold">{{product.name}}</h1>
      <p class="text-gray-600 mt-2">{{product.description}}</p>
      <p class="text-2xl text-blue-600 font-bold mt-4">{{product.effectivePrice | currency}}</p>
      <p class="text-sm text-gray-500">Stock: {{product.stockQuantity}}</p>
      <div class="mt-4 flex gap-2">
        <input type="number" [(ngModel)]="qty" min="1" class="border rounded px-3 py-2 w-20" />
        <button (click)="addToCart()" class="bg-blue-600 text-white px-4 py-2 rounded">Add to Cart</button>
        <button (click)="toggleWishlist()" class="border px-4 py-2 rounded">Wishlist</button>
      </div>
      <h3 class="font-bold mt-6 mb-2">Reviews</h3>
      <div *ngFor="let r of reviews" class="border-b py-2">
        <p class="font-semibold">{{r.userName}} <span class="text-yellow-500">{{r.rating}}/5</span></p>
        <p>{{r.comment}}</p>
      </div>
      <div class="mt-4">
        <select [(ngModel)]="newRating" class="border rounded px-2 py-1">
          <option [value]="5">5</option><option [value]="4">4</option><option [value]="3">3</option><option [value]="2">2</option><option [value]="1">1</option>
        </select>
        <textarea [(ngModel)]="newComment" class="border rounded w-full mt-2 p-2" placeholder="Comment"></textarea>
        <button (click)="submitReview()" class="bg-green-600 text-white px-4 py-2 rounded mt-2">Submit Review</button>
      </div>
    </div>
  </div>
  `
})
export class ProductDetailComponent implements OnInit {
  product: Product | null = null;
  reviews: Review[] = [];
  qty = 1; newRating = 5; newComment = '';
  constructor(private route: ActivatedRoute, private ps: ProductService, private cart: CartService, private rs: ReviewService, private ws: WishlistService) {}
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.ps.get(id).subscribe(p => this.product = p);
    this.rs.byProduct(id).subscribe(r => this.reviews = r);
  }
  addToCart() { if (this.product) this.cart.add(this.product.id, this.qty).subscribe(); }
  toggleWishlist() { if (this.product) this.ws.add(this.product.id).subscribe(); }
  submitReview() {
    if (!this.product) return;
    this.rs.create({ productId: this.product.id, rating: this.newRating, comment: this.newComment }).subscribe(r => { this.reviews.unshift(r); this.newComment=''; });
  }
}