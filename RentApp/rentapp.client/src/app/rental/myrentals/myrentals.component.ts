import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RentalService } from '../rental.service';

@Component({
  selector: 'app-my-rentals',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './myrentals.component.html',
  styleUrls: ['./myrentals.component.css']
})
export class MyRentalsComponent implements OnInit {
  rentals: any[] = [];
  message = '';

  constructor(private rentalService: RentalService) { }

  ngOnInit(): void {
    this.rentalService.getMyRentals().subscribe({
      next: (data) => {
        // Adaugăm status calculat local (Activ/Inactiv)
        this.rentals = data.map(r => ({
          ...r,
          status: this.getRentalStatus(r.startDate, r.endDate)
        }));
      },
      error: () => {
        this.message = 'Eroare la încărcarea închirierilor.';
      }
    });
  }

  getRentalStatus(startDateStr: string, endDateStr: string): string {
    const today = new Date();
    const start = new Date(startDateStr);
    const end = new Date(endDateStr);
    return (start <= today && today <= end) ? 'Activ' : 'Inactiv';
  }
}
