import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../product.service';
import { Product } from '../../models/Product';
import { Review } from '../../models/Review';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class ProductDetailsComponent implements OnInit {
  product: Product | null = null;
  isLoading: boolean = true;
  errorMessage: string = '';
  reviews: Review[] = [];
  averageRating: number = 0;
  Math = Math;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadProduct(id);
      this.loadReviews(id);
      this.loadAverageRating(id);
    }
  }

  loadProduct(id: number): void {
    this.isLoading = true;
    this.productService.getById(id).subscribe({
      next: (response: any) => {
        let productData: any;
        if (response && response.product) {
          productData = response.product;
        } else {
          productData = response;
        }
        this.product = this.mapToProduct(productData);
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Nu am putut încărca detaliile produsului.';
        this.isLoading = false;
      }
    });
  }

  loadReviews(productId: number): void {
    this.productService.getReviews(productId).subscribe({
      next: (reviews) => this.reviews = reviews,
      error: () => this.reviews = []
    });
  }

  loadAverageRating(productId: number): void {
    this.productService.getAverageRating(productId).subscribe({
      next: (res) => this.averageRating = res.average,
      error: () => this.averageRating = 0
    });
  }

  mapToProduct(data: any): Product {
    return {
      id: data.id || 0,
      name: data.name || 'Fără nume',
      category: data.category || 'Nedefinit',
      location: data.location || 'Nespecificat',
      description: data.description || 'Fără descriere',
      pricePerDay: data.pricePerDay || 0,
      available: data.available !== undefined ? data.available : true,
      addedAt: data.addedAt ? new Date(data.addedAt) : new Date(),
      userName: data.userName || 'Necunoscut',
      telephoneNumber: data.telephoneNumber || undefined,
      imagePath: data.imagePath || undefined
    };
  }

  goBack(): void {
    this.router.navigate(['/products']);
  }
  
}
