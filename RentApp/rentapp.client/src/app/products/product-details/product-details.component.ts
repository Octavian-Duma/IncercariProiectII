import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../product.service';
import { Product } from '../../models/Product';
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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadProduct(id);
    }
  }

  loadProduct(id: number): void {
    this.productService.getById(id).subscribe({
      next: (product) => {
        this.product = product;
      },
      error: (error) => {
        console.error('Eroare la încărcarea produsului:', error);
        this.router.navigate(['/products']);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/products']);
  }
}
