import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-account',
  standalone: true,
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css'],
  imports: [CommonModule, FormsModule]
})
export class AccountComponent implements OnInit {
  userName: string | null = '';
  userEmail: string | null = '';
  userTelephone: string | null = '';
  message: string = '';

  showChangePassword = false;
  oldPassword = '';
  newPassword = '';
  showDetails: boolean = false;

  constructor(private authService: AuthService, private router: Router) { }


  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    } else {
      this.userName = this.authService.getUserName();
      this.userEmail = localStorage.getItem('email');
      this.userTelephone = localStorage.getItem('telephoneNumber');
    }
  }

  editAccount() {
    this.router.navigate(['/edit-account']);
  }

  changePassword() {
    this.authService.changePassword(this.oldPassword, this.newPassword).subscribe({
      next: (response) => {
        this.message = response.Message; 
        this.oldPassword = '';
        this.newPassword = '';
        this.showChangePassword = false;
      },
      error: (error) => {
        console.error('Eroare backend:', error);

        if (error.status === 400) {
          this.message = 'Parola veche este incorecta.';
        } else if (error.status === 401) {
          this.message = 'Autentificare esuata. Token invalid.';
        } else if (error.status === 404) {
          this.message = 'Utilizatorul nu a fost gasit.';
        } else {
          this.message = 'Eroare la schimbarea parolei. Te rugam sa incerci din nou.';
        }
      }
    });
  }

  goToMyRentals() {
    this.router.navigate(['/rentals/my']);
  }


}
