import { Component, OnInit } from '@angular/core';
import { ProductService } from '../product.service';
import { RentalService } from '../../rental/rental.service';
import { Product } from '../../models/Product';
import { AuthService } from '../../auth/auth.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';


@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styleUrls: ['./my-products.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class MyProductsComponent implements OnInit {
  myProducts: Product[] = [];
  allRentals: any[] = [];
  openRequestsForProductId: number | null = null;
  rentalRequests: any[] = [];
  message = '';
  loading = false;
  currentUserName = '';

  constructor(
    private productService: ProductService,
    private rentalService: RentalService,
    private authService: AuthService,
    private router: Router // <-- adaugă aici
  ) { }


  ngOnInit(): void {
    this.currentUserName = this.authService.getUserName() || '';
    this.loadMyProducts();
    this.loadAllRentals();
  }

  loadMyProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.myProducts = products.filter(p => p.userName === this.currentUserName);
      },
      error: () => this.message = 'Eroare la încărcarea produselor tale.'
    });
  }

  loadAllRentals(): void {
    this.rentalService.getMyRentals().subscribe({
      next: (rentals) => this.allRentals = rentals,
      error: () => this.allRentals = []
    });
  }

  showRequests(productId: number): void {
    if (this.openRequestsForProductId === productId) {
      this.openRequestsForProductId = null;
      return;
    }
    this.openRequestsForProductId = productId;
    this.rentalService.getRequestsForProduct(productId).subscribe({
      next: (requests) => this.rentalRequests = requests,
      error: () => this.rentalRequests = []
    });
  }
  editProduct(productId: number) {
    this.router.navigate(['/products-edit', productId]);
  }
  deleteProduct(productId: number): void {
    if (confirm('Ești sigur că vrei să ștergi acest produs?')) {
      this.loading = true;
      this.productService.delete(productId).subscribe({
        next: () => {
          this.message = 'Produs șters cu succes!';
          this.loadMyProducts();
          this.loading = false;
          setTimeout(() => this.message = '', 3000);
        },
        error: () => {
          this.message = 'Eroare la ștergerea produsului!';
          this.loading = false;
          setTimeout(() => this.message = '', 3000);
        }
      });
    }
  }
}
